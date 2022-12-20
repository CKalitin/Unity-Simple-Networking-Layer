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
        //Debug.Log($"Packet Received: {Enum.GetName(typeof(ClientPackets), _packet.PacketId)}"); TODO DELETE

        // Break out of Packet Handle Thread
        ThreadManager.ExecuteOnMainThread(() => {
            // Call callback events
            USNLCallbackEvents.PacketCallbackEvents[_packet.PacketId](_packetStruct);
        });
    }
}
