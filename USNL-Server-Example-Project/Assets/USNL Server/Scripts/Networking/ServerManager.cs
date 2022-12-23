using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace USNL {
    public class ServerManager : MonoBehaviour {
        #region Variables

        public static ServerManager instance;

        [Header("Editor Config")]
        [SerializeField] private Package.ServerConfig serverConfig;
        [SerializeField] private bool useServerConfigAndDataFilesInEditor;

        [Header("Server Config")]
        [Tooltip("In seconds.")]
        [SerializeField] private float timeoutTime = 6f;
        [SerializeField] private float serverInfoPacketSendInterval = 1f;

        private bool isMigratingHost = false;
        private DateTime timeOfStartup;

        private float lastServerInfoPacketSentTime;

        public Package.ServerConfig ServerConfig { get => serverConfig; set => serverConfig = value; }
        
        public bool IsMigratingHost { get => isMigratingHost; set => isMigratingHost = value; }
        public DateTime TimeOfStartup { get => timeOfStartup; set => timeOfStartup = value; }
        public bool ServerActive { get => USNL.Package.Server.ServerData.IsServerActive; }

        #endregion

        #region Core

        private void Awake() {
            if (instance == null) {
                instance = this;
            } else if (instance != this) {
                Debug.Log("Server Manager instance already exists, destroying object!");
                Destroy(this);
            }

            if (Application.isEditor) {
                Application.runInBackground = true;
            }

            Debug.LogError("Opened Console.");
        }

        private void Start() {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }

        private void Update() {
            LookForServerQuitFile();
            CheckClientsTimedout();
            ContinuouslySendServerInfoPackets();
        }

        private void OnEnable() { USNL.CallbackEvents.OnWelcomeReceivedPacket += OnWelcomeReceivedPacket; }
        private void OnDisable() { USNL.CallbackEvents.OnWelcomeReceivedPacket -= OnWelcomeReceivedPacket; }

        private void OnApplicationQuit() {
            StopServer();
        }

        #endregion

        #region Server Manager

        public void StartServer() {
            Package.Server.ServerData.IsServerActive = true;

            if (useServerConfigAndDataFilesInEditor && Application.isEditor) {
                WriteServerDataFile();
                ReadServerConfigFile();
            }
            
            Package.Server.Start(serverConfig.MaxClients, serverConfig.ServerPort);

            TimeOfStartup = DateTime.Now;
        }

        public void StopServer() {
            Package.Server.CommandDisconnectAllClients("Server is shutting down.");

            Package.Server.ServerData.IsServerActive = false;
            if (useServerConfigAndDataFilesInEditor && Application.isEditor)
                WriteServerDataFile();
            
            StartCoroutine(Package.Server.ShutdownServer());
        }

        private void OnWelcomeReceivedPacket(object _packetObject) {
            USNL.Package.WelcomeReceivedPacket _wrp = (USNL.Package.WelcomeReceivedPacket)_packetObject;

            Debug.Log($"{Package.Server.Clients[_wrp.FromClient].Tcp.socket.Client.RemoteEndPoint} connected successfully and is now Player {_wrp.FromClient}.");
            if (_wrp.FromClient != _wrp.ClientIdCheck) {
                Debug.Log($"ID: ({_wrp.FromClient}) has assumed the wrong client ID ({_wrp.ClientIdCheck}).");
            }

            USNL.Package.PacketSend.ServerInfo(_wrp.FromClient, serverConfig.ServerName, GetConnectedClientsIds(), serverConfig.MaxClients, GetNumberOfConnectedClients() >= serverConfig.MaxClients);

            USNL.CallbackEvents.CallOnClientConnectedCallbacks(_wrp.FromClient);
        }

        #endregion

        #region Helper Functions

        private void CheckClientsTimedout() {
            for (int i = 0; i < USNL.Package.Server.Clients.Count; i++) {
                if (!USNL.Package.Server.Clients[i].IsConnected) continue;
                if (-USNL.Package.Server.Clients[i].Tcp.LastPacketTime.Subtract(DateTime.Now).TotalSeconds < timeoutTime) continue;

                Debug.Log($"Client {i} has timed out.");
                USNL.Package.Server.Clients[i].Disconnect();
            }
        }

        private void SendServerInfoPacketToAllClients() {
            for (int i = 0; i < USNL.Package.Server.Clients.Count; i++) {
                if (!USNL.Package.Server.Clients[i].IsConnected) continue;

                USNL.Package.PacketSend.ServerInfo(USNL.Package.Server.Clients[i].ClientId, serverConfig.ServerName, GetConnectedClientsIds(), serverConfig.MaxClients, GetNumberOfConnectedClients() >= serverConfig.MaxClients);
            }
        }

        private void ContinuouslySendServerInfoPackets() {
            if (Time.time - lastServerInfoPacketSentTime > serverInfoPacketSendInterval) {
                SendServerInfoPacketToAllClients();
                lastServerInfoPacketSentTime = Time.time;
            }
        }
        
        #endregion

        #region Public Functions

        public static int GetNumberOfConnectedClients() {
            int result = 0;
            for (int i = 0; i < USNL.Package.Server.Clients.Count; i++) {
                if (USNL.Package.Server.Clients[i].IsConnected)
                    result++;
            }
            return result;
        }

        public static int[] GetConnectedClientsIds() {
            List<int> result = new List<int>();
            for (int i = 0; i < USNL.Package.Server.Clients.Count; i++) {
                if (USNL.Package.Server.Clients[i].IsConnected)
                    result.Add(i);
            }
            return result.ToArray();
        }
        
        public void ClientDisconnected(int _clientId) {
            USNL.CallbackEvents.CallOnClientDisconnectedCallbacks(_clientId);
        }

        #endregion

        #region Server Config and Data

        private void LookForServerQuitFile() {
            if (File.Exists(GetApplicationPath() + "ServerQuit")) {
                Debug.Log("Server Quit commanded from host client, shutting down server.");
                File.Delete(GetApplicationPath() + "ServerQuit");
                StopServer();
                Application.Quit();
            }
        }

        public void WriteServerDataFile() {
            string jsonText = JsonConvert.SerializeObject(Package.Server.ServerData, Formatting.Indented);

            StreamWriter sw = new StreamWriter($"{GetApplicationPath()}ServerData.json");
            sw.Write(jsonText);
            sw.Flush();
            sw.Close();
            
            Debug.Log("Wrote Server Data file at: " + GetApplicationPath() + "ServerData.json");
        }

        public void ReadServerConfigFile() {
            string path = GetApplicationPath() + "/ServerConfig.json";

            if (!File.Exists(path)) {
                string jsonText = JsonConvert.SerializeObject(serverConfig, Formatting.Indented);

                StreamWriter sw = new StreamWriter($"{path}");
                sw.Write(jsonText);
                sw.Flush();
                sw.Close();

                Debug.Log("Server Config file did not exist. Created one.");
                return;
            }
            
            string text = File.ReadAllText($"{path}");
            serverConfig = JsonConvert.DeserializeObject<Package.ServerConfig>(text);

            Debug.Log("Read server config file.");
        }

        private string GetApplicationPath() {
            string dataPath = Application.dataPath;
            string[] slicedPath = dataPath.Split("/");
            string path = "";
            for (int i = 0; i < slicedPath.Length - 1; i++) {
                path += slicedPath[i] + "/";
            }

            return path;
        }

        #endregion
    }
}
