using System.Collections.Generic;
using UnityEngine;

#region Packets

// Sent from Server to Client
public enum ServerPackets {
    Welcome,
    SyncedObjectInstantiate,
    SyncedObjectDestroy,
    SyncedObjectUpdate,
}

// Sent from Client to Server
public enum ClientPackets {
    WelcomeReceived,
    ClientInput,
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

public struct ClientInputPacket {
    private int fromClient;

    private byte[] keycodesDown;
    private byte[] keycodesUp;

    public ClientInputPacket(int _fromClient, byte[] _keycodesDown, byte[] _keycodesUp) {
        fromClient = _fromClient;
        keycodesDown = _keycodesDown;
        keycodesUp = _keycodesUp;
    }

    public int FromClient { get => fromClient; set => fromClient = value; }
    public byte[] KeycodesDown { get => keycodesDown; set => keycodesDown = value; }
    public byte[] KeycodesUp { get => keycodesUp; set => keycodesUp = value; }
}

public static class PacketHandlers {
    public delegate void PacketHandler(Packet _packet);
    public static List<PacketHandler> packetHandlers = new List<PacketHandler>() {
        { WelcomeReceived },
        { ClientInput },
    };

    public static void WelcomeReceived(Packet _packet) {
        int clientIdCheck = _packet.ReadInt();

        WelcomeReceivedPacket welcomeReceivedPacket = new WelcomeReceivedPacket(_packet.FromClient, clientIdCheck);
        PacketManager.instance.PacketReceived(_packet, welcomeReceivedPacket);
    }

    public static void ClientInput(Packet _packet) {
        byte[] keycodesDown = _packet.ReadBytes(_packet.ReadInt());
if (keycodesDown.Length <= 0) { _packet.ReadInt(); }
        byte[] keycodesUp = _packet.ReadBytes(_packet.ReadInt());
if (keycodesUp.Length <= 0) { _packet.ReadInt(); }

        ClientInputPacket clientInputPacket = new ClientInputPacket(_packet.FromClient, keycodesDown, keycodesUp);
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
        for (int i = 0; i < Server.MaxClients; i++) {
            Server.clients[i].Tcp.SendData(_packet);
        }
    }

    private static void SendTCPDataToAll(int _excpetClient, Packet _packet) {
        _packet.WriteLength();
        for (int i = 0; i < Server.MaxClients; i++) {
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
        for (int i = 0; i < Server.MaxClients; i++) {
            Server.clients[i].Udp.SendData(_packet);
        }
    }

    private static void SendUDPDataToAll(int _excpetClient, Packet _packet) {
        _packet.WriteLength();
        for (int i = 0; i < Server.MaxClients; i++) {
            if (i != _excpetClient) {
                Server.clients[i].Udp.SendData(_packet);
            }
        }
    }

    #endregion

    public static void Welcome(int _toClient, string _welcomeMessage, int _clientId) {
        using (Packet _packet = new Packet((int)ServerPackets.Welcome)) {
            _packet.Write(_welcomeMessage);
            _packet.Write(_clientId);

            SendTCPData(_toClient, _packet);
        }
    }

    public static void SyncedObjectInstantiate(int _toClient, int _syncedObjectPrefebId, int _syncedObjectUUID, Vector3 _position, Quaternion _rotation, Vector3 _scale) {
        using (Packet _packet = new Packet((int)ServerPackets.SyncedObjectInstantiate)) {
            _packet.Write(_syncedObjectPrefebId);
            _packet.Write(_syncedObjectUUID);
            _packet.Write(_position);
            _packet.Write(_rotation);
            _packet.Write(_scale);

            SendTCPData(_toClient, _packet);
        }
    }

    public static void SyncedObjectDestroy(int _toClient, int _syncedObjectUUID) {
        using (Packet _packet = new Packet((int)ServerPackets.SyncedObjectDestroy)) {
            _packet.Write(_syncedObjectUUID);

            SendTCPData(_toClient, _packet);
        }
    }

    public static void SyncedObjectUpdate(int _syncedObjectUUID, Vector3 _position, Quaternion _rotation, Vector3 _scale) {
        using (Packet _packet = new Packet((int)ServerPackets.SyncedObjectUpdate)) {
            _packet.Write(_syncedObjectUUID);
            _packet.Write(_position);
            _packet.Write(_rotation);
            _packet.Write(_scale);

            SendUDPDataToAll(_packet);
        }
    }
}

#endregion Packets

#region Callbacks

public static class USNLCallbackEvents {
    public delegate void USNLCallbackEvent(object _param);

    public static USNLCallbackEvent[] PacketCallbackEvents = {
        CallOnWelcomeReceivedPacketCallbacks,
        CallOnClientInputPacketCallbacks,
    };

    public static event USNLCallbackEvent OnServerStarted;
    public static event USNLCallbackEvent OnServerStopped;
    public static event USNLCallbackEvent OnClientConnected;
    public static event USNLCallbackEvent OnClientDisconnected;

    public static event USNLCallbackEvent OnWelcomeReceivedPacket;
    public static event USNLCallbackEvent OnClientInputPacket;

    public static void CallOnServerStartedCallbacks(object _param) { if (OnServerStarted != null) { OnServerStarted(_param); } }
    public static void CallOnServerStoppedCallbacks(object _param) { if (OnServerStopped != null) { OnServerStopped(_param); } }
    public static void CallOnClientConnectedCallbacks(object _param) { if (OnClientConnected != null) { OnClientConnected(_param); } }
    public static void CallOnClientDisconnectedCallbacks(object _param) { if (OnClientDisconnected != null) { OnClientDisconnected(_param); } }

    public static void CallOnWelcomeReceivedPacketCallbacks(object _param) { if (OnWelcomeReceivedPacket != null) { OnWelcomeReceivedPacket(_param); } }
    public static void CallOnClientInputPacketCallbacks(object _param) { if (OnClientInputPacket != null) { OnClientInputPacket(_param); } }
}

#endregion
