using System;
using System.Collections.Generic;
using UnityEngine;

#region Packets

// Sent from Server to Client
public enum ServerPackets {
    Welcome,
    SyncedObjectInstantiate,
    SyncedObjectDestroy,
    SyncedObjectUpdate,
    Ping,
    VariablesTest,
}

// Sent from Client to Server
public enum ClientPackets {
    WelcomeReceived,
    ClientInput,
    Ping,
    VariablesTest,
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

public struct SyncedObjectUpdatePacket {
    private int syncedObjectUUID;
    private Vector3 position;
    private Quaternion rotation;
    private Vector3 scale;

    public SyncedObjectUpdatePacket(int _syncedObjectUUID, Vector3 _position, Quaternion _rotation, Vector3 _scale) {
        syncedObjectUUID = _syncedObjectUUID;
        position = _position;
        rotation = _rotation;
        scale = _scale;
    }

    public int SyncedObjectUUID { get => syncedObjectUUID; set => syncedObjectUUID = value; }
    public Vector3 Position { get => position; set => position = value; }
    public Quaternion Rotation { get => rotation; set => rotation = value; }
    public Vector3 Scale { get => scale; set => scale = value; }
}

public struct PingPacket {
    private bool sendPingBack;

    public PingPacket(bool _sendPingBack) {
        sendPingBack = _sendPingBack;
    }

    public bool SendPingBack { get => sendPingBack; set => sendPingBack = value; }
}

public struct VariablesTestPacket {
    private byte varByte;
    private short varShort;
    private int varInt;
    private long varLong;
    private float varFloat;
    private bool varBool;
    private string varString;
    private Vector2 varVec2;
    private Vector3 varVec3;
    private Quaternion varQuat;
    private byte[] varArrayByte;
    private short[] varArrayShort;
    private int[] varArrayInt;
    private long[] varArrayLong;
    private float[] varArrayFloat;
    private bool[] varArrayBool;
    private string[] varArrayString;
    private Vector2[] varArrayVec2;
    private Vector3[] varArrayVec3;
    private Quaternion[] varArrayQuat;

    public VariablesTestPacket(byte _varByte, short _varShort, int _varInt, long _varLong, float _varFloat, bool _varBool, string _varString, Vector2 _varVec2, Vector3 _varVec3, Quaternion _varQuat, byte[] _varArrayByte, short[] _varArrayShort, int[] _varArrayInt, long[] _varArrayLong, float[] _varArrayFloat, bool[] _varArrayBool, string[] _varArrayString, Vector2[] _varArrayVec2, Vector3[] _varArrayVec3, Quaternion[] _varArrayQuat) {
        varByte = _varByte;
        varShort = _varShort;
        varInt = _varInt;
        varLong = _varLong;
        varFloat = _varFloat;
        varBool = _varBool;
        varString = _varString;
        varVec2 = _varVec2;
        varVec3 = _varVec3;
        varQuat = _varQuat;
        varArrayByte = _varArrayByte;
        varArrayShort = _varArrayShort;
        varArrayInt = _varArrayInt;
        varArrayLong = _varArrayLong;
        varArrayFloat = _varArrayFloat;
        varArrayBool = _varArrayBool;
        varArrayString = _varArrayString;
        varArrayVec2 = _varArrayVec2;
        varArrayVec3 = _varArrayVec3;
        varArrayQuat = _varArrayQuat;
    }

    public byte VarByte { get => varByte; set => varByte = value; }
    public short VarShort { get => varShort; set => varShort = value; }
    public int VarInt { get => varInt; set => varInt = value; }
    public long VarLong { get => varLong; set => varLong = value; }
    public float VarFloat { get => varFloat; set => varFloat = value; }
    public bool VarBool { get => varBool; set => varBool = value; }
    public string VarString { get => varString; set => varString = value; }
    public Vector2 VarVec2 { get => varVec2; set => varVec2 = value; }
    public Vector3 VarVec3 { get => varVec3; set => varVec3 = value; }
    public Quaternion VarQuat { get => varQuat; set => varQuat = value; }
    public byte[] VarArrayByte { get => varArrayByte; set => varArrayByte = value; }
    public short[] VarArrayShort { get => varArrayShort; set => varArrayShort = value; }
    public int[] VarArrayInt { get => varArrayInt; set => varArrayInt = value; }
    public long[] VarArrayLong { get => varArrayLong; set => varArrayLong = value; }
    public float[] VarArrayFloat { get => varArrayFloat; set => varArrayFloat = value; }
    public bool[] VarArrayBool { get => varArrayBool; set => varArrayBool = value; }
    public string[] VarArrayString { get => varArrayString; set => varArrayString = value; }
    public Vector2[] VarArrayVec2 { get => varArrayVec2; set => varArrayVec2 = value; }
    public Vector3[] VarArrayVec3 { get => varArrayVec3; set => varArrayVec3 = value; }
    public Quaternion[] VarArrayQuat { get => varArrayQuat; set => varArrayQuat = value; }
}

public static class PacketHandlers {
    public delegate void PacketHandler(Packet _packet);
    public static List<PacketHandler> packetHandlers = new List<PacketHandler>() {
        { Welcome },
        { SyncedObjectInstantiate },
        { SyncedObjectDestroy },
        { SyncedObjectUpdate },
        { Ping },
        { VariablesTest },
    };

    public static void Welcome(Packet _packet) {
        string welcomeMessage = _packet.ReadString();
        int clientId = _packet.ReadInt();

        WelcomePacket welcomePacket = new WelcomePacket(welcomeMessage, clientId);
        PacketManager.instance.PacketReceived(_packet, welcomePacket);
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

    public static void SyncedObjectUpdate(Packet _packet) {
        int syncedObjectUUID = _packet.ReadInt();
        Vector3 position = _packet.ReadVector3();
        Quaternion rotation = _packet.ReadQuaternion();
        Vector3 scale = _packet.ReadVector3();

        SyncedObjectUpdatePacket syncedObjectUpdatePacket = new SyncedObjectUpdatePacket(syncedObjectUUID, position, rotation, scale);
        PacketManager.instance.PacketReceived(_packet, syncedObjectUpdatePacket);
    }

    public static void Ping(Packet _packet) {
        bool sendPingBack = _packet.ReadBool();

        PingPacket pingPacket = new PingPacket(sendPingBack);
        PacketManager.instance.PacketReceived(_packet, pingPacket);
    }

    public static void VariablesTest(Packet _packet) {
        Debug.Log("Handle: " + _packet.Length());

        byte varByte = _packet.ReadByte();
        short varShort = _packet.ReadShort();
        int varInt = _packet.ReadInt();
        long varLong = _packet.ReadLong();
        float varFloat = _packet.ReadFloat();
        bool varBool = _packet.ReadBool();
        string varString = _packet.ReadString();
        Vector2 varVec2 = _packet.ReadVector2();
        Vector3 varVec3 = _packet.ReadVector3();
        Quaternion varQuat = _packet.ReadQuaternion();
        byte[] varArrayByte = _packet.ReadBytes();
        short[] varArrayShort = _packet.ReadShorts();
        int[] varArrayInt = _packet.ReadInts();
        long[] varArrayLong = _packet.ReadLongs();
        float[] varArrayFloat = _packet.ReadFloats();
        bool[] varArrayBool = _packet.ReadBools();
        string[] varArrayString = _packet.ReadStrings();
        Vector2[] varArrayVec2 = _packet.ReadVector2s();
        Vector3[] varArrayVec3 = _packet.ReadVector3s();
        Quaternion[] varArrayQuat = _packet.ReadQuaternions();

        VariablesTestPacket variablesTestPacket = new VariablesTestPacket(varByte, varShort, varInt, varLong, varFloat, varBool, varString, varVec2, varVec3, varQuat, varArrayByte, varArrayShort, varArrayInt, varArrayLong, varArrayFloat, varArrayBool, varArrayString, varArrayVec2, varArrayVec3, varArrayQuat);
        PacketManager.instance.PacketReceived(_packet, variablesTestPacket);
    }
}

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

    public static void ClientInput(byte[] _keycodesDown, byte[] _keycodesUp) {
        using (Packet _packet = new Packet((int)ClientPackets.ClientInput)) {
            _packet.Write(_keycodesDown);
            _packet.Write(_keycodesUp);

            SendTCPData(_packet);
        }
    }

    public static void Ping(bool _sendPingBack) {
        using (Packet _packet = new Packet((int)ClientPackets.Ping)) {
            _packet.Write(_sendPingBack);

            SendTCPData(_packet);
        }
    }

    public static void VariablesTest(byte _varByte, short _varShort, int _varInt, long _varLong, float _varFloat, bool _varBool, string _varString, Vector2 _varVec2, Vector3 _varVec3, Quaternion _varQuat, byte[] _varArrayByte, short[] _varArrayShort, int[] _varArrayInt, long[] _varArrayLong, float[] _varArrayFloat, bool[] _varArrayBool, string[] _varArrayString, Vector2[] _varArrayVec2, Vector3[] _varArrayVec3, Quaternion[] _varArrayQuat) {
        using (Packet _packet = new Packet((int)ClientPackets.VariablesTest)) {
            _packet.Write(_varByte);
            _packet.Write(_varShort);
            _packet.Write(_varInt);
            _packet.Write(_varLong);
            _packet.Write(_varFloat);
            _packet.Write(_varBool);
            _packet.Write(_varString);
            _packet.Write(_varVec2);
            _packet.Write(_varVec3);
            _packet.Write(_varQuat);
            _packet.Write(_varArrayByte);
            _packet.Write(_varArrayShort);
            _packet.Write(_varArrayInt);
            _packet.Write(_varArrayLong);
            _packet.Write(_varArrayFloat);
            _packet.Write(_varArrayBool);
            _packet.Write(_varArrayString);
            _packet.Write(_varArrayVec2);
            _packet.Write(_varArrayVec3);
            _packet.Write(_varArrayQuat);

            SendTCPData(_packet);

            Debug.Log("Send: " + _packet.Length());
        }
    }
}

#endregion Packets

#region Callbacks

public static class USNLCallbackEvents {
    public delegate void USNLCallbackEvent(object _param);

    public static USNLCallbackEvent[] PacketCallbackEvents = {
        CallOnWelcomePacketCallbacks,
        CallOnSyncedObjectInstantiatePacketCallbacks,
        CallOnSyncedObjectDestroyPacketCallbacks,
        CallOnSyncedObjectUpdatePacketCallbacks,
        CallOnPingPacketCallbacks,
        CallOnVariablesTestPacketCallbacks,
    };

    public static event USNLCallbackEvent OnConnected;
    public static event USNLCallbackEvent OnDisconnected;

    public static event USNLCallbackEvent OnWelcomePacket;
    public static event USNLCallbackEvent OnSyncedObjectInstantiatePacket;
    public static event USNLCallbackEvent OnSyncedObjectDestroyPacket;
    public static event USNLCallbackEvent OnSyncedObjectUpdatePacket;
    public static event USNLCallbackEvent OnPingPacket;
    public static event USNLCallbackEvent OnVariablesTestPacket;

    public static void CallOnConnectedCallbacks(object _param) { if (OnConnected != null) { OnConnected(_param); } }
    public static void CallOnDisconnectedCallbacks(object _param) { if (OnDisconnected != null) { OnDisconnected(_param); } }

    public static void CallOnWelcomePacketCallbacks(object _param) { if (OnWelcomePacket != null) { OnWelcomePacket(_param); } }
    public static void CallOnSyncedObjectInstantiatePacketCallbacks(object _param) { if (OnSyncedObjectInstantiatePacket != null) { OnSyncedObjectInstantiatePacket(_param); } }
    public static void CallOnSyncedObjectDestroyPacketCallbacks(object _param) { if (OnSyncedObjectDestroyPacket != null) { OnSyncedObjectDestroyPacket(_param); } }
    public static void CallOnSyncedObjectUpdatePacketCallbacks(object _param) { if (OnSyncedObjectUpdatePacket != null) { OnSyncedObjectUpdatePacket(_param); } }
    public static void CallOnPingPacketCallbacks(object _param) { if (OnPingPacket != null) { OnPingPacket(_param); } }
    public static void CallOnVariablesTestPacketCallbacks(object _param) { if (OnVariablesTestPacket != null) { OnVariablesTestPacket(_param); } }
}

#endregion
