using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
    public void OnWelcomeReceivedPacket(WelcomeReceivedPacket _packet) {
        Debug.Log("It worked!");
    }
}
