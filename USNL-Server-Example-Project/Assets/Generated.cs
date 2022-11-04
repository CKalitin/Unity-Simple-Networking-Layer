using System.Collections.Generic;
using UnityEngine;

#region Packets

#region Packet Enums

// Sent from Server to Client
public enum ServerPackets {
    Welcome,
    Ping,
    SyncedObjectInstantiate,
    SyncedObjectDestroy,
    SyncedObjectInterpolationMode,
    SyncedObjectVec2PosUpdate,
    SyncedObjectVec3PosUpdate,
    SyncedObjectRotZUpdate,
    SyncedObjectRotUpdate,
    SyncedObjectVec2ScaleUpdate,
    SyncedObjectVec3ScaleUpdate,
    SyncedObjectVec2PosInterpolation,
    SyncedObjectVec3PosInterpolation,
    SyncedObjectRotZInterpolation,
    SyncedObjectRotInterpolation,
    SyncedObjectVec2ScaleInterpolation,
    SyncedObjectVec3ScaleInterpolation,
}

// Sent from Client to Server
public enum ClientPackets {
    WelcomeReceived,
    Ping,
    ClientInput,
}

#endregion

#region Packet Structs

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

public struct PingPacket {
    private int fromClient;

    private bool sendPingBack;

    public PingPacket(int _fromClient, bool _sendPingBack) {
        fromClient = _fromClient;
        sendPingBack = _sendPingBack;
    }

    public int FromClient { get => fromClient; set => fromClient = value; }
    public bool SendPingBack { get => sendPingBack; set => sendPingBack = value; }
}

public struct ClientInputPacket {
    private int fromClient;

    private int[] keycodesDown;
    private int[] keycodesUp;

    public ClientInputPacket(int _fromClient, int[] _keycodesDown, int[] _keycodesUp) {
        fromClient = _fromClient;
        keycodesDown = _keycodesDown;
        keycodesUp = _keycodesUp;
    }

    public int FromClient { get => fromClient; set => fromClient = value; }
    public int[] KeycodesDown { get => keycodesDown; set => keycodesDown = value; }
    public int[] KeycodesUp { get => keycodesUp; set => keycodesUp = value; }
}

#endregion

#region Packet Handlers

public static class PacketHandlers {
    public delegate void PacketHandler(Packet _packet);
    public static List<PacketHandler> packetHandlers = new List<PacketHandler>() {
        { WelcomeReceived },
        { Ping },
        { ClientInput },
    };

    public static void WelcomeReceived(Packet _packet) {
        int clientIdCheck = _packet.ReadInt();

        WelcomeReceivedPacket welcomeReceivedPacket = new WelcomeReceivedPacket(_packet.FromClient, clientIdCheck);
        PacketManager.instance.PacketReceived(_packet, welcomeReceivedPacket);
    }

    public static void Ping(Packet _packet) {
        bool sendPingBack = _packet.ReadBool();

        PingPacket pingPacket = new PingPacket(_packet.FromClient, sendPingBack);
        PacketManager.instance.PacketReceived(_packet, pingPacket);
    }

    public static void ClientInput(Packet _packet) {
        int[] keycodesDown = _packet.ReadInts();
        int[] keycodesUp = _packet.ReadInts();

        ClientInputPacket clientInputPacket = new ClientInputPacket(_packet.FromClient, keycodesDown, keycodesUp);
        PacketManager.instance.PacketReceived(_packet, clientInputPacket);
    }
}

#endregion

#region Packet Send

public static class PacketSend {
    #region TCP & UDP Send Functions

    private static void SendTCPData(int _toClient, Packet _packet) {
        _packet.WriteLength();
        Server.Clients[_toClient].Tcp.SendData(_packet);
            if (Server.Clients[_toClient].IsConnected) { NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length()); }
    }

    private static void SendTCPDataToAll(Packet _packet) {
        _packet.WriteLength();
        for (int i = 0; i < Server.MaxClients; i++) {
            Server.Clients[i].Tcp.SendData(_packet);
            if (Server.Clients[i].IsConnected) { NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length()); }
        }
    }

    private static void SendTCPDataToAll(int _excpetClient, Packet _packet) {
        _packet.WriteLength();
        for (int i = 0; i < Server.MaxClients; i++) {
            if (i != _excpetClient) {
                Server.Clients[i].Tcp.SendData(_packet);
                if (Server.Clients[i].IsConnected) { NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length()); }
            }
        }
    }

    private static void SendUDPData(int _toClient, Packet _packet) {
        _packet.WriteLength();
        Server.Clients[_toClient].Udp.SendData(_packet);
        if (Server.Clients[_toClient].IsConnected) { NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length()); }
    }

    private static void SendUDPDataToAll(Packet _packet) {
        _packet.WriteLength();
        for (int i = 0; i < Server.MaxClients; i++) {
            Server.Clients[i].Udp.SendData(_packet);
            if (Server.Clients[i].IsConnected) { NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length()); }
        }
    }

    private static void SendUDPDataToAll(int _excpetClient, Packet _packet) {
        _packet.WriteLength();
        for (int i = 0; i < Server.MaxClients; i++) {
            if (i != _excpetClient) {
                Server.Clients[i].Udp.SendData(_packet);
                if (Server.Clients[i].IsConnected) { NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length()); }
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

    public static void Ping(int _toClient, bool _sendPingBack) {
        using (Packet _packet = new Packet((int)ServerPackets.Ping)) {
            _packet.Write(_sendPingBack);

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

    public static void SyncedObjectInterpolationMode(int _toClient, bool _serverInterpolation) {
        using (Packet _packet = new Packet((int)ServerPackets.SyncedObjectInterpolationMode)) {
            _packet.Write(_serverInterpolation);

            SendTCPData(_toClient, _packet);
        }
    }

    public static void SyncedObjectVec2PosUpdate(int[] _syncedObjectUUIDs, Vector2[] _positions) {
        using (Packet _packet = new Packet((int)ServerPackets.SyncedObjectVec2PosUpdate)) {
            _packet.Write(_syncedObjectUUIDs);
            _packet.Write(_positions);

            SendUDPDataToAll(_packet);
        }
    }

    public static void SyncedObjectVec3PosUpdate(int[] _syncedObjectUUIDs, Vector3[] _positions) {
        using (Packet _packet = new Packet((int)ServerPackets.SyncedObjectVec3PosUpdate)) {
            _packet.Write(_syncedObjectUUIDs);
            _packet.Write(_positions);

            SendUDPDataToAll(_packet);
        }
    }

    public static void SyncedObjectRotZUpdate(int[] _syncedObjectUUIDs, float[] _rotations) {
        using (Packet _packet = new Packet((int)ServerPackets.SyncedObjectRotZUpdate)) {
            _packet.Write(_syncedObjectUUIDs);
            _packet.Write(_rotations);

            SendUDPDataToAll(_packet);
        }
    }

    public static void SyncedObjectRotUpdate(int[] _syncedObjectUUIDs, Vector3[] _rotations) {
        using (Packet _packet = new Packet((int)ServerPackets.SyncedObjectRotUpdate)) {
            _packet.Write(_syncedObjectUUIDs);
            _packet.Write(_rotations);

            SendUDPDataToAll(_packet);
        }
    }

    public static void SyncedObjectVec2ScaleUpdate(int[] _syncedObjectUUIDs, Vector2[] _scales) {
        using (Packet _packet = new Packet((int)ServerPackets.SyncedObjectVec2ScaleUpdate)) {
            _packet.Write(_syncedObjectUUIDs);
            _packet.Write(_scales);

            SendUDPDataToAll(_packet);
        }
    }

    public static void SyncedObjectVec3ScaleUpdate(int[] _syncedObjectUUIDs, Vector3[] _scales) {
        using (Packet _packet = new Packet((int)ServerPackets.SyncedObjectVec3ScaleUpdate)) {
            _packet.Write(_syncedObjectUUIDs);
            _packet.Write(_scales);

            SendUDPDataToAll(_packet);
        }
    }

    public static void SyncedObjectVec2PosInterpolation(int[] _syncedObjectUUIDs, Vector2[] _interpolatePositions) {
        using (Packet _packet = new Packet((int)ServerPackets.SyncedObjectVec2PosInterpolation)) {
            _packet.Write(_syncedObjectUUIDs);
            _packet.Write(_interpolatePositions);

            SendUDPDataToAll(_packet);
        }
    }

    public static void SyncedObjectVec3PosInterpolation(int[] _syncedObjectUUIDs, Vector3[] _interpolatePositions) {
        using (Packet _packet = new Packet((int)ServerPackets.SyncedObjectVec3PosInterpolation)) {
            _packet.Write(_syncedObjectUUIDs);
            _packet.Write(_interpolatePositions);

            SendUDPDataToAll(_packet);
        }
    }

    public static void SyncedObjectRotZInterpolation(int[] _syncedObjectUUIDs, float[] _interpolateRotations) {
        using (Packet _packet = new Packet((int)ServerPackets.SyncedObjectRotZInterpolation)) {
            _packet.Write(_syncedObjectUUIDs);
            _packet.Write(_interpolateRotations);

            SendUDPDataToAll(_packet);
        }
    }

    public static void SyncedObjectRotInterpolation(int[] _syncedObjectUUIDs, Vector3[] _interpolateRotations) {
        using (Packet _packet = new Packet((int)ServerPackets.SyncedObjectRotInterpolation)) {
            _packet.Write(_syncedObjectUUIDs);
            _packet.Write(_interpolateRotations);

            SendUDPDataToAll(_packet);
        }
    }

    public static void SyncedObjectVec2ScaleInterpolation(int[] _syncedObjectUUIDs, Vector2[] _interpolateScales) {
        using (Packet _packet = new Packet((int)ServerPackets.SyncedObjectVec2ScaleInterpolation)) {
            _packet.Write(_syncedObjectUUIDs);
            _packet.Write(_interpolateScales);

            SendUDPDataToAll(_packet);
        }
    }

    public static void SyncedObjectVec3ScaleInterpolation(int[] _syncedObjectUUIDs, Vector3[] _interpolateScales) {
        using (Packet _packet = new Packet((int)ServerPackets.SyncedObjectVec3ScaleInterpolation)) {
            _packet.Write(_syncedObjectUUIDs);
            _packet.Write(_interpolateScales);

            SendUDPDataToAll(_packet);
        }
    }
}

#endregion

#endregion Packets

#region Callbacks

public static class USNLCallbackEvents {
    public delegate void USNLCallbackEvent(object _param);

    public static USNLCallbackEvent[] PacketCallbackEvents = {
        CallOnWelcomeReceivedPacketCallbacks,
        CallOnPingPacketCallbacks,
        CallOnClientInputPacketCallbacks,
    };

    public static event USNLCallbackEvent OnServerStarted;
    public static event USNLCallbackEvent OnServerStopped;
    public static event USNLCallbackEvent OnClientConnected;
    public static event USNLCallbackEvent OnClientDisconnected;

    public static event USNLCallbackEvent OnWelcomeReceivedPacket;
    public static event USNLCallbackEvent OnPingPacket;
    public static event USNLCallbackEvent OnClientInputPacket;

    public static void CallOnServerStartedCallbacks(object _param) { if (OnServerStarted != null) { OnServerStarted(_param); } }
    public static void CallOnServerStoppedCallbacks(object _param) { if (OnServerStopped != null) { OnServerStopped(_param); } }
    public static void CallOnClientConnectedCallbacks(object _param) { if (OnClientConnected != null) { OnClientConnected(_param); } }
    public static void CallOnClientDisconnectedCallbacks(object _param) { if (OnClientDisconnected != null) { OnClientDisconnected(_param); } }

    public static void CallOnWelcomeReceivedPacketCallbacks(object _param) { if (OnWelcomeReceivedPacket != null) { OnWelcomeReceivedPacket(_param); } }
    public static void CallOnPingPacketCallbacks(object _param) { if (OnPingPacket != null) { OnPingPacket(_param); } }
    public static void CallOnClientInputPacketCallbacks(object _param) { if (OnClientInputPacket != null) { OnClientInputPacket(_param); } }
}

#endregion
