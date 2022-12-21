using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace USNL.Package {
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
            //Debug.Log($"Packet Received: {Enum.GetName(typeof(ServerPackets), _packet.PacketId)}");

            // Break out of Packet Handle Thread
            USNL.Package.ThreadManager.ExecuteOnMainThread(() => {
                // Call callback events
                USNL.CallbackEvents.PacketCallbackEvents[_packet.PacketId](_packetStruct);
            });
        }
    }
}
