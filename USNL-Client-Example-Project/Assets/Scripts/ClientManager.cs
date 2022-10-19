using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientManager : MonoBehaviour {
    #region Variables

    public static ClientManager instance;

    [SerializeField] private string ip = "127.0.0.1";
    [SerializeField] private int port = 26950;

    #endregion

    #region Core

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Debug.Log("Client Manager instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Start() {
        ConnectToServer();
    }

    private void ConnectToServer() {
        Client.instance.SetIP(ip, port);
        Client.instance.ConnectToServer();
    }

    private void DisconnectFromServer() {
        Client.instance.Disconnect();
    }

    #endregion
}
