using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerExample : MonoBehaviour {
    private void Start() {
        ServerManager.instance.StartServer();
    }
}
