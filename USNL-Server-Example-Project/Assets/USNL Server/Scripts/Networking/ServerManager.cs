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
        [Space]
        [SerializeField] private int dataBufferSize = 4096;
        
        private bool isMigratingHost = false;
        private DateTime timeOfStartup;

        public Package.ServerConfig ServerConfig { get => serverConfig; set => serverConfig = value; }

        public int DataBufferSize { get => dataBufferSize; set => dataBufferSize = value; }
        public bool IsMigratingHost { get => isMigratingHost; set => isMigratingHost = value; }
        public DateTime TimeOfStartup { get => timeOfStartup; set => timeOfStartup = value; }

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

            WriteServerDataFile();

            ReadServerConfigFile();

            Package.Server.Start(serverConfig.MaxPlayers, serverConfig.ServerPort);

            TimeOfStartup = DateTime.Now;
        }

        public void StopServer() {
            Package.Server.DisconnectAllClients("Server is shutting down.");

            Package.Server.ServerData.IsServerActive = false;
            WriteServerDataFile();
            
            StartCoroutine(Package.Server.ShutdownServer());
        }

        private void OnWelcomeReceivedPacket(object _packetObject) {
            USNL.Package.WelcomeReceivedPacket _wrp = (USNL.Package.WelcomeReceivedPacket)_packetObject;

            Debug.Log($"{Package.Server.Clients[_wrp.FromClient].Tcp.socket.Client.RemoteEndPoint} connected successfully and is now Player {_wrp.FromClient}.");
            if (_wrp.FromClient != _wrp.ClientIdCheck) {
                Debug.Log($"ID: ({_wrp.FromClient}) has assumed the wrong client ID ({_wrp.ClientIdCheck}).");
            }

            USNL.CallbackEvents.CallOnClientConnectedCallbacks(_wrp.FromClient);
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
