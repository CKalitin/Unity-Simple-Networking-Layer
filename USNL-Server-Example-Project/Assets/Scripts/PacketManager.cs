using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PacketManager : MonoBehaviour {
    public static PacketManager instance;

    private Dictionary<int, List<PacketReceivedCallback>> packetReceivedCallbacks = new Dictionary<int, List<PacketReceivedCallback>>();

    private struct PacketReceivedCallback {
        private MethodInfo methodInfo;
        private Type classType;

        public PacketReceivedCallback(MethodInfo _methodInfo, Type _type) {
            methodInfo = _methodInfo;
            classType = _type;
        }

        public MethodInfo MethodInfo { get => methodInfo; set => methodInfo = value; }
        public Type ClassType { get => classType; set => classType = value; }
    }


    private void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Debug.Log("Packet Manager instance already exists, destroying object.");
            Destroy(this);
        }

        GeneratePacketReceivedCallbacks();

        Debug.Log(packetReceivedCallbacks[0][0].ClassType);
        Debug.Log(packetReceivedCallbacks[0][0].MethodInfo);

        object[] parameters = new object[] { /*new WelcomeReceivedPacket()*/ };
        packetReceivedCallbacks[0][0].MethodInfo.Invoke(FindObjectOfType<Test>(), parameters);
        parameters = new object[] { new ClientInputPacket() };
        packetReceivedCallbacks[1][0].MethodInfo.Invoke(packetReceivedCallbacks[1][0].ClassType, parameters);

        for (int i = 0; i < packetReceivedCallbacks.Count; i++) {
            for (int x = 0; x < packetReceivedCallbacks[i].Count; x++) {
                Debug.Log(packetReceivedCallbacks[i][x]);
            }
        }
    }

    #region Packet Received Callbacks

    public void PacketReceived(Packet _packet, object _packetStruct) {
        // Yes, I used object as a parameter, and yes i know. ew i hate this code already why couldn't I have thought of something better

        Debug.Log($"Packet Received: {_packet.PacketId}");

        object[] parameters = new object[] { _packetStruct };

        for (int i = 0; i < packetReceivedCallbacks[_packet.PacketId].Count; i++) {
            var type = packetReceivedCallbacks[_packet.PacketId][i].ClassType.GetType();
            var fuck = FindObjectsOfType<>().Length;

            for (int x = 0; x < FindObjectsOfType<>().Length; x++)
            packetReceivedCallbacks[_packet.PacketId][i].MethodInfo.Invoke(packetReceivedCallbacks[_packet.PacketId][i].ClassType, parameters);
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
                MethodInfo method = types[i].GetMethod($"On{Enum.GetNames(typeof(ClientPackets))[x]}Packet", BindingFlags.NonPublic);
                if (method == null) { method = types[i].GetMethod($"On{Enum.GetNames(typeof(ClientPackets))[x]}Packet"); }

                if (method != null) {
                    if (!packetReceivedCallbacks.ContainsKey(x)) { packetReceivedCallbacks.Add(x, new List<PacketReceivedCallback>() { }); } // Initialize dictionary entry of key x
                    packetReceivedCallbacks[x].Add(new PacketReceivedCallback(method, types[i]));
                }
            }
        }
    }

    // Gets all user types (class / monobehaviour names)
    private static List<Type> GetAllUserScriptTypes() {
        // https://forum.unity.com/threads/geting-a-array-or-list-of-all-unity-types.416976/
        List<Type> results = new List<Type>();

        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
            if (assembly.FullName.StartsWith("Assembly-CSharp")) {
                foreach (Type type in assembly.GetTypes()) { results.Add(type); }
                break;
            }
        }

        return results;
    }

    #endregion
}
