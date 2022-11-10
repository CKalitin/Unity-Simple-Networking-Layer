using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientManager : MonoBehaviour {
    #region Variables

    public static ClientManager instance;

    [Header("Connection Info")]
    [SerializeField] private string ip = "127.0.0.1";
    [SerializeField] private int port = 26950;
    [Space]
    [Tooltip("If connection is not established after x milliseconds, stop attemping connection.")]
    [SerializeField] private float connectionTimeout;

    private bool isConnecting = false;
    private bool isMigratingHost = false;
    private bool isBecomingHost = false;

    public bool IsConnected { get => Client.instance.IsConnected; }
    public bool IsAttempingConnection { get => isConnecting; }
    public bool IsHost { get => Client.instance.IsHost; }
    public bool IsMigratingHost { get => isMigratingHost; }
    public bool IsBecomingHost { get => isBecomingHost; }

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

    private void OnEnable() { USNLCallbackEvents.OnWelcomePacket += OnWelcomePacket; }
    private void OnDisable() { USNLCallbackEvents.OnWelcomePacket -= OnWelcomePacket; }

    #endregion

    #region Functions

    public void ConnectToServer() {
        Client.instance.SetIP(ip, port);
        Client.instance.ConnectToServer();

        StartCoroutine(AttemptingConnection());
    }

    public void ConnectToServer(string _ip, int _port) {
        ip = _ip;
        port = _port;
        Client.instance.SetIP(ip, port);
        Client.instance.ConnectToServer();

        StartCoroutine(AttemptingConnection());
    }

    public void SetIPAndPort(string _ip, int _port) {
        ip = _ip;
        port = _port;
    }

    public void DisconnectFromServer() {
        Client.instance.Disconnect();
    }

    private void OnWelcomePacket(object _packetObject) {
        WelcomePacket _wp = (WelcomePacket)_packetObject;

        Debug.Log($"Welcome message from Server: {_wp.WelcomeMessage}, Client Id: {_wp.ClientId}");
        Client.instance.ClientId = _wp.ClientId;

        PacketSend.WelcomeReceived(_wp.ClientId);

        USNLCallbackEvents.CallOnConnectedCallbacks(0);
    }

    public void SetDisconnectedFromServer() {
        USNLCallbackEvents.CallOnDisconnectedCallbacks(0);
    }

    private IEnumerator AttemptingConnection() {
        isConnecting = true;
        float timer = 0f;
        while (timer < connectionTimeout) {
            yield return new WaitForEndOfFrame();
            if (Client.instance.IsConnected)
                break;
            timer += Time.unscaledDeltaTime;
        }
        isConnecting = false;
    }

    #endregion
}
