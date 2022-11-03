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

    private void OnEnable() {
        USNLCallbackEvents.OnWelcomeReceivedPacket += OnWelcomeReceivedPacket;
    }

    private void OnDisable() {
        USNLCallbackEvents.OnWelcomeReceivedPacket -= OnWelcomeReceivedPacket;
    }

    #endregion

    #region Server Manager

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
}
