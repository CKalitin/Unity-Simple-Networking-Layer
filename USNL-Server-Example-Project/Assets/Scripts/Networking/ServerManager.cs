using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ServerManager : MonoBehaviour {
    #region Variables
    
    public static ServerManager instance;

    [Header("Editor Config")]
    [SerializeField] private int port = 26950;
    [SerializeField] private int maxPlayers = 20;
    [SerializeField] private string serverName = "Server";
    [SerializeField] private string welcomeMessage = "Holy fuck my code worked! :O :)";
    [Space]
    [SerializeField] private int dataBufferSize = 4096;

    private bool isServerActive = true;
    private bool isMigratingHost = false;
    private DateTime timeOfStartup;

    public int MaxPlayers { get => maxPlayers; set => maxPlayers = value; }
    public int Port { get => port; set => port = value; }
    public string WelcomeMessage { get => welcomeMessage; set => welcomeMessage = value; }
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

    private void OnEnable() { USNLCallbackEvents.OnWelcomeReceivedPacket += OnWelcomeReceivedPacket; }
    private void OnDisable() { USNLCallbackEvents.OnWelcomeReceivedPacket -= OnWelcomeReceivedPacket; }

    private void OnApplicationQuit() {
        StopServer();
    }

    #endregion

    #region Server Manager

    public void StartServer() {
        isServerActive = true;
        
        WriteServerDataFile();
        
        ReadServerConfigFile();

        Server.Start(maxPlayers, port);
        
        TimeOfStartup = DateTime.Now;
    }

    public void StopServer() {
        Server.DisconnectAllClients();

        isServerActive = false;
        WriteServerDataFile();
        Server.Stop();
    }
    
    private void OnWelcomeReceivedPacket(object _packetObject) {
        WelcomeReceivedPacket _wrp = (WelcomeReceivedPacket)_packetObject;

        Debug.Log($"{Server.Clients[_wrp.FromClient].Tcp.socket.Client.RemoteEndPoint} connected successfully and is now Player {_wrp.FromClient}.");
        if (_wrp.FromClient != _wrp.ClientIdCheck) {
            Debug.Log($"ID: ({_wrp.FromClient}) has assumed the wrong client ID ({_wrp.ClientIdCheck}).");
        }

        USNLCallbackEvents.CallOnClientConnectedCallbacks(_wrp.FromClient);
    }

    public void ClientDisconnected(int _clientId) {
        USNLCallbackEvents.CallOnClientDisconnectedCallbacks(_clientId);
    }
    
    #endregion

    #region Server Config and Data

    private void LookForServerQuitFile() {
        if (File.Exists(GetApplicationPath() + "ServerQuit")) {
            Debug.Log("ServerQuit commanded from host client, shutting down server.");
            File.Delete(GetApplicationPath() + "ServerQuit");
            StopServer();
            Application.Quit();
        }
    }

    public void WriteServerDataFile() {
        string text = "{" +
            $"\n    \"serverActive\":{isServerActive}" +
            "\n}";
        
        StreamWriter sw = new StreamWriter($"{GetApplicationPath()}ServerData.json");
        sw.Write(text);
        sw.Flush();
        sw.Close();

        Debug.Log("Wrote Server Data file at: " + GetApplicationPath() + "ServerData.json");
    }

    public void ReadServerConfigFile() {
        string path = GetApplicationPath() + "/ServerConfig.json";

        if (!File.Exists(path)) {
            string serverConfigFileText = "{" +
            $"\n    \"serverPort\":{port}" +
            $"\n    \"maxPlayers\":{maxPlayers}" +
            $"\n    \"serverName\":{serverName}" +
            $"\n    \"welcomeMessage\":{welcomeMessage}" +
            "\n}";

            StreamWriter sw = new StreamWriter($"{path}");
            sw.Write(serverConfigFileText);
            sw.Flush();
            sw.Close();
            
            Debug.Log("Server Config file did not exist. Created one.");
            return;
        }

        string[] text = File.ReadAllLines($"{path}");
                for (int i = 0; i < text.Length; i++) {
            if (text[i].Contains("serverPort")) {
                string[] split = text[i].Split(':');
                string value = split[1].Replace(",", "").Replace(" ", "");
                int newPort = int.Parse(value);
                if (newPort != 0) port = newPort;
            } else if (text[i].Contains("maxPlayers")) {
                string[] split = text[i].Split(':');
                string value = split[1].Replace(",", "").Replace(" ", "");
                int newMaxPlayers = int.Parse(value);
            } else if (text[i].Contains("serverName")) {
                string[] split = text[i].Split(':');
                string value = split[1].Replace(",", "").Replace(" ", "");
                string newServerName = value;
                if (newServerName != "") serverName = newServerName;
            } else if (text[i].Contains("weclomeMessage")) {
                string[] split = text[i].Split(':');
                string value = split[1].Replace(",", "").Replace(" ", "");
                string newWelcomeMessage = value;
                if (newWelcomeMessage != "") welcomeMessage = newWelcomeMessage;
            }
        }
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
