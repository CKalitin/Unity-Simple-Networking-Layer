using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
    public void OnWelcomePacket(WelcomePacket _packet) {
        Debug.Log("It worked!");
    }

    private void OnFactorioIsFunPacket(FactorioIsFunPacket _packet) {
        Debug.Log("Private worked!");
    }
}
