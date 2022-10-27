using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PacketManager : MonoBehaviour {
    public static PacketManager instance;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Debug.Log("Packet Manager instance already exists, destroying object.");
            Destroy(this);
        }
    }

    public void PacketReceived(Packet _packet, object _packetStruct) {
        Debug.Log($"Packet Received: {Enum.GetName(typeof(ServerPackets), _packet.PacketId)}");

        object[] parameters = new object[] { _packetStruct };

        // Call callback events
        if (!ClientManager.instance.PacketManager) { return; }
        USNLCallbackEvents.PacketCallbackEvents[_packet.PacketId](_packetStruct);
    }
}
