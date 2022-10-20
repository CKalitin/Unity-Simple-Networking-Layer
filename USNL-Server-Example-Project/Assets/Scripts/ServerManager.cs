using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerManager : MonoBehaviour {
    #region Variables
    
    public static ServerManager instance;

    [SerializeField] private int maxPlayers = 20;
    [SerializeField] private int port = 26950;
    [SerializeField] private string welcomeMessage = "Holy fuck my code worked! :O :)";
    [Space]
    [SerializeField] private int dataBufferSize = 4096;

    public int MaxPlayers { get => maxPlayers; set => maxPlayers = value; }
    public int Port { get => port; set => port = value; }
    public string WelcomeMessage { get => welcomeMessage; set => welcomeMessage = value; }
    public int DataBufferSize { get => dataBufferSize; set => dataBufferSize = value; }

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

        Server.Start(maxPlayers, port);
    }

    private void OnApplicationQuit() {
        Server.Stop();
    }

    #endregion

    #region Server Manager

    private void OnWelcomeReceivedPacket(WelcomeReceivedPacket _wrp) {
        Debug.Log($"{Server.clients[_wrp.FromClient].Tcp.socket.Client.RemoteEndPoint} connected successfully and is now Player {_wrp.FromClient}.");
        if (_wrp.FromClient != _wrp.FromClient) {
            Debug.Log($"ID: {_wrp.FromClient}) has assumed the wrong client ID ({_wrp.FromClient}.");
        }
    }

    #endregion
}
