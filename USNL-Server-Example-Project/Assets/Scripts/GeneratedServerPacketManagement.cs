using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Sent from server to client.
public enum ServerPackets {
    welcome = 1
}

/// Sent from client to server.
public enum ClientPackets {
    welcomeReceived = 1
}

// Packet Structs:

public struct WelcomeReceivedPacket {
    private int clientIdCheck;

    public int ClientIdCheck { get => clientIdCheck; set => clientIdCheck = value; }
}

public static class PacketHandler {
    public static void WelcomeReceived(int _fromClient, Packet _packet) {
        int _clientIdCheck = _packet.ReadInt();

        Debug.Log($"{Server.clients[_fromClient].Tcp.socket.Client.RemoteEndPoint} connected successfully and is now Player {_fromClient}.");
        if (_fromClient != _clientIdCheck) {
            Debug.Log($"ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck}.");
        }

        PacketManager.instance.PacketReceived(_packet);
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

    #region Packet Send Functions

    public static void Welcome(int _toClient, string _msg) {
        using (Packet _packet = new Packet((int)ServerPackets.welcome)) {
            _packet.Write(_msg);
            _packet.Write(_toClient); // Client needs to know its own id

            SendTCPData(_toClient, _packet);
        }
    }

    #endregion
}
