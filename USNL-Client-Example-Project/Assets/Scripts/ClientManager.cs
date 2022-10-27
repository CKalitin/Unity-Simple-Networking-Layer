using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientManager : MonoBehaviour {
    #region Variables

    public static ClientManager instance;

    [SerializeField] private string ip = "127.0.0.1";
    [SerializeField] private int port = 26950;

    public bool HandleData = true;
    public bool PacketHandlers = true;
    public bool PacketManager = true;

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
        ConnectToServer();
    }

    private void OnEnable() {
        USNLCallbackEvents.OnWelcomePacket += OnWelcomePacket;
    }

    private void OnDisable() {
        USNLCallbackEvents.OnWelcomePacket -= OnWelcomePacket;
    }

    private void OnWelcomePacket(object _packetObject) {
        WelcomePacket _wp = (WelcomePacket)_packetObject;

        Debug.Log($"Welcome message from Server: {_wp.WelcomeMessage}, Client Id: {_wp.ClientId}");
        Client.instance.ClientId = _wp.ClientId;

        PacketSend.WelcomeReceived(_wp.ClientId);

        USNLCallbackEvents.CallOnConnectedCallbacks(0);
    }

    #endregion

    #region Functions

    private void ConnectToServer() {
        Client.instance.SetIP(ip, port);
        Client.instance.ConnectToServer();
    }

    private void DisconnectFromServer() {
        Client.instance.Disconnect();
    }

    public void DisconnectedFromServer() {
        USNLCallbackEvents.CallOnDisconnectedCallbacks(0);
    }

    #endregion
}
