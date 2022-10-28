using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
    ClientInput clientInput;

    private void OnEnable() {
        USNLCallbackEvents.OnServerStarted += OnServerStarted;
    }

    private void OnDisable() {
        USNLCallbackEvents.OnServerStarted -= OnServerStarted;
    }

    private void OnServerStarted(object _param) {
        clientInput = InputManager.instance.ClientInputs[0];
    }

    void Update() {
        if (InputManager.instance.ClientInputs[0].GetKeyDown(KeyCode.W)) {
            Debug.Log("W Down");
        }
        if (InputManager.instance.ClientInputs[0].GetKeyUp(KeyCode.W)) {
            Debug.Log("W Up");
        }
        if (InputManager.instance.ClientInputs[0].GetKey(KeyCode.W)) {
            Debug.Log("W Pressed");
        }

        if (clientInput.GetMouseButtonDown("Mouse0")) {
            Debug.Log("Left Click Down");
        }
        if (clientInput.GetMouseButtonUp("Mouse0")) {
            Debug.Log("Left Click Up");
        }
        if (clientInput.GetMouseButton("Mouse0")) {
            Debug.Log("Left Click Pressed");
        }
    }
}
