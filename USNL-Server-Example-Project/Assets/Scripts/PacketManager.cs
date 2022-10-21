using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PacketManager : MonoBehaviour {
    public static PacketManager instance;

    private Dictionary<int, CallbackManager> packetReceivedCallbacks = new Dictionary<int, CallbackManager>();

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Debug.Log("Packet Manager instance already exists, destroying object.");
            Destroy(this);
        }

        GenerateCallbacks();
    }

    private void GenerateCallbacks() {
        for (int x = 0; x < Enum.GetNames(typeof(ClientPackets)).Length; x++) {
            // In any user script if they make a private or public function named "On(PacketName)Packet" it will be called when that packet is received
            CallbackManager _callbackManager = new CallbackManager(new string[] { $"On{ Enum.GetNames(typeof(ClientPackets))[x] }Packet" });
            packetReceivedCallbacks.Add(x, _callbackManager);
        }
    }

    public void PacketReceived(Packet _packet, object _packetStruct) {
        Debug.Log($"Packet Received: {Enum.GetName(typeof(ClientPackets), _packet.PacketId)}");

        object[] parameters = new object[] { _packetStruct };

        packetReceivedCallbacks[_packet.PacketId].CallCallbacks(parameters);
    }
}
