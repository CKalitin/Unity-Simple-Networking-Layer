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
    Test,
}

// Sent from Client to Server
public enum ClientPackets {
    WelcomeReceived,
    Ping,
    ClientInput,
    Test,
}

#endregion

#region Packet Structs

public struct WelcomePacket {
    private string welcomeMessage;
    private string serverName;
    private int clientId;

    public WelcomePacket(string _welcomeMessage, string _serverName, int _clientId) {
        welcomeMessage = _welcomeMessage;
        serverName = _serverName;
        clientId = _clientId;
    }

    public string WelcomeMessage { get => welcomeMessage; set => welcomeMessage = value; }
    public string ServerName { get => serverName; set => serverName = value; }
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

public struct SyncedObjectInterpolationModePacket {
    private bool serverInterpolation;

    public SyncedObjectInterpolationModePacket(bool _serverInterpolation) {
        serverInterpolation = _serverInterpolation;
    }

    public bool ServerInterpolation { get => serverInterpolation; set => serverInterpolation = value; }
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
    private Vector3[] rotations;

    public SyncedObjectRotUpdatePacket(int[] _syncedObjectUUIDs, Vector3[] _rotations) {
        syncedObjectUUIDs = _syncedObjectUUIDs;
        rotations = _rotations;
    }

    public int[] SyncedObjectUUIDs { get => syncedObjectUUIDs; set => syncedObjectUUIDs = value; }
    public Vector3[] Rotations { get => rotations; set => rotations = value; }
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

public struct SyncedObjectVec2PosInterpolationPacket {
    private int[] syncedObjectUUIDs;
    private Vector2[] interpolatePositions;

    public SyncedObjectVec2PosInterpolationPacket(int[] _syncedObjectUUIDs, Vector2[] _interpolatePositions) {
        syncedObjectUUIDs = _syncedObjectUUIDs;
        interpolatePositions = _interpolatePositions;
    }

    public int[] SyncedObjectUUIDs { get => syncedObjectUUIDs; set => syncedObjectUUIDs = value; }
    public Vector2[] InterpolatePositions { get => interpolatePositions; set => interpolatePositions = value; }
}

public struct SyncedObjectVec3PosInterpolationPacket {
    private int[] syncedObjectUUIDs;
    private Vector3[] interpolatePositions;

    public SyncedObjectVec3PosInterpolationPacket(int[] _syncedObjectUUIDs, Vector3[] _interpolatePositions) {
        syncedObjectUUIDs = _syncedObjectUUIDs;
        interpolatePositions = _interpolatePositions;
    }

    public int[] SyncedObjectUUIDs { get => syncedObjectUUIDs; set => syncedObjectUUIDs = value; }
    public Vector3[] InterpolatePositions { get => interpolatePositions; set => interpolatePositions = value; }
}

public struct SyncedObjectRotZInterpolationPacket {
    private int[] syncedObjectUUIDs;
    private float[] interpolateRotations;

    public SyncedObjectRotZInterpolationPacket(int[] _syncedObjectUUIDs, float[] _interpolateRotations) {
        syncedObjectUUIDs = _syncedObjectUUIDs;
        interpolateRotations = _interpolateRotations;
    }

    public int[] SyncedObjectUUIDs { get => syncedObjectUUIDs; set => syncedObjectUUIDs = value; }
    public float[] InterpolateRotations { get => interpolateRotations; set => interpolateRotations = value; }
}

public struct SyncedObjectRotInterpolationPacket {
    private int[] syncedObjectUUIDs;
    private Vector3[] interpolateRotations;

    public SyncedObjectRotInterpolationPacket(int[] _syncedObjectUUIDs, Vector3[] _interpolateRotations) {
        syncedObjectUUIDs = _syncedObjectUUIDs;
        interpolateRotations = _interpolateRotations;
    }

    public int[] SyncedObjectUUIDs { get => syncedObjectUUIDs; set => syncedObjectUUIDs = value; }
    public Vector3[] InterpolateRotations { get => interpolateRotations; set => interpolateRotations = value; }
}

public struct SyncedObjectVec2ScaleInterpolationPacket {
    private int[] syncedObjectUUIDs;
    private Vector2[] interpolateScales;

    public SyncedObjectVec2ScaleInterpolationPacket(int[] _syncedObjectUUIDs, Vector2[] _interpolateScales) {
        syncedObjectUUIDs = _syncedObjectUUIDs;
        interpolateScales = _interpolateScales;
    }

    public int[] SyncedObjectUUIDs { get => syncedObjectUUIDs; set => syncedObjectUUIDs = value; }
    public Vector2[] InterpolateScales { get => interpolateScales; set => interpolateScales = value; }
}

public struct SyncedObjectVec3ScaleInterpolationPacket {
    private int[] syncedObjectUUIDs;
    private Vector3[] interpolateScales;

    public SyncedObjectVec3ScaleInterpolationPacket(int[] _syncedObjectUUIDs, Vector3[] _interpolateScales) {
        syncedObjectUUIDs = _syncedObjectUUIDs;
        interpolateScales = _interpolateScales;
    }

    public int[] SyncedObjectUUIDs { get => syncedObjectUUIDs; set => syncedObjectUUIDs = value; }
    public Vector3[] InterpolateScales { get => interpolateScales; set => interpolateScales = value; }
}

public struct TestPacket {
    private short testt;

    public TestPacket(short _testt) {
        testt = _testt;
    }

    public short Testt { get => testt; set => testt = value; }
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
        { SyncedObjectInterpolationMode },
        { SyncedObjectVec2PosUpdate },
        { SyncedObjectVec3PosUpdate },
        { SyncedObjectRotZUpdate },
        { SyncedObjectRotUpdate },
        { SyncedObjectVec2ScaleUpdate },
        { SyncedObjectVec3ScaleUpdate },
        { SyncedObjectVec2PosInterpolation },
        { SyncedObjectVec3PosInterpolation },
        { SyncedObjectRotZInterpolation },
        { SyncedObjectRotInterpolation },
        { SyncedObjectVec2ScaleInterpolation },
        { SyncedObjectVec3ScaleInterpolation },
        { Test },
    };

    public static void Welcome(Packet _packet) {
        string welcomeMessage = _packet.ReadString();
        string serverName = _packet.ReadString();
        int clientId = _packet.ReadInt();

        WelcomePacket welcomePacket = new WelcomePacket(welcomeMessage, serverName, clientId);
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

    public static void SyncedObjectInterpolationMode(Packet _packet) {
        bool serverInterpolation = _packet.ReadBool();

        SyncedObjectInterpolationModePacket syncedObjectInterpolationModePacket = new SyncedObjectInterpolationModePacket(serverInterpolation);
        PacketManager.instance.PacketReceived(_packet, syncedObjectInterpolationModePacket);
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
        Vector3[] rotations = _packet.ReadVector3s();

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

    public static void SyncedObjectVec2PosInterpolation(Packet _packet) {
        int[] syncedObjectUUIDs = _packet.ReadInts();
        Vector2[] interpolatePositions = _packet.ReadVector2s();

        SyncedObjectVec2PosInterpolationPacket syncedObjectVec2PosInterpolationPacket = new SyncedObjectVec2PosInterpolationPacket(syncedObjectUUIDs, interpolatePositions);
        PacketManager.instance.PacketReceived(_packet, syncedObjectVec2PosInterpolationPacket);
    }

    public static void SyncedObjectVec3PosInterpolation(Packet _packet) {
        int[] syncedObjectUUIDs = _packet.ReadInts();
        Vector3[] interpolatePositions = _packet.ReadVector3s();

        SyncedObjectVec3PosInterpolationPacket syncedObjectVec3PosInterpolationPacket = new SyncedObjectVec3PosInterpolationPacket(syncedObjectUUIDs, interpolatePositions);
        PacketManager.instance.PacketReceived(_packet, syncedObjectVec3PosInterpolationPacket);
    }

    public static void SyncedObjectRotZInterpolation(Packet _packet) {
        int[] syncedObjectUUIDs = _packet.ReadInts();
        float[] interpolateRotations = _packet.ReadFloats();

        SyncedObjectRotZInterpolationPacket syncedObjectRotZInterpolationPacket = new SyncedObjectRotZInterpolationPacket(syncedObjectUUIDs, interpolateRotations);
        PacketManager.instance.PacketReceived(_packet, syncedObjectRotZInterpolationPacket);
    }

    public static void SyncedObjectRotInterpolation(Packet _packet) {
        int[] syncedObjectUUIDs = _packet.ReadInts();
        Vector3[] interpolateRotations = _packet.ReadVector3s();

        SyncedObjectRotInterpolationPacket syncedObjectRotInterpolationPacket = new SyncedObjectRotInterpolationPacket(syncedObjectUUIDs, interpolateRotations);
        PacketManager.instance.PacketReceived(_packet, syncedObjectRotInterpolationPacket);
    }

    public static void SyncedObjectVec2ScaleInterpolation(Packet _packet) {
        int[] syncedObjectUUIDs = _packet.ReadInts();
        Vector2[] interpolateScales = _packet.ReadVector2s();

        SyncedObjectVec2ScaleInterpolationPacket syncedObjectVec2ScaleInterpolationPacket = new SyncedObjectVec2ScaleInterpolationPacket(syncedObjectUUIDs, interpolateScales);
        PacketManager.instance.PacketReceived(_packet, syncedObjectVec2ScaleInterpolationPacket);
    }

    public static void SyncedObjectVec3ScaleInterpolation(Packet _packet) {
        int[] syncedObjectUUIDs = _packet.ReadInts();
        Vector3[] interpolateScales = _packet.ReadVector3s();

        SyncedObjectVec3ScaleInterpolationPacket syncedObjectVec3ScaleInterpolationPacket = new SyncedObjectVec3ScaleInterpolationPacket(syncedObjectUUIDs, interpolateScales);
        PacketManager.instance.PacketReceived(_packet, syncedObjectVec3ScaleInterpolationPacket);
    }

    public static void Test(Packet _packet) {
        short testt = _packet.ReadShort();

        TestPacket testPacket = new TestPacket(testt);
        PacketManager.instance.PacketReceived(_packet, testPacket);
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

    public static void ClientInput(int[] _keycodesDown, int[] _keycodesUp) {
        using (Packet _packet = new Packet((int)ClientPackets.ClientInput)) {
            _packet.Write(_keycodesDown);
            _packet.Write(_keycodesUp);

            SendTCPData(_packet);
        }
    }

    public static void Test(short _testt) {
        using (Packet _packet = new Packet((int)ClientPackets.Test)) {
            _packet.Write(_testt);

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
        CallOnSyncedObjectInterpolationModePacketCallbacks,
        CallOnSyncedObjectVec2PosUpdatePacketCallbacks,
        CallOnSyncedObjectVec3PosUpdatePacketCallbacks,
        CallOnSyncedObjectRotZUpdatePacketCallbacks,
        CallOnSyncedObjectRotUpdatePacketCallbacks,
        CallOnSyncedObjectVec2ScaleUpdatePacketCallbacks,
        CallOnSyncedObjectVec3ScaleUpdatePacketCallbacks,
        CallOnSyncedObjectVec2PosInterpolationPacketCallbacks,
        CallOnSyncedObjectVec3PosInterpolationPacketCallbacks,
        CallOnSyncedObjectRotZInterpolationPacketCallbacks,
        CallOnSyncedObjectRotInterpolationPacketCallbacks,
        CallOnSyncedObjectVec2ScaleInterpolationPacketCallbacks,
        CallOnSyncedObjectVec3ScaleInterpolationPacketCallbacks,
        CallOnTestPacketCallbacks,
    };

    public static event USNLCallbackEvent OnConnected;
    public static event USNLCallbackEvent OnDisconnected;

    public static event USNLCallbackEvent OnWelcomePacket;
    public static event USNLCallbackEvent OnPingPacket;
    public static event USNLCallbackEvent OnSyncedObjectInstantiatePacket;
    public static event USNLCallbackEvent OnSyncedObjectDestroyPacket;
    public static event USNLCallbackEvent OnSyncedObjectInterpolationModePacket;
    public static event USNLCallbackEvent OnSyncedObjectVec2PosUpdatePacket;
    public static event USNLCallbackEvent OnSyncedObjectVec3PosUpdatePacket;
    public static event USNLCallbackEvent OnSyncedObjectRotZUpdatePacket;
    public static event USNLCallbackEvent OnSyncedObjectRotUpdatePacket;
    public static event USNLCallbackEvent OnSyncedObjectVec2ScaleUpdatePacket;
    public static event USNLCallbackEvent OnSyncedObjectVec3ScaleUpdatePacket;
    public static event USNLCallbackEvent OnSyncedObjectVec2PosInterpolationPacket;
    public static event USNLCallbackEvent OnSyncedObjectVec3PosInterpolationPacket;
    public static event USNLCallbackEvent OnSyncedObjectRotZInterpolationPacket;
    public static event USNLCallbackEvent OnSyncedObjectRotInterpolationPacket;
    public static event USNLCallbackEvent OnSyncedObjectVec2ScaleInterpolationPacket;
    public static event USNLCallbackEvent OnSyncedObjectVec3ScaleInterpolationPacket;
    public static event USNLCallbackEvent OnTestPacket;

    public static void CallOnConnectedCallbacks(object _param) { if (OnConnected != null) { OnConnected(_param); } }
    public static void CallOnDisconnectedCallbacks(object _param) { if (OnDisconnected != null) { OnDisconnected(_param); } }

    public static void CallOnWelcomePacketCallbacks(object _param) { if (OnWelcomePacket != null) { OnWelcomePacket(_param); } }
    public static void CallOnPingPacketCallbacks(object _param) { if (OnPingPacket != null) { OnPingPacket(_param); } }
    public static void CallOnSyncedObjectInstantiatePacketCallbacks(object _param) { if (OnSyncedObjectInstantiatePacket != null) { OnSyncedObjectInstantiatePacket(_param); } }
    public static void CallOnSyncedObjectDestroyPacketCallbacks(object _param) { if (OnSyncedObjectDestroyPacket != null) { OnSyncedObjectDestroyPacket(_param); } }
    public static void CallOnSyncedObjectInterpolationModePacketCallbacks(object _param) { if (OnSyncedObjectInterpolationModePacket != null) { OnSyncedObjectInterpolationModePacket(_param); } }
    public static void CallOnSyncedObjectVec2PosUpdatePacketCallbacks(object _param) { if (OnSyncedObjectVec2PosUpdatePacket != null) { OnSyncedObjectVec2PosUpdatePacket(_param); } }
    public static void CallOnSyncedObjectVec3PosUpdatePacketCallbacks(object _param) { if (OnSyncedObjectVec3PosUpdatePacket != null) { OnSyncedObjectVec3PosUpdatePacket(_param); } }
    public static void CallOnSyncedObjectRotZUpdatePacketCallbacks(object _param) { if (OnSyncedObjectRotZUpdatePacket != null) { OnSyncedObjectRotZUpdatePacket(_param); } }
    public static void CallOnSyncedObjectRotUpdatePacketCallbacks(object _param) { if (OnSyncedObjectRotUpdatePacket != null) { OnSyncedObjectRotUpdatePacket(_param); } }
    public static void CallOnSyncedObjectVec2ScaleUpdatePacketCallbacks(object _param) { if (OnSyncedObjectVec2ScaleUpdatePacket != null) { OnSyncedObjectVec2ScaleUpdatePacket(_param); } }
    public static void CallOnSyncedObjectVec3ScaleUpdatePacketCallbacks(object _param) { if (OnSyncedObjectVec3ScaleUpdatePacket != null) { OnSyncedObjectVec3ScaleUpdatePacket(_param); } }
    public static void CallOnSyncedObjectVec2PosInterpolationPacketCallbacks(object _param) { if (OnSyncedObjectVec2PosInterpolationPacket != null) { OnSyncedObjectVec2PosInterpolationPacket(_param); } }
    public static void CallOnSyncedObjectVec3PosInterpolationPacketCallbacks(object _param) { if (OnSyncedObjectVec3PosInterpolationPacket != null) { OnSyncedObjectVec3PosInterpolationPacket(_param); } }
    public static void CallOnSyncedObjectRotZInterpolationPacketCallbacks(object _param) { if (OnSyncedObjectRotZInterpolationPacket != null) { OnSyncedObjectRotZInterpolationPacket(_param); } }
    public static void CallOnSyncedObjectRotInterpolationPacketCallbacks(object _param) { if (OnSyncedObjectRotInterpolationPacket != null) { OnSyncedObjectRotInterpolationPacket(_param); } }
    public static void CallOnSyncedObjectVec2ScaleInterpolationPacketCallbacks(object _param) { if (OnSyncedObjectVec2ScaleInterpolationPacket != null) { OnSyncedObjectVec2ScaleInterpolationPacket(_param); } }
    public static void CallOnSyncedObjectVec3ScaleInterpolationPacketCallbacks(object _param) { if (OnSyncedObjectVec3ScaleInterpolationPacket != null) { OnSyncedObjectVec3ScaleInterpolationPacket(_param); } }
    public static void CallOnTestPacketCallbacks(object _param) { if (OnTestPacket != null) { OnTestPacket(_param); } }
}

#endregion
