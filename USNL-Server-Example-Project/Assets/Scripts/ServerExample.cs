using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerExample : MonoBehaviour {
    public bool runServer;

    private void Update() {
        if (runServer && !USNL.ServerManager.instance.ServerActive) {
            USNL.ServerManager.instance.StartServer();
        }
        
        if (!runServer && USNL.ServerManager.instance.ServerActive) {
            USNL.ServerManager.instance.StopServer();
        }
    }
}
