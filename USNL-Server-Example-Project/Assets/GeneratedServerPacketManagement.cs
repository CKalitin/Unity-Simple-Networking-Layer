using System.Collections.Generic;using UnityEngine;

// Sent from Server to Client
public enum ServerPackets {
    Welcome = 1,
    Packet2 = 2,
}

// Sent from Client to Server
public enum ClientPackets {
    WelcomeReceived = 1,
    ClientInput = 2,
}

/*** Packet Structs ***/

public struct WelcomeReceivedPacket {
    private int clientId;

    private int clientIdCheck;
    private float anotherVariable;

    public WelcomeReceivedPacket(int _clientId, int _clientIdCheck, float _anotherVariable) {
        clientId = _clientId;
        clientIdCheck = _clientIdCheck;
        anotherVariable = _anotherVariable;
    }

    public int ClientId { get => clientId; set => clientId = value; }
    public int ClientIdCheck { get => clientIdCheck; set => clientIdCheck = value; }
    public float AnotherVariable { get => anotherVariable; set => anotherVariable = value; }
}
public struct ClientInputPacket {
    private int clientId;

    private Vector2 mousePosition;
    private Quaternion colour;

    public ClientInputPacket(int _clientId, Vector2 _mousePosition, Quaternion _colour) {
        clientId = _clientId;
        mousePosition = _mousePosition;
        colour = _colour;
    }

    public int ClientId { get => clientId; set => clientId = value; }
    public Vector2 MousePosition { get => mousePosition; set => mousePosition = value; }
    public Quaternion Colour { get => colour; set => colour = value; }
}

public static class PacketHandlers {
    public delegate void PacketHandler(Packet _packet);
    public static List<PacketHandler> packetHandlers = new List<PacketHandler>() {
        { WelcomeReceived },
        { ClientInput },
    };

    public static void WelcomeReceived(Packet _packet) {
        WelcomeReceivedPacket welcomeReceivedPacket = new WelcomeReceivedPacket(_packet.PacketId, _packet.ReadInt(), _packet.ReadFloat());
        PacketManager.instance.PacketReceived(_packet, welcomeReceivedPacket);
    }

    public static void ClientInput(Packet _packet) {
        ClientInputPacket clientInputPacket = new ClientInputPacket(_packet.PacketId, _packet.ReadVector2(), _packet.ReadQuaternion());
        PacketManager.instance.PacketReceived(_packet, clientInputPacket);
    }
}

public static class PacketSend {
    #region TCP & UDP Send Functions

    private static void SendTCPData(int _toClient, Packet _packet) {
        _packet.WriteLength();
        Server.clients[_toClient].Tcp.SendData(_packet);
    }

    private static void SendTCPDataToAll(Packet _packet) {
        _packet.WriteLength();
        for (int i = 1; i < Server.MaxClients; i++) {
            Server.clients[i].Tcp.SendData(_packet);
        }
    }

    private static void SendTCPDataToAll(int _excpetClient, Packet _packet) {
        _packet.WriteLength();
        for (int i = 1; i < Server.MaxClients; i++) {
            if (i != _excpetClient) {
                Server.clients[i].Tcp.SendData(_packet);
            }
        }
    }

    private static void SendUDPData(int _toClient, Packet _packet) {
        _packet.WriteLength();
        Server.clients[_toClient].Udp.SendData(_packet);
    }

    private static void SendUDPDataToAll(Packet _packet) {
        _packet.WriteLength();
        for (int i = 1; i < Server.MaxClients; i++) {
            Server.clients[i].Udp.SendData(_packet);
        }
    }

    private static void SendUDPDataToAll(int _excpetClient, Packet _packet) {
        _packet.WriteLength();
        for (int i = 1; i < Server.MaxClients; i++) {
            if (i != _excpetClient) {
                Server.clients[i].Udp.SendData(_packet);
            }
        }
    }

    #endregion

    public static void Welcome(int _toClient, string _msg, Vector2 _justAVector2) {
        using (Packet _packet = new Packet((int)ServerPackets.Welcome)) {
            _packet.Write(_toClient);
            _packet.Write(_msg);
            _packet.Write(_justAVector2);

            SendTCPData(_toClient, _packet);
        }
    }

    public static void Packet2(int _exceptClient, byte[] _factorioIsFun) {
        using (Packet _packet = new Packet((int)ServerPackets.Packet2)) {
            _packet.Write(_factorioIsFun);

            SendTCPDataToAll(_packet);
        }
    }
}
