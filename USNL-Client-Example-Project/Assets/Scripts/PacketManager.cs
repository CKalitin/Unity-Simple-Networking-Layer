using System;
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

        // Track how long this takes in a project with many scripts
        GeneratePacketReceivedCallbacks();
    }

    #region Packet Received Callbacks

    public void PacketReceived(Packet _packet, object _packetStruct) {
        // Yes, I used object as a parameter, and yes i know. ew i hate this code already why couldn't I have thought of something better, is this really that unsafe? probably.

        Debug.Log($"Packet Received: {Enum.GetName(typeof(ServerPackets), _packet.PacketId)}");

        object[] parameters = new object[] { _packetStruct };
        CallPacketReceivedCallbacks(_packet.PacketId, parameters);
    }

    private void CallPacketReceivedCallbacks(int _packetId, object[] _parameters) {
        if (packetReceivedCallbacks.ContainsKey(_packetId)) {
            // Loop through all callback methods
            for (int i = 0; i < packetReceivedCallbacks[_packetId].Count; i++) {
                // Get and Loop through all classes of type of the base call of method[i]
                List<MonoBehaviour> types = GetObjectsOfType(packetReceivedCallbacks[_packetId][i].ClassType);
                for (int x = 0; x < types.Count; x++) {
                    try {
                        packetReceivedCallbacks[_packetId][i].MethodInfo.Invoke(types[x], _parameters);
                    } catch (Exception _ex) {
                        Debug.LogError($"Could not run packet callback function: {packetReceivedCallbacks[_packetId][i].MethodInfo} in class {types[x].GetType()}\n{_ex}");
                    }
                }
            }
        }
    }

    private List<MonoBehaviour> GetObjectsOfType(Type _t) {
        // Pretty slow alternative to using FindObjectsOfType<>(), but that doesn't work with a variable type

        MonoBehaviour[] monoBehaviours = FindObjectsOfType<MonoBehaviour>();
        List<MonoBehaviour> output = new List<MonoBehaviour>();

        for (int i = 0; i < monoBehaviours.Length; i++) {
            if (monoBehaviours[i].GetType() == _t) {
                output.Add(monoBehaviours[i]);
            }
        }

        return output;
    }

    private void GeneratePacketReceivedCallbacks() {
        // https://stackoverflow.com/questions/540066/calling-a-function-from-a-string-in-c-sharp

        List<Type> types = GetAllUserScriptTypes();

        // Iterate through every user script type
        for (int i = 0; i < types.Count; i++) {
            // Iterate through every server packet type
            for (int x = 0; x < Enum.GetNames(typeof(ServerPackets)).Length; x++) {
                // Find method of name "On{packetName}Packet" / "OnWelcomeReceivedPacket()"
                MethodInfo method = types[i].GetMethod($"On{Enum.GetNames(typeof(ServerPackets))[x]}Packet", BindingFlags.NonPublic | BindingFlags.Instance);
                if (method == null) { method = types[i].GetMethod($"On{Enum.GetNames(typeof(ServerPackets))[x]}Packet"); }

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
