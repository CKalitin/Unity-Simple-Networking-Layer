using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerExample : MonoBehaviour {
    private void Start() {
        USNL.ServerManager.instance.StartServer();
    }
}
