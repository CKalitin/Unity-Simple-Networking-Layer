using System.Collections.Generic;using UnityEngine;

// Sent from Server to Client
public enum ServerPackets {
    Welcome,
}

// Sent from Client to Server
public enum ClientPackets {
    WelcomeReceived,
}

/*** Packet Structs ***/

public struct WelcomePacket {
    private string welcomeMessage;
    private int clientId;

    public WelcomePacket(string _welcomeMessage, int _clientId) {
        welcomeMessage = _welcomeMessage;
        clientId = _clientId;
    }

    public string WelcomeMessage { get => welcomeMessage; set => welcomeMessage = value; }
    public int ClientId { get => clientId; set => clientId = value; }
}

public static class PacketHandlers {
    public delegate void PacketHandler(Packet _packet);
    public static List<PacketHandler> packetHandlers = new List<PacketHandler>() {
        { Welcome },
    };

    public static void Welcome(Packet _packet) {
_packet.ReadInt(); // This needs to be here for packet reading, idk why it just works.
        WelcomePacket welcomePacket = new WelcomePacket(_packet.ReadString(), _packet.ReadInt());
        PacketManager.instance.PacketReceived(_packet, welcomePacket);
    }
}

public static class PacketSend {
    private static void SendTCPData(Packet _packet) {
        _packet.WriteLength();
        if (Client.instance.IsConnected) {
            Client.instance.Tcp.SendData(_packet);
        }
    }

    private static void SendUDPData(Packet _packet) {
        _packet.WriteLength();
        if (Client.instance.IsConnected) {
            Client.instance.Udp.SendData(_packet);
        }
    }

    public static void WelcomeReceived(int _clientIdCheck) {
        using (Packet _packet = new Packet((int)ClientPackets.WelcomeReceived)) {
            _packet.Write(_clientIdCheck);

            SendTCPData(_packet);
        }
    }
}
