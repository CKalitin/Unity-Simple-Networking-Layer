using System.Collections.Generic;using UnityEngine;

// Sent from Server to Client
public enum ServerPackets {
    Welcome = 1,
    FactorioIsFun = 2,
}

// Sent from Client to Server
public enum ClientPackets {
    WelcomeReceived = 1,
    ClientInput = 2,
}

/*** Packet Structs ***/

public struct WelcomePacket {
    private int clientId;

    private string msg;

    public WelcomePacket(int _clientId, string _msg) {
        clientId = _clientId;
        msg = _msg;
    }

    public int ClientId { get => clientId; set => clientId = value; }
    public string Msg { get => msg; set => msg = value; }
}
public struct FactorioIsFunPacket {
    private int clientId;

    private int justAnInt;

    public FactorioIsFunPacket(int _clientId, int _justAnInt) {
        clientId = _clientId;
        justAnInt = _justAnInt;
    }

    public int ClientId { get => clientId; set => clientId = value; }
    public int JustAnInt { get => justAnInt; set => justAnInt = value; }
}

public static class PacketHandlers {
    public delegate void PacketHandler(Packet _packet);
    public static List<PacketHandler> packetHandlers = new List<PacketHandler>() {
        { Welcome },
        { FactorioIsFun },
    };

    public static void Welcome(Packet _packet) {
        WelcomePacket welcomePacket = new WelcomePacket(_packet.PacketId, _packet.ReadString());
        PacketManager.instance.PacketReceived(_packet, welcomePacket);
    }

    public static void FactorioIsFun(Packet _packet) {
        FactorioIsFunPacket factorioIsFunPacket = new FactorioIsFunPacket(_packet.PacketId, _packet.ReadInt());
        PacketManager.instance.PacketReceived(_packet, factorioIsFunPacket);
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

    public static void ClientInput(Vector2 _mousePosition, Quaternion _colour) {
        using (Packet _packet = new Packet((int)ClientPackets.ClientInput)) {
            _packet.Write(_mousePosition);
            _packet.Write(_colour);

            SendUDPData(_packet);
        }
    }
}
