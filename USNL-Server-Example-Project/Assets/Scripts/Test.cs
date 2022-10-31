using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
    private void OnEnable() {
        USNLCallbackEvents.OnClientConnected += OnConnected;
        USNLCallbackEvents.OnVariablesTestPacket += OnVariablesTestPacket;
    }

    private void OnDisable() {
        USNLCallbackEvents.OnClientConnected -= OnConnected;
        USNLCallbackEvents.OnVariablesTestPacket -= OnVariablesTestPacket;
    }

    private void OnConnected(object _object) {
        byte varByte = 1;
        short varShort = 2;
        int varInt = 3;
        long varLong = 4;
        float varFloat = 4.20f;
        bool varBool = true;
        string varString = "This is a string";
        Vector2 varVec2 = new Vector2(2, 2);
        Vector3 varVec3 = new Vector3(3, 3, 3);
        Quaternion varQuat = new Quaternion(4, 4, 4, 4);

        byte[] varArrayByte = { 1, 1, 1 };
        short[] varArrayShort = { 2, 2, 2 };
        int[] varArrayInt = { 3, 3, 3 };
        long[] varArrayLong = { 4, 4, 4 };
        float[] varArrayFloat = { 4.20f, 4.20f, 4.20f };
        bool[] varArrayBool = { true, false, true };
        string[] varArrayString = { "This is a string", "String 2", "Third string" };
        Vector2[] varArrayVec2 = { new Vector2(2, 2), new Vector2(2, 3), new Vector2(2, 4) };
        Vector3[] varArrayVec3 = { new Vector3(3, 3, 3), new Vector3(3, 3, 4), new Vector3(3, 3, 5) };
        Quaternion[] varArrayQuat = { new Quaternion(4, 4, 4, 4), new Quaternion(4, 4, 4, 5), new Quaternion(4, 4, 4, 6) };

        PacketSend.VariablesTest((int)_object, varByte, varShort, varInt, varLong, varFloat, varBool, varString, varVec2, varVec3, varQuat, varArrayByte, varArrayShort, varArrayInt, varArrayLong, varArrayFloat, varArrayBool, varArrayString, varArrayVec2, varArrayVec3, varArrayQuat);

        PrintVariablesTestPacket(new object[] { varByte, varShort, varInt, varLong, varFloat, varBool, varString, varVec2, varVec3, varQuat, String.Join(", ", varArrayByte), String.Join(", ", varArrayShort), String.Join(", ", varArrayInt), String.Join(", ", varArrayLong), String.Join(", ", varArrayFloat), String.Join(", ", varArrayBool), String.Join(", ", varArrayString), String.Join(", ", varArrayVec2), String.Join(", ", varArrayVec3), String.Join(", ", varArrayQuat) });
    }

    private void OnVariablesTestPacket(object _packetObject) {
        VariablesTestPacket _packet = (VariablesTestPacket)_packetObject;

        byte varByte = _packet.VarByte;
        short varShort = _packet.VarShort;
        int varInt = _packet.VarInt;
        long varLong = _packet.VarLong;
        float varFloat = _packet.VarFloat;
        bool varBool = _packet.VarBool;
        string varString = _packet.VarString;
        Vector2 varVec2 = _packet.VarVec2;
        Vector3 varVec3 = _packet.VarVec3;
        Quaternion varQuat = _packet.VarQuat;

        byte[] varArrayByte = _packet.VarArrayByte;
        short[] varArrayShort = _packet.VarArrayShort;
        int[] varArrayInt = _packet.VarArrayInt;
        long[] varArrayLong = _packet.VarArrayLong;
        float[] varArrayFloat = _packet.VarArrayFloat;
        bool[] varArrayBool = _packet.VarArrayBool;
        string[] varArrayString = _packet.VarArrayString;
        Vector2[] varArrayVec2 = _packet.VarArrayVec2;
        Vector3[] varArrayVec3 = _packet.VarArrayVec3;
        Quaternion[] varArrayQuat = _packet.VarArrayQuat;

        PrintVariablesTestPacket(new object[] { varByte, varShort, varInt, varLong, varFloat, varBool, varString, varVec2, varVec3, varQuat, String.Join(", ", varArrayByte), String.Join(", ", varArrayShort), String.Join(", ", varArrayInt), String.Join(", ", varArrayLong), String.Join(", ", varArrayFloat), String.Join(", ", varArrayBool), String.Join(", ", varArrayString), String.Join(", ", varArrayVec2), String.Join(", ", varArrayVec3), String.Join(", ", varArrayQuat) });
    }

    private void PrintVariablesTestPacket(object[] _objects) {
        Debug.Log(string.Join(", ", _objects));
    }
}
