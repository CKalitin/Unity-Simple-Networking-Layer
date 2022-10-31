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

public struct VariablesTestPacket {
    private int fromClient;

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

    public VariablesTestPacket(int _fromClient, byte _varByte, short _varShort, int _varInt, long _varLong, float _varFloat, bool _varBool, string _varString, Vector2 _varVec2, Vector3 _varVec3, Quaternion _varQuat, byte[] _varArrayByte, short[] _varArrayShort, int[] _varArrayInt, long[] _varArrayLong, float[] _varArrayFloat, bool[] _varArrayBool, string[] _varArrayString, Vector2[] _varArrayVec2, Vector3[] _varArrayVec3, Quaternion[] _varArrayQuat) {
        fromClient = _fromClient;
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

    public int FromClient { get => fromClient; set => fromClient = value; }
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
        { WelcomeReceived },
        { ClientInput },
        { Ping },
        { VariablesTest },
    };

    public static void WelcomeReceived(Packet _packet) {
        int clientIdCheck = _packet.ReadInt();

        WelcomeReceivedPacket welcomeReceivedPacket = new WelcomeReceivedPacket(_packet.FromClient, clientIdCheck);
        PacketManager.instance.PacketReceived(_packet, welcomeReceivedPacket);
    }

    public static void ClientInput(Packet _packet) {
        byte[] keycodesDown = _packet.ReadBytes();
        byte[] keycodesUp = _packet.ReadBytes();

        ClientInputPacket clientInputPacket = new ClientInputPacket(_packet.FromClient, keycodesDown, keycodesUp);
        PacketManager.instance.PacketReceived(_packet, clientInputPacket);
    }

    public static void Ping(Packet _packet) {
        bool sendPingBack = _packet.ReadBool();

        PingPacket pingPacket = new PingPacket(_packet.FromClient, sendPingBack);
        PacketManager.instance.PacketReceived(_packet, pingPacket);
    }

    public static void VariablesTest(Packet _packet) {
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

        VariablesTestPacket variablesTestPacket = new VariablesTestPacket(_packet.FromClient, varByte, varShort, varInt, varLong, varFloat, varBool, varString, varVec2, varVec3, varQuat, varArrayByte, varArrayShort, varArrayInt, varArrayLong, varArrayFloat, varArrayBool, varArrayString, varArrayVec2, varArrayVec3, varArrayQuat);
        PacketManager.instance.PacketReceived(_packet, variablesTestPacket);
    }
}

public static class PacketSend {
    #region TCP & UDP Send Functions

    private static void SendTCPData(int _toClient, Packet _packet) {
        _packet.WriteLength();
        Server.clients[_toClient].Tcp.SendData(_packet);
            if (Server.clients[_toClient].IsConnected) { NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length()); }
    }

    private static void SendTCPDataToAll(Packet _packet) {
        _packet.WriteLength();
        for (int i = 0; i < Server.MaxClients; i++) {
            Server.clients[i].Tcp.SendData(_packet);
            if (Server.clients[i].IsConnected) { NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length()); }
        }
    }

    private static void SendTCPDataToAll(int _excpetClient, Packet _packet) {
        _packet.WriteLength();
        for (int i = 0; i < Server.MaxClients; i++) {
            if (i != _excpetClient) {
                Server.clients[i].Tcp.SendData(_packet);
                if (Server.clients[i].IsConnected) { NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length()); }
            }
        }
    }

    private static void SendUDPData(int _toClient, Packet _packet) {
        _packet.WriteLength();
        Server.clients[_toClient].Udp.SendData(_packet);
        if (Server.clients[_toClient].IsConnected) { NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length()); }
    }

    private static void SendUDPDataToAll(Packet _packet) {
        _packet.WriteLength();
        for (int i = 0; i < Server.MaxClients; i++) {
            Server.clients[i].Udp.SendData(_packet);
            if (Server.clients[i].IsConnected) { NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length()); }
        }
    }

    private static void SendUDPDataToAll(int _excpetClient, Packet _packet) {
        _packet.WriteLength();
        for (int i = 0; i < Server.MaxClients; i++) {
            if (i != _excpetClient) {
                Server.clients[i].Udp.SendData(_packet);
                if (Server.clients[i].IsConnected) { NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length()); }
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

    public static void Ping(int _toClient, bool _sendPingBack) {
        using (Packet _packet = new Packet((int)ServerPackets.Ping)) {
            _packet.Write(_sendPingBack);

            SendTCPData(_toClient, _packet);
        }
    }

    public static void VariablesTest(int _toClient, byte _varByte, short _varShort, int _varInt, long _varLong, float _varFloat, bool _varBool, string _varString, Vector2 _varVec2, Vector3 _varVec3, Quaternion _varQuat, byte[] _varArrayByte, short[] _varArrayShort, int[] _varArrayInt, long[] _varArrayLong, float[] _varArrayFloat, bool[] _varArrayBool, string[] _varArrayString, Vector2[] _varArrayVec2, Vector3[] _varArrayVec3, Quaternion[] _varArrayQuat) {
        using (Packet _packet = new Packet((int)ServerPackets.VariablesTest)) {
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

            SendTCPData(_toClient, _packet);
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
        CallOnPingPacketCallbacks,
        CallOnVariablesTestPacketCallbacks,
    };

    public static event USNLCallbackEvent OnServerStarted;
    public static event USNLCallbackEvent OnServerStopped;
    public static event USNLCallbackEvent OnClientConnected;
    public static event USNLCallbackEvent OnClientDisconnected;

    public static event USNLCallbackEvent OnWelcomeReceivedPacket;
    public static event USNLCallbackEvent OnClientInputPacket;
    public static event USNLCallbackEvent OnPingPacket;
    public static event USNLCallbackEvent OnVariablesTestPacket;

    public static void CallOnServerStartedCallbacks(object _param) { if (OnServerStarted != null) { OnServerStarted(_param); } }
    public static void CallOnServerStoppedCallbacks(object _param) { if (OnServerStopped != null) { OnServerStopped(_param); } }
    public static void CallOnClientConnectedCallbacks(object _param) { if (OnClientConnected != null) { OnClientConnected(_param); } }
    public static void CallOnClientDisconnectedCallbacks(object _param) { if (OnClientDisconnected != null) { OnClientDisconnected(_param); } }

    public static void CallOnWelcomeReceivedPacketCallbacks(object _param) { if (OnWelcomeReceivedPacket != null) { OnWelcomeReceivedPacket(_param); } }
    public static void CallOnClientInputPacketCallbacks(object _param) { if (OnClientInputPacket != null) { OnClientInputPacket(_param); } }
    public static void CallOnPingPacketCallbacks(object _param) { if (OnPingPacket != null) { OnPingPacket(_param); } }
    public static void CallOnVariablesTestPacketCallbacks(object _param) { if (OnVariablesTestPacket != null) { OnVariablesTestPacket(_param); } }
}

#endregion
