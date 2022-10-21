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

public struct WelcomeReceivedPacket {
    private int fromClient;

    private int clientIdCheck;

    public WelcomeReceivedPacket(int _fromClient, int _clientIdCheck) {
        fromClient = _fromClient;
        clientIdCheck = _clientIdCheck;
    }

    public int FromClient { get => fromClient; set => fromClient = value; }
    public int ClientIdCheck { get => clientIdCheck; set => clientIdCheck = value; }
}

public static class PacketHandlers {
    public delegate void PacketHandler(Packet _packet);
    public static List<PacketHandler> packetHandlers = new List<PacketHandler>() {
        { WelcomeReceived },
    };

    public static void WelcomeReceived(Packet _packet) {
        WelcomeReceivedPacket welcomeReceivedPacket = new WelcomeReceivedPacket(_packet.PacketId, _packet.ReadInt());
        PacketManager.instance.PacketReceived(_packet, welcomeReceivedPacket);
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

    public static void Welcome(int _toClient, string _welcomeMessage, int _clientId) {
        using (Packet _packet = new Packet((int)ServerPackets.Welcome)) {
            _packet.Write(_toClient);
            _packet.Write(_welcomeMessage);
            _packet.Write(_clientId);

            SendTCPData(_toClient, _packet);
        }
    }
}
