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
        [SerializeField] private USNL.Package.ServerConfig serverConfig;
        [Tooltip("These files are normally used when client self-hosts a server.")]
        [SerializeField] private bool useServerConfigAndDataFilesInEditor;

        [Header("Server Config")]
        [Tooltip("In seconds.")]
        [SerializeField] private float timeoutTime = 6f;
        [SerializeField] private float serverInfoPacketSendInterval = 1f;

        private bool isMigratingHost = false;
        private DateTime timeOfStartup;

        private float lastServerInfoPacketSentTime;

        public USNL.Package.ServerConfig ServerConfig { get => serverConfig; set => serverConfig = value; }

        public bool IsMigratingHost { get => isMigratingHost; set => isMigratingHost = value; }
        public DateTime TimeOfStartup { get => timeOfStartup; set => timeOfStartup = value; }
        public bool ServerActive { get => USNL.Package.Server.ServerData.IsServerActive; }
        public int ClientsInWaitingLobby { get => USNL.Package.Server.WaitingLobbyClients.Count; }

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
            
            lastServerInfoPacketSentTime = Time.time;
        }

        private void Update() {
            LookForServerQuitFile();
            CheckClientsTimedout();
            CheckWaitingLobbyClientsConnected();
            ContinuouslySendServerInfoPackets();
        }

        private void OnEnable() {
            USNL.CallbackEvents.OnWelcomeReceivedPacket += OnWelcomeReceivedPacket;
            USNL.CallbackEvents.OnConnectPacket += OnConnectPacket;
            USNL.CallbackEvents.OnConnectionConfirmedPacket += OnConnectionConfirmedPacket;
        }
        
        private void OnDisable() {
            USNL.CallbackEvents.OnWelcomeReceivedPacket -= OnWelcomeReceivedPacket;
            USNL.CallbackEvents.OnConnectPacket -= OnConnectPacket;
            USNL.CallbackEvents.OnConnectionConfirmedPacket -= OnConnectionConfirmedPacket;
        }

        private void OnApplicationQuit() {
            StopServer();
        }

        #endregion

        #region Server Manager

        public void StartServer() {
            if (USNL.Package.Server.ServerData.IsServerActive) {
                Debug.LogWarning("Cannot start server. Server is already active.");
                return;
            }
            
            USNL.Package.Server.ServerData.IsServerActive = true;

            if (useServerConfigAndDataFilesInEditor && Application.isEditor) {
                WriteServerDataFile();
                ReadServerConfigFile();
            }

            USNL.Package.Server.Start(serverConfig.MaxClients, serverConfig.ServerPort);

            TimeOfStartup = DateTime.Now;
        }

        public void StopServer() {
            if (!USNL.Package.Server.ServerData.IsServerActive) {
                Debug.LogWarning("Cannot stop server. Server is not active.");
                return;
            }
            
            USNL.Package.Server.DisconnectAllClients("Server is shutting down.");

            USNL.Package.Server.ServerData.IsServerActive = false;

            if (useServerConfigAndDataFilesInEditor && Application.isEditor)
                WriteServerDataFile();
            
            StartCoroutine(USNL.Package.Server.ShutdownServer());
        }

        #endregion

        #region Packet Handling

        private void OnWelcomeReceivedPacket(object _packetObject) {
            USNL.Package.WelcomeReceivedPacket _wr = (USNL.Package.WelcomeReceivedPacket)_packetObject;

            Debug.Log($"1 {Package.Server.WaitingLobbyClients[0].Udp.endPoint != null}");
            
            USNL.Package.Client client = GetClientFromClientId(_wr.FromClient);
            
            Debug.Log($"{client.Tcp.socket.Client.RemoteEndPoint} connected successfully and is now Waiting Lobby Client {_wr.FromClient - 1000000}.");

            if (_wr.FromClient != _wr.LobbyClientIdCheck) {
                Debug.Log($"ID: ({_wr.FromClient}) has assumed the wrong lobby client ID ({_wr.LobbyClientIdCheck}).");
            }

            Debug.Log($"2 {client.Udp.endPoint != null}");
            
            client = GetClientFromClientId(_wr.FromClient);

            Debug.Log($"3 {client.Udp.endPoint != null}");
        }

        private void OnConnectPacket(object _packetObject) {
            USNL.Package.ConnectPacket _cp = (USNL.Package.ConnectPacket)_packetObject;

            Debug.Log($"4 {USNL.Package.Server.WaitingLobbyClients[_cp.FromClient - 1000000].Udp.endPoint != null}");

            int _clientId = -1;
            for (int i = 0; i < USNL.Package.Server.Clients.Count; i++) {
                if (!USNL.Package.Server.Clients[i].IsConnected) {
                    _clientId = i;
                    break;
                }
            }

            if (_clientId == -1) {
                Debug.Log($"Cannot connect Waiting Lobby Client {_cp.FromClient - 1000000}. Server is full.");
                return;
            }

            USNL.Package.Server.Clients[_clientId] = USNL.Package.Server.WaitingLobbyClients[_cp.FromClient - 1000000];
            USNL.Package.Server.Clients[_clientId].ClientId = _clientId;
            RemoveWaitingLobbyClient(_cp.FromClient - 1000000);

            USNL.Package.PacketSend.ConnectReceived(_clientId, _clientId, serverConfig.WelcomeMessage);

            Debug.Log($"5 {USNL.Package.Server.WaitingLobbyClients[_cp.FromClient - 1000000].Udp.endPoint != null}");
        }

        private void OnConnectionConfirmedPacket(object _packetObject) {
            USNL.Package.ConnectionConfirmedPacket _crp = (USNL.Package.ConnectionConfirmedPacket)_packetObject;
            
            Debug.Log($"{USNL.Package.Server.Clients[_crp.FromClient].Tcp.socket.Client.RemoteEndPoint} connected successfully and is now Client {_crp.FromClient}.");
            if (_crp.FromClient != _crp.ClientIdCheck) {
                Debug.Log($"ID: ({_crp.FromClient}) has assumed the wrong client ID ({_crp.ClientIdCheck}).");
            }

            USNL.CallbackEvents.CallOnClientConnectedCallbacks(_crp.FromClient);
        }

        public void ClientDisconnected(int _clientId) {
            USNL.CallbackEvents.CallOnClientDisconnectedCallbacks(_clientId);
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

            for (int i = 0; i < USNL.Package.Server.WaitingLobbyClients.Count; i++) {
                if (!USNL.Package.Server.WaitingLobbyClients[i].IsConnected) continue;
                if (-USNL.Package.Server.WaitingLobbyClients[i].Tcp.LastPacketTime.Subtract(DateTime.Now).TotalSeconds < timeoutTime) continue;

                Debug.Log($"Waiting Lobby Client {i} has timed out.");
                USNL.Package.Server.WaitingLobbyClients[i].Disconnect();
            }
        }

        private void CheckWaitingLobbyClientsConnected() {
            int clientsRemoved = 0;
            for (int i = 0; i < USNL.Package.Server.WaitingLobbyClients.Count; i++) {
                if (USNL.Package.Server.WaitingLobbyClients[i].IsConnected) continue;
                if (-USNL.Package.Server.WaitingLobbyClients[i].Tcp.LastPacketTime.Subtract(DateTime.Now).TotalSeconds < 2f) continue;

                RemoveWaitingLobbyClient(i - clientsRemoved);
                clientsRemoved++;

                Debug.Log($"Removed waiting lobby client of Id {i}.");
            }
        }

        private void RemoveWaitingLobbyClient(int _index) {
            USNL.Package.Server.WaitingLobbyClients.RemoveAt(_index);

            // Decrement waiting lobby clients with IDs greater than this one
            for (int j = _index; j < USNL.Package.Server.WaitingLobbyClients.Count; j++) {
                USNL.Package.Server.WaitingLobbyClients[j].ClientId--;
            }
        }

        private void SendServerInfoPacketToAllClients() {
            for (int i = 0; i < USNL.Package.Server.Clients.Count; i++) {
                if (!USNL.Package.Server.Clients[i].IsConnected) continue;

                USNL.Package.PacketSend.ServerInfo(USNL.Package.Server.Clients[i].ClientId, ServerConfig.ServerName, USNL.ServerManager.GetConnectedClientsIds(), ServerConfig.MaxClients, USNL.ServerManager.GetNumberOfConnectedClients() > ServerConfig.MaxClients);
            }

            for (int i = 0; i < USNL.Package.Server.WaitingLobbyClients.Count; i++) {
                if (!USNL.Package.Server.WaitingLobbyClients[i].IsConnected) continue;

                USNL.Package.PacketSend.ServerInfo(USNL.Package.Server.WaitingLobbyClients[i].ClientId, ServerConfig.ServerName, USNL.ServerManager.GetConnectedClientsIds(), ServerConfig.MaxClients, USNL.ServerManager.GetNumberOfConnectedClients() > ServerConfig.MaxClients);
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

        public static USNL.Package.Client GetClientFromClientId(int _clientId) {
            USNL.Package.Client client = null;
            if (_clientId >= 1000000) client = USNL.Package.Server.WaitingLobbyClients[_clientId % 1000000];
            else client = USNL.Package.Server.Clients[_clientId];
            return client;
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
            string jsonText = JsonConvert.SerializeObject(USNL.Package.Server.ServerData, Formatting.Indented);

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
            serverConfig = JsonConvert.DeserializeObject<USNL.Package.ServerConfig>(text);

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
