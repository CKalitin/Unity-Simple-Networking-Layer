using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace USNL {
    public enum IPType {
        WAN,
        LAN,
        Localhost,
        Unknown
    }

    [Serializable]
    public struct ServerInfo {
        public string ServerName;
        public int[] ConnectedClientIds;
        public int MaxClients;
        public bool ServerFull;
    }

    public class ClientManager : MonoBehaviour {
        #region Variables

        public static ClientManager instance;

        [Header("Connection Info")]
        [SerializeField] private int serverID;
        [SerializeField] private int port = 26950;
        [Space]
        [Tooltip("If connection is not established after x seconds, stop attemping connection.")]
        [SerializeField] private float connectionTimeout = 10f;
        [Tooltip("If this is too low the server will see 2 attempted connections and the client will not received UDP data, who knows why.")]
        [SerializeField] private float timeBetweenConnectionAttempts = 3f;
        [Space]
        [Tooltip("In seconds.")]
        [SerializeField] private float timeoutTime = 5f;

        private int wanClientId;
        private int lanClientId;
        private string wanClientIp;
        private string lanClientIp;

        private bool attemptConnection = true;

        private bool isAttempingConnection = false;
        private bool isMigratingHost = false;
        private bool isBecomingHost = false;

        private bool inLobby = true;

        private DateTime timeOfConnection;

        private string serverName;

        [Header("Server Host")]
        [SerializeField] private string serverExeName = "USNL-Server-Example-Project.exe";
        [Tooltip("Be sure to add '/' at the end")]
        [SerializeField] private string serverPath = "Server";
        [Tooltip("Be sure to add '/' at the end.\nThis is not affected by useApplicationDataPath.")]
        [SerializeField] private string editorServerPath = "Server";
        [Tooltip("When the project is built this tick adds the path to the game files before the server path.\nIf server files are in a child folder of the game files, tick this.")]
        [SerializeField] private bool useApplicationPath = false;
        [SerializeField] private USNL.Package.ServerConfig serverConfig;

        [Header("Server Info")]
        [SerializeField] private ServerInfo serverInfo;

        public int WanClientId { get => wanClientId; set => wanClientId = value; }
        public int LanClientId { get => lanClientId; set => lanClientId = value; }
        public string WanClientIp { get => wanClientIp; set => wanClientIp = value; }
        public string LanClientIP { get => lanClientIp; set => lanClientIp = value; }
        
        public bool IsConnected { get => USNL.Package.Client.instance.IsConnected; }
        public bool IsAttempingConnection { get => isAttempingConnection; }
        public bool InLobby { get => inLobby; set => inLobby = value; }
        public bool IsHost { get => USNL.Package.Client.instance.IsHost; }
        public bool IsMigratingHost { get => isMigratingHost; }
        public bool IsBecomingHost { get => isBecomingHost; }
        public DateTime TimeOfConnection { get => timeOfConnection; set => timeOfConnection = value; }

        public string ServerExeName { get => serverExeName; set => serverExeName = value; }
        public string ServerPath { get => serverPath; set => serverPath = value; }
        public string EditorServerPath { get => editorServerPath; set => editorServerPath = value; }
        public bool UseApplicationPath { get => useApplicationPath; set => useApplicationPath = value; }

        public USNL.Package.ServerConfig ServerConfig { get => serverConfig; set => serverConfig = value; }
        public USNL.Package.ServerData ServerData { get => USNL.Package.ServerHost.GetServerData(); }

        public bool IsServerRunning { get => USNL.Package.ServerHost.IsServerRunning(); }
        public string ServerName { get => serverName; set => serverName = value; }
        public ServerInfo ServerInfo { get => serverInfo; set => serverInfo = value; }

        public bool IsServerActive() { USNL.Package.ServerHost.ReadServerDataFile(); return USNL.Package.ServerHost.GetServerData().IsServerActive; }

        #endregion

        #region Core

        private void Awake() {
            if (instance == null) {
                instance = this;
            } else if (instance != this) {
                Debug.Log("Client Manager instance already exists, destroying object!");
                Destroy(this);
            }

            if (Application.isEditor) {
                Application.runInBackground = true;
            }
        }

        private void Start() {
            SetClientId();
        }

        private void Update() {
            CheckServerLaunchSuccessful();
            if (USNL.Package.ServerHost.GetServerData().IsServerActive == false && !USNL.Package.ServerHost.LaunchingServer) USNL.Package.Client.instance.IsHost = false;

            if (-USNL.Package.Client.instance.Tcp.LastPacketTime.Subtract(DateTime.Now).TotalSeconds >= timeoutTime & USNL.Package.Client.instance.IsConnected) {
                Debug.Log("Timed out.");
                DisconnectFromServer();
            }
        }

        private void OnEnable() {
            USNL.CallbackEvents.OnWelcomePacket += OnWelcomePacket;
            USNL.CallbackEvents.OnConnectReceivedPacket += OnConnectReceivedPacket;
            USNL.CallbackEvents.OnDisconnectClientPacket += OnDisconnectClientPacket;
            USNL.CallbackEvents.OnServerInfoPacket += OnServerInfoPacket;
        }

        private void OnDisable() {
            USNL.CallbackEvents.OnWelcomePacket -= OnWelcomePacket;
            USNL.CallbackEvents.OnConnectReceivedPacket -= OnConnectReceivedPacket;
            USNL.CallbackEvents.OnDisconnectClientPacket -= OnDisconnectClientPacket;
            USNL.CallbackEvents.OnServerInfoPacket -= OnServerInfoPacket;
        }

        private void OnApplicationQuit() {
            CloseServer();
        }

        #endregion

        #region Client to Server Functions

        public void ConnectToServerLobby() {
            USNL.Package.Client.instance.SetIP(serverID, port);
            StartCoroutine(AttemptingConnection());
        }

        public void ConnectToServerLobby(int _id, int _port) {
            serverID = _id;
            port = _port;

            USNL.Package.Client.instance.SetIP(serverID, port);
            StartCoroutine(AttemptingConnection());
        }

        public void DisconnectFromServer() {
            USNL.Package.Client.instance.Disconnect();
        }

        public void StopAttemptingConnection() {
            attemptConnection = false;
        }

        private IEnumerator AttemptingConnection() {
            isAttempingConnection = true;
            attemptConnection = true;

            int connectionsAttempted = 0;

            float timer = 0.00001f;
            while (timer < connectionTimeout && attemptConnection) {
                yield return new WaitForEndOfFrame();

                if (USNL.Package.Client.instance.IsConnected)
                    break;

                if (timer > connectionsAttempted * timeBetweenConnectionAttempts) {
                    USNL.Package.Client.instance.ConnectToServer();
                    connectionsAttempted++;
                }

                timer += Time.unscaledDeltaTime;
            }

            isAttempingConnection = false;
        }

        // Exit lobby
        public void FullyConnectToServer() {
            if (!USNL.Package.Client.instance.IsConnected) {
                Debug.Log("Cannot fully connect to server, not connected to server.");
                return;
            }
            
            USNL.Package.PacketSend.Connect(0);
        }

        #endregion

        #region IP and ID Functions

        private void SetClientId() {
            wanClientIp = GetWanIP();
            lanClientIp = GetLanIP();

            wanClientId = USNL.Package.Client.instance.IPToID(wanClientIp);
            lanClientId = USNL.Package.Client.instance.IPToID(lanClientIp);
        }

        private string GetWanIP() {
            try {
                string url = "http://checkip.dyndns.org";
                System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                System.Net.WebResponse resp = req.GetResponse();
                System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                string response = sr.ReadToEnd().Trim();
                string[] a = response.Split(':');
                string a2 = a[1].Substring(1);
                string[] a3 = a2.Split('<');
                string a4 = a3[0];
                return a4;
            } catch {
                return GetWanIP();
                // Kinda jank but it should work
            }
        }

        // https://stackoverflow.com/questions/6803073/get-local-ip-address
        private string GetLanIP() {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList) {
                if (ip.AddressFamily == AddressFamily.InterNetwork) {
                    return ip.ToString();
                }
            }
            Debug.LogWarning("Failed to Get LAN IP. No network adapters with an IPv4 address in the system.");
            return "";
        }

        public IPType GetIPType(string _ip) {
            //if (_ip == wanClientIp) return IPType.WAN; TODO - If server can be pinged, it is valid
            if (_ip.Substring(0, 5) == lanClientIp.Substring(0, 5)) return IPType.LAN;
            else if (_ip == lanClientIp) return IPType.Localhost;
            else return IPType.Unknown;
        }

        #endregion

        #region Server Host Functions

        public void LaunchServer() {
            USNL.Package.ServerHost.LaunchServer();
        }

        public void CloseServer() {
            USNL.Package.ServerHost.CloseServer();
        }

        private void CheckServerLaunchSuccessful() {
            if (USNL.Package.ServerHost.LaunchingServer && ServerData.IsServerActive) {
                USNL.Package.ServerHost.LaunchingServer = false;
                Debug.Log("Successfuly launched server. Server is Active.");
            }
        }

        #endregion

        #region Packet Handlers

        private void OnWelcomePacket(object _packetObject) {
            USNL.Package.WelcomePacket _wp = (USNL.Package.WelcomePacket)_packetObject;
            
            Debug.Log($"Connected to server lobby.\nWelcome Message: {_wp.WelcomeMessage}");

            USNL.Package.Client.instance.ClientId = _wp.LobbyClientId;

            InLobby = true;

            USNL.Package.PacketSend.WelcomeReceived(_wp.LobbyClientId);
        }

        private void OnConnectReceivedPacket(object _packetObject) {
            USNL.Package.ConnectReceivedPacket _cr = (USNL.Package.ConnectReceivedPacket)_packetObject;

            if (_cr.ClientId < 0) {
                Debug.Log("Could not connect to server. Server full.");
                return;
            }

            Debug.Log($"Connection message from Server: {_cr.ConnectMessage}, Client Id: {_cr.ClientId}");
            USNL.Package.Client.instance.ClientId = _cr.ClientId;

            timeOfConnection = DateTime.Now;
            
            inLobby = false;

            USNL.Package.PacketSend.ConnectionConfirmed(_cr.ClientId);

            USNL.CallbackEvents.CallOnConnectedCallbacks(0);
        }

        private void OnDisconnectClientPacket(object _packetObject) {
            USNL.Package.DisconnectClientPacket _dcp = (USNL.Package.DisconnectClientPacket)_packetObject;

            Debug.Log($"Disconnection commanded from server.\nMessage: {_dcp.DisconnectMessage}");

            USNL.Package.Client.instance.Disconnect();
        }

        private void OnServerInfoPacket(object _packetObject) {
            USNL.Package.ServerInfoPacket _si = (USNL.Package.ServerInfoPacket)_packetObject;
            serverInfo.ServerName = _si.ServerName;
            serverInfo.ConnectedClientIds = _si.ConnectedClientsIds;
            serverInfo.MaxClients = _si.MaxClients;
            serverInfo.ServerFull = _si.ServerFull;
        }

        #endregion
    }
}