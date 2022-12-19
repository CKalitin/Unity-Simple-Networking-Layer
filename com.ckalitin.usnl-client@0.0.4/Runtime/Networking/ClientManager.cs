using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class ClientManager : MonoBehaviour {
    #region Variables

    public static ClientManager instance;

    [Header("Connection Info")]
    [SerializeField] private int serverID;
    [SerializeField] private int port = 26950;
    [Space]
    [Tooltip("If connection is not established after x seconds, stop attemping connection.")]
    [SerializeField] private float connectionTimeout = 5f;
    [Tooltip("If this is too low the server will see 2 attempted connections and the client will not received UDP data, who knows why.")]
    [SerializeField] private float timeBetweenConnectionAttempts = 3f;

    private int wanClientId;
    private int lanClientId;
    private string clientIP = "";

    private bool attemptConnection = true;

    private bool isAttempingConnection = false;
    private bool isMigratingHost = false;
    private bool isBecomingHost = false;

    private DateTime timeOfConnection;

    [Header("Server Host")]
    [SerializeField] private string serverExeName = "USNL-Server-Example-Project.exe";
    [Tooltip("Be sure to add '/' at the end")]
    [SerializeField] private string serverPath = "Server";
    [Tooltip("Be sure to add '/' at the end.\nThis is not affected by useApplicationDataPath.")]
    [SerializeField] private string editorServerPath = "Server";
    [Tooltip("If when the project is built the server is a part of the game files, tick this and use the local path to the server folder.")]
    [SerializeField] private bool useApplicationPath = false;
    [SerializeField] private ServerConfig serverConfig;

    public int WanClientId { get => wanClientId; set => wanClientId = value; }
    public int LanClientId { get => lanClientId; set => lanClientId = value; }
    public string ClientIP { get => clientIP; set => clientIP = value; }
    
    // Debug Menu Variables
    public bool IsConnected { get => Client.instance.IsConnected; }
    public bool IsAttempingConnection { get => isAttempingConnection; }
    public bool IsHost { get => Client.instance.IsHost; }
    public bool IsMigratingHost { get => isMigratingHost; }
    public bool IsBecomingHost { get => isBecomingHost; }
    public DateTime TimeOfConnection { get => timeOfConnection; set => timeOfConnection = value; }

    public string ServerExeName { get => serverExeName; set => serverExeName = value; }
    public string ServerPath { get => serverPath; set => serverPath = value; }
    public string EditorServerPath { get => editorServerPath; set => editorServerPath = value; }
    public bool UseApplicationPath { get => useApplicationPath; set => useApplicationPath = value; }
    
    public ServerConfig ServerConfig { get => serverConfig; set => serverConfig = value; }
    public ServerData ServerData { get => ServerHost.GetServerData();  }
    
    public bool IsServerRunning { get => ServerHost.IsServerRunning(); }
    

    public bool IsServerActive() { ServerHost.ReadServerDataFile(); return ServerHost.GetServerData().IsServerActive; }

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
        if (ServerHost.GetServerData().IsServerActive == false && !ServerHost.LaunchingServer) Client.instance.IsHost = false;
    }

    private void OnEnable() { USNLCallbackEvents.OnWelcomePacket += OnWelcomePacket; }
    private void OnDisable() { USNLCallbackEvents.OnWelcomePacket -= OnWelcomePacket; }

    private void OnApplicationQuit() {
        CloseServer();
    }

    #endregion

    #region Client to Server Functions

    public void ConnectToServer() {
        Client.instance.SetIP(serverID, port);
        StartCoroutine(AttemptingConnection());
    }

    /// <summary> Attempts to connect to the server with ip and port provided. </summary>
    /// <param name="_id">IP Address</param>
    /// <param name="_port">Port</param>
    public void ConnectToServer(int _id, int _port) {
        serverID = _id;
        port = _port;

        Client.instance.SetIP(serverID, port);
        StartCoroutine(AttemptingConnection());
    }
    
    public void DisconnectFromServer() {
        Client.instance.Disconnect();
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
            
            if (Client.instance.IsConnected)
                break;

            if (timer > connectionsAttempted * timeBetweenConnectionAttempts) {
                Client.instance.ConnectToServer();
                connectionsAttempted++;
            }

            timer += Time.unscaledDeltaTime;
        }
        
        isAttempingConnection = false;
    }

    private void SetClientId() {
        wanClientId = Client.instance.IPToID(GetWanIP());
        lanClientId = Client.instance.IPToID(GetLanIP());
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
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }

    #endregion

    #region Server Host Functions

    public void LaunchServer() {
        ServerHost.LaunchServer();
    }
    
    public void CloseServer() {
        ServerHost.CloseServer();
    }

    private void CheckServerLaunchSuccessful() {
        if (ServerHost.LaunchingServer && ServerData.IsServerActive) {
            ServerHost.LaunchingServer = false;
            Debug.Log("Successfuly launched server. Server is Active.");
        }
    }

    #endregion

    #region Management Functions

    private void OnWelcomePacket(object _packetObject) {
        WelcomePacket _wp = (WelcomePacket)_packetObject;

        Debug.Log($"Welcome message from Server: {_wp.WelcomeMessage}, Client Id: {_wp.ClientId}");
        Client.instance.ClientId = _wp.ClientId;

        timeOfConnection = DateTime.Now;

        PacketSend.WelcomeReceived(_wp.ClientId);

        USNLCallbackEvents.CallOnConnectedCallbacks(0);
    }

    public void SetDisconnectedFromServer() {
        USNLCallbackEvents.CallOnDisconnectedCallbacks(0);
    }
    
    #endregion
}
