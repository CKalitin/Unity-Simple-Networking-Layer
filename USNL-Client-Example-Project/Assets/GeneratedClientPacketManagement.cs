using System.Collections.Generic;using UnityEngine;

// Sent from Server to Client
public enum ServerPackets {
    Welcome = 1,
}

// Sent from Client to Server
public enum ClientPackets {
    WelcomeReceived = 1,
}

/*** Packet Structs ***/

public struct WelcomePacket {
    private int clientId;

    private string welcomeMessage;

    public WelcomePacket(int _clientId, string _welcomeMessage) {
        clientId = _clientId;
        welcomeMessage = _welcomeMessage;
    }

    public int ClientId { get => clientId; set => clientId = value; }
    public string WelcomeMessage { get => welcomeMessage; set => welcomeMessage = value; }
}

public static class PacketHandlers {
    public delegate void PacketHandler(Packet _packet);
    public static List<PacketHandler> packetHandlers = new List<PacketHandler>() {
        { Welcome },
    };

    public static void Welcome(Packet _packet) {
        Debug.Log("Welcome Packet Handler");
        WelcomePacket welcomePacket = new WelcomePacket(_packet.PacketId, _packet.ReadString());
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
