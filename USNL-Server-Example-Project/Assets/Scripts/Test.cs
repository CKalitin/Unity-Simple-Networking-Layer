using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
    public void OnWelcomeReceivedPacket(/*WelcomeReceivedPacket _packet*/) {
        Debug.Log("It worked!");
        int realcode = 111;
        realcode += 121112;
    }

    public void OnClientInputPacket(ClientInputPacket _packet) {
        Debug.Log("Private worked!");
    }
}
