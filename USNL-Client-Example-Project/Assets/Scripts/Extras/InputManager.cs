using System;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {
    List<int> keycodesDown = new List<int>();
    List<int> keycodesUp = new List<int>();

    private void Update() {
        SendKeycodes();
    }

    private void OnGUI() {
        if (Event.current.isKey | Event.current.isMouse) {
            if (Event.current.type == EventType.KeyDown) {
                keycodesDown.Add((int)Event.current.keyCode);
            } else if (Event.current.type == EventType.KeyUp) {
                keycodesUp.Add((int)Event.current.keyCode);
            } else if (Event.current.type == EventType.MouseDown) {
                // Convert Mouse Button Code to Key Code
                // <6 because KeyCode only supports 6 Mouse Buttons while Mouse Button Codes supports 8, add 323 to convert to KeyCode
                if ((int)Event.current.button < 6) {
                    keycodesDown.Add((int)Event.current.button + 323);
                }
            } else if (Event.current.type == EventType.MouseUp) {
                // Convert Mouse Button Code to Key Code
                // <6 because KeyCode only supports 6 Mouse Buttons while Mouse Button Codes supports 8, add 323 to convert to KeyCode
                if ((int)Event.current.button < 6) {
                    keycodesUp.Add((int)Event.current.button + 323);
                }
            }
        }
    }

    private void SendKeycodes() {
        if (Client.instance.IsConnected) {
            // Declare arrays of the length of the keycodes list * 4 because 4 bytes are needed for 1 int
            byte[] keycodesDownBytes = new byte[keycodesDown.Count * 4];
            byte[] keycodesUpBytes = new byte[keycodesUp.Count * 4];

            // Convert keycodesDown and Up to byte list
            for (int i = 0; i < keycodesDown.Count; i++) {
                byte[] values = BitConverter.GetBytes(keycodesDown[i]);
                keycodesDownBytes[i] = values[0];
                keycodesDownBytes[i + 1] = values[1];
                keycodesDownBytes[i + 2] = values[2];
                keycodesDownBytes[i + 3] = values[3];
            }
            for (int i = 0; i < keycodesUp.Count; i++) {
                byte[] values = BitConverter.GetBytes(keycodesUp[i]);
                keycodesUpBytes[i] = values[0];
                keycodesUpBytes[i + 1] = values[1];
                keycodesUpBytes[i + 2] = values[2];
                keycodesUpBytes[i + 3] = values[3];
            }

            // Clear lists to allow for new values and no duplicates next time this function is run
            keycodesDown.Clear();
            keycodesUp.Clear();

            PacketSend.ClientInput(keycodesDownBytes, keycodesUpBytes);
        }
    }
}
