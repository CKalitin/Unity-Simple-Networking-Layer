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
    SyncedObjectVec2PosUpdate,
    SyncedObjectVec3PosUpdate,
    SyncedObjectRotZUpdate,
    SyncedObjectRotUpdate,
    SyncedObjectVec2ScaleUpdate,
    SyncedObjectVec3ScaleUpdate,
}

// Sent from Client to Server
public enum ClientPackets {
    WelcomeReceived,
    Ping,
    ClientInput,
}

#endregion

#region Packet Structs

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

public struct PingPacket {
    private bool sendPingBack;

    public PingPacket(bool _sendPingBack) {
        sendPingBack = _sendPingBack;
    }

    public bool SendPingBack { get => sendPingBack; set => sendPingBack = value; }
}

public struct SyncedObjectInstantiatePacket {
    private int syncedObjectPrefebId;
    private int syncedObjectUUID;
    private Vector3 position;
    private Quaternion rotation;
    private Vector3 scale;

    public SyncedObjectInstantiatePacket(int _syncedObjectPrefebId, int _syncedObjectUUID, Vector3 _position, Quaternion _rotation, Vector3 _scale) {
        syncedObjectPrefebId = _syncedObjectPrefebId;
        syncedObjectUUID = _syncedObjectUUID;
        position = _position;
        rotation = _rotation;
        scale = _scale;
    }

    public int SyncedObjectPrefebId { get => syncedObjectPrefebId; set => syncedObjectPrefebId = value; }
    public int SyncedObjectUUID { get => syncedObjectUUID; set => syncedObjectUUID = value; }
    public Vector3 Position { get => position; set => position = value; }
    public Quaternion Rotation { get => rotation; set => rotation = value; }
    public Vector3 Scale { get => scale; set => scale = value; }
}

public struct SyncedObjectDestroyPacket {
    private int syncedObjectUUID;

    public SyncedObjectDestroyPacket(int _syncedObjectUUID) {
        syncedObjectUUID = _syncedObjectUUID;
    }

    public int SyncedObjectUUID { get => syncedObjectUUID; set => syncedObjectUUID = value; }
}

public struct SyncedObjectVec2PosUpdatePacket {
    private int[] syncedObjectUUIDs;
    private Vector2[] positions;

    public SyncedObjectVec2PosUpdatePacket(int[] _syncedObjectUUIDs, Vector2[] _positions) {
        syncedObjectUUIDs = _syncedObjectUUIDs;
        positions = _positions;
    }

    public int[] SyncedObjectUUIDs { get => syncedObjectUUIDs; set => syncedObjectUUIDs = value; }
    public Vector2[] Positions { get => positions; set => positions = value; }
}

public struct SyncedObjectVec3PosUpdatePacket {
    private int[] syncedObjectUUIDs;
    private Vector3[] positions;

    public SyncedObjectVec3PosUpdatePacket(int[] _syncedObjectUUIDs, Vector3[] _positions) {
        syncedObjectUUIDs = _syncedObjectUUIDs;
        positions = _positions;
    }

    public int[] SyncedObjectUUIDs { get => syncedObjectUUIDs; set => syncedObjectUUIDs = value; }
    public Vector3[] Positions { get => positions; set => positions = value; }
}

public struct SyncedObjectRotZUpdatePacket {
    private int[] syncedObjectUUIDs;
    private float[] rotations;

    public SyncedObjectRotZUpdatePacket(int[] _syncedObjectUUIDs, float[] _rotations) {
        syncedObjectUUIDs = _syncedObjectUUIDs;
        rotations = _rotations;
    }

    public int[] SyncedObjectUUIDs { get => syncedObjectUUIDs; set => syncedObjectUUIDs = value; }
    public float[] Rotations { get => rotations; set => rotations = value; }
}

public struct SyncedObjectRotUpdatePacket {
    private int[] syncedObjectUUIDs;
    private Quaternion[] rotations;

    public SyncedObjectRotUpdatePacket(int[] _syncedObjectUUIDs, Quaternion[] _rotations) {
        syncedObjectUUIDs = _syncedObjectUUIDs;
        rotations = _rotations;
    }

    public int[] SyncedObjectUUIDs { get => syncedObjectUUIDs; set => syncedObjectUUIDs = value; }
    public Quaternion[] Rotations { get => rotations; set => rotations = value; }
}

public struct SyncedObjectVec2ScaleUpdatePacket {
    private int[] syncedObjectUUIDs;
    private Vector2[] scales;

    public SyncedObjectVec2ScaleUpdatePacket(int[] _syncedObjectUUIDs, Vector2[] _scales) {
        syncedObjectUUIDs = _syncedObjectUUIDs;
        scales = _scales;
    }

    public int[] SyncedObjectUUIDs { get => syncedObjectUUIDs; set => syncedObjectUUIDs = value; }
    public Vector2[] Scales { get => scales; set => scales = value; }
}

public struct SyncedObjectVec3ScaleUpdatePacket {
    private int[] syncedObjectUUIDs;
    private Vector3[] scales;

    public SyncedObjectVec3ScaleUpdatePacket(int[] _syncedObjectUUIDs, Vector3[] _scales) {
        syncedObjectUUIDs = _syncedObjectUUIDs;
        scales = _scales;
    }

    public int[] SyncedObjectUUIDs { get => syncedObjectUUIDs; set => syncedObjectUUIDs = value; }
    public Vector3[] Scales { get => scales; set => scales = value; }
}

#endregion

#region Packet Handlers

public static class PacketHandlers {
    public delegate void PacketHandler(Packet _packet);
    public static List<PacketHandler> packetHandlers = new List<PacketHandler>() {
        { Welcome },
        { Ping },
        { SyncedObjectInstantiate },
        { SyncedObjectDestroy },
        { SyncedObjectVec2PosUpdate },
        { SyncedObjectVec3PosUpdate },
        { SyncedObjectRotZUpdate },
        { SyncedObjectRotUpdate },
        { SyncedObjectVec2ScaleUpdate },
        { SyncedObjectVec3ScaleUpdate },
    };

    public static void Welcome(Packet _packet) {
        string welcomeMessage = _packet.ReadString();
        int clientId = _packet.ReadInt();

        WelcomePacket welcomePacket = new WelcomePacket(welcomeMessage, clientId);
        PacketManager.instance.PacketReceived(_packet, welcomePacket);
    }

    public static void Ping(Packet _packet) {
        bool sendPingBack = _packet.ReadBool();

        PingPacket pingPacket = new PingPacket(sendPingBack);
        PacketManager.instance.PacketReceived(_packet, pingPacket);
    }

    public static void SyncedObjectInstantiate(Packet _packet) {
        int syncedObjectPrefebId = _packet.ReadInt();
        int syncedObjectUUID = _packet.ReadInt();
        Vector3 position = _packet.ReadVector3();
        Quaternion rotation = _packet.ReadQuaternion();
        Vector3 scale = _packet.ReadVector3();

        SyncedObjectInstantiatePacket syncedObjectInstantiatePacket = new SyncedObjectInstantiatePacket(syncedObjectPrefebId, syncedObjectUUID, position, rotation, scale);
        PacketManager.instance.PacketReceived(_packet, syncedObjectInstantiatePacket);
    }

    public static void SyncedObjectDestroy(Packet _packet) {
        int syncedObjectUUID = _packet.ReadInt();

        SyncedObjectDestroyPacket syncedObjectDestroyPacket = new SyncedObjectDestroyPacket(syncedObjectUUID);
        PacketManager.instance.PacketReceived(_packet, syncedObjectDestroyPacket);
    }

    public static void SyncedObjectVec2PosUpdate(Packet _packet) {
        int[] syncedObjectUUIDs = _packet.ReadInts();
        Vector2[] positions = _packet.ReadVector2s();

        SyncedObjectVec2PosUpdatePacket syncedObjectVec2PosUpdatePacket = new SyncedObjectVec2PosUpdatePacket(syncedObjectUUIDs, positions);
        PacketManager.instance.PacketReceived(_packet, syncedObjectVec2PosUpdatePacket);
    }

    public static void SyncedObjectVec3PosUpdate(Packet _packet) {
        int[] syncedObjectUUIDs = _packet.ReadInts();
        Vector3[] positions = _packet.ReadVector3s();

        SyncedObjectVec3PosUpdatePacket syncedObjectVec3PosUpdatePacket = new SyncedObjectVec3PosUpdatePacket(syncedObjectUUIDs, positions);
        PacketManager.instance.PacketReceived(_packet, syncedObjectVec3PosUpdatePacket);
    }

    public static void SyncedObjectRotZUpdate(Packet _packet) {
        int[] syncedObjectUUIDs = _packet.ReadInts();
        float[] rotations = _packet.ReadFloats();

        SyncedObjectRotZUpdatePacket syncedObjectRotZUpdatePacket = new SyncedObjectRotZUpdatePacket(syncedObjectUUIDs, rotations);
        PacketManager.instance.PacketReceived(_packet, syncedObjectRotZUpdatePacket);
    }

    public static void SyncedObjectRotUpdate(Packet _packet) {
        int[] syncedObjectUUIDs = _packet.ReadInts();
        Quaternion[] rotations = _packet.ReadQuaternions();

        SyncedObjectRotUpdatePacket syncedObjectRotUpdatePacket = new SyncedObjectRotUpdatePacket(syncedObjectUUIDs, rotations);
        PacketManager.instance.PacketReceived(_packet, syncedObjectRotUpdatePacket);
    }

    public static void SyncedObjectVec2ScaleUpdate(Packet _packet) {
        int[] syncedObjectUUIDs = _packet.ReadInts();
        Vector2[] scales = _packet.ReadVector2s();

        SyncedObjectVec2ScaleUpdatePacket syncedObjectVec2ScaleUpdatePacket = new SyncedObjectVec2ScaleUpdatePacket(syncedObjectUUIDs, scales);
        PacketManager.instance.PacketReceived(_packet, syncedObjectVec2ScaleUpdatePacket);
    }

    public static void SyncedObjectVec3ScaleUpdate(Packet _packet) {
        int[] syncedObjectUUIDs = _packet.ReadInts();
        Vector3[] scales = _packet.ReadVector3s();

        SyncedObjectVec3ScaleUpdatePacket syncedObjectVec3ScaleUpdatePacket = new SyncedObjectVec3ScaleUpdatePacket(syncedObjectUUIDs, scales);
        PacketManager.instance.PacketReceived(_packet, syncedObjectVec3ScaleUpdatePacket);
    }
}

#endregion

#region Packet Send

public static class PacketSend {
    private static void SendTCPData(Packet _packet) {
        _packet.WriteLength();
        if (Client.instance.IsConnected) {
            Client.instance.Tcp.SendData(_packet);
            NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length());
        }
    }

    private static void SendUDPData(Packet _packet) {
        _packet.WriteLength();
        if (Client.instance.IsConnected) {
            Client.instance.Udp.SendData(_packet);
            NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length());
        }
    }

    public static void WelcomeReceived(int _clientIdCheck) {
        using (Packet _packet = new Packet((int)ClientPackets.WelcomeReceived)) {
            _packet.Write(_clientIdCheck);

            SendTCPData(_packet);
        }
    }

    public static void Ping(bool _sendPingBack) {
        using (Packet _packet = new Packet((int)ClientPackets.Ping)) {
            _packet.Write(_sendPingBack);

            SendTCPData(_packet);
        }
    }

    public static void ClientInput(byte[] _keycodesDown, byte[] _keycodesUp) {
        using (Packet _packet = new Packet((int)ClientPackets.ClientInput)) {
            _packet.Write(_keycodesDown);
            _packet.Write(_keycodesUp);

            SendTCPData(_packet);
        }
    }
}

#endregion

#endregion Packets

#region Callbacks

public static class USNLCallbackEvents {
    public delegate void USNLCallbackEvent(object _param);

    public static USNLCallbackEvent[] PacketCallbackEvents = {
        CallOnWelcomePacketCallbacks,
        CallOnPingPacketCallbacks,
        CallOnSyncedObjectInstantiatePacketCallbacks,
        CallOnSyncedObjectDestroyPacketCallbacks,
        CallOnSyncedObjectVec2PosUpdatePacketCallbacks,
        CallOnSyncedObjectVec3PosUpdatePacketCallbacks,
        CallOnSyncedObjectRotZUpdatePacketCallbacks,
        CallOnSyncedObjectRotUpdatePacketCallbacks,
        CallOnSyncedObjectVec2ScaleUpdatePacketCallbacks,
        CallOnSyncedObjectVec3ScaleUpdatePacketCallbacks,
    };

    public static event USNLCallbackEvent OnConnected;
    public static event USNLCallbackEvent OnDisconnected;

    public static event USNLCallbackEvent OnWelcomePacket;
    public static event USNLCallbackEvent OnPingPacket;
    public static event USNLCallbackEvent OnSyncedObjectInstantiatePacket;
    public static event USNLCallbackEvent OnSyncedObjectDestroyPacket;
    public static event USNLCallbackEvent OnSyncedObjectVec2PosUpdatePacket;
    public static event USNLCallbackEvent OnSyncedObjectVec3PosUpdatePacket;
    public static event USNLCallbackEvent OnSyncedObjectRotZUpdatePacket;
    public static event USNLCallbackEvent OnSyncedObjectRotUpdatePacket;
    public static event USNLCallbackEvent OnSyncedObjectVec2ScaleUpdatePacket;
    public static event USNLCallbackEvent OnSyncedObjectVec3ScaleUpdatePacket;

    public static void CallOnConnectedCallbacks(object _param) { if (OnConnected != null) { OnConnected(_param); } }
    public static void CallOnDisconnectedCallbacks(object _param) { if (OnDisconnected != null) { OnDisconnected(_param); } }

    public static void CallOnWelcomePacketCallbacks(object _param) { if (OnWelcomePacket != null) { OnWelcomePacket(_param); } }
    public static void CallOnPingPacketCallbacks(object _param) { if (OnPingPacket != null) { OnPingPacket(_param); } }
    public static void CallOnSyncedObjectInstantiatePacketCallbacks(object _param) { if (OnSyncedObjectInstantiatePacket != null) { OnSyncedObjectInstantiatePacket(_param); } }
    public static void CallOnSyncedObjectDestroyPacketCallbacks(object _param) { if (OnSyncedObjectDestroyPacket != null) { OnSyncedObjectDestroyPacket(_param); } }
    public static void CallOnSyncedObjectVec2PosUpdatePacketCallbacks(object _param) { if (OnSyncedObjectVec2PosUpdatePacket != null) { OnSyncedObjectVec2PosUpdatePacket(_param); } }
    public static void CallOnSyncedObjectVec3PosUpdatePacketCallbacks(object _param) { if (OnSyncedObjectVec3PosUpdatePacket != null) { OnSyncedObjectVec3PosUpdatePacket(_param); } }
    public static void CallOnSyncedObjectRotZUpdatePacketCallbacks(object _param) { if (OnSyncedObjectRotZUpdatePacket != null) { OnSyncedObjectRotZUpdatePacket(_param); } }
    public static void CallOnSyncedObjectRotUpdatePacketCallbacks(object _param) { if (OnSyncedObjectRotUpdatePacket != null) { OnSyncedObjectRotUpdatePacket(_param); } }
    public static void CallOnSyncedObjectVec2ScaleUpdatePacketCallbacks(object _param) { if (OnSyncedObjectVec2ScaleUpdatePacket != null) { OnSyncedObjectVec2ScaleUpdatePacket(_param); } }
    public static void CallOnSyncedObjectVec3ScaleUpdatePacketCallbacks(object _param) { if (OnSyncedObjectVec3ScaleUpdatePacket != null) { OnSyncedObjectVec3ScaleUpdatePacket(_param); } }
}

#endregion
