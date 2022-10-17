using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PacketManager : MonoBehaviour {
    public static PacketManager instance;

    private List<List<MethodInfo>> packetReceivedCallbacks = new List<List<MethodInfo>>(); // Index is packet index in enum

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Debug.Log("Packet Manager instance already exists, destroying object.");
            Destroy(this);
        }

        // TODO: Track how long this takes in a project with tons of scripts
        GeneratePacketReceivedCallbacks();
    }

    #region Packet Received Callbacks

    public void PacketReceived(Packet _packet) {
        object[] parameters = new object[] { _packet }; // passed to function (hope c# garbage collection gets this (probably will))

        for (int i = 0; i < packetReceivedCallbacks[_packet.PacketId].Count; i++) {
            packetReceivedCallbacks[_packet.PacketId][i].Invoke(this, parameters);
        }
    }

    private void GeneratePacketReceivedCallbacks() {
        // https://stackoverflow.com/questions/540066/calling-a-function-from-a-string-in-c-sharp

        List<Type> types = GetAllUserScriptTypes();

        // Iterate through every user script type
        for (int i = 0; i < types.Count; i++) {
            // Iterate through every server packet type
            for (int x = 0; x < Enum.GetNames(typeof(ClientPackets)).Length; x++) {
                // Find method of name "On{packetName}Packet" / "OnWelcomeReceivedPacket()"
                MethodInfo method = types[i].GetMethod($"On{Enum.GetNames(typeof(ClientPackets))[x]}Packet");
                packetReceivedCallbacks[i].Add(method);
            }
        }
    }

    // Gets all user types (class / monobehaviour names)
    private static List<Type> GetAllUserScriptTypes() {
        // https://forum.unity.com/threads/geting-a-array-or-list-of-all-unity-types.416976/
        List<Type> results = new List<Type>();
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
            if (assembly.FullName.StartsWith("Assembly--CSharp.dll")) {
                foreach (Type type in assembly.GetTypes()) { results.Add(type); }
                break;
            }
        }
        return results;
    }

    #endregion
}
