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
}

// Sent from Client to Server
public enum ClientPackets {
    WelcomeReceived,
    ClientInput,
    Ping,
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

public static class PacketHandlers {
    public delegate void PacketHandler(Packet _packet);
    public static List<PacketHandler> packetHandlers = new List<PacketHandler>() {
        { Welcome },
        { SyncedObjectInstantiate },
        { SyncedObjectDestroy },
        { SyncedObjectUpdate },
        { Ping },
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
            if (_keycodesDown.Length > 0) {
                _packet.Write(_keycodesDown.Length);
                _packet.Write(_keycodesDown);
            } else {
                _packet.Write(0);
                _packet.Write(0);
            }
            if (_keycodesUp.Length > 0) {
                _packet.Write(_keycodesUp.Length);
                _packet.Write(_keycodesUp);
            } else {
                _packet.Write(0);
                _packet.Write(0);
            }

            SendTCPData(_packet);
        }
    }

    public static void Ping(bool _sendPingBack) {
        using (Packet _packet = new Packet((int)ClientPackets.Ping)) {
            _packet.Write(_sendPingBack);

            SendTCPData(_packet);
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
    };

    public static event USNLCallbackEvent OnConnected;
    public static event USNLCallbackEvent OnDisconnected;

    public static event USNLCallbackEvent OnWelcomePacket;
    public static event USNLCallbackEvent OnSyncedObjectInstantiatePacket;
    public static event USNLCallbackEvent OnSyncedObjectDestroyPacket;
    public static event USNLCallbackEvent OnSyncedObjectUpdatePacket;
    public static event USNLCallbackEvent OnPingPacket;

    public static void CallOnConnectedCallbacks(object _param) { if (OnConnected != null) { OnConnected(_param); } }
    public static void CallOnDisconnectedCallbacks(object _param) { if (OnDisconnected != null) { OnDisconnected(_param); } }

    public static void CallOnWelcomePacketCallbacks(object _param) { if (OnWelcomePacket != null) { OnWelcomePacket(_param); } }
    public static void CallOnSyncedObjectInstantiatePacketCallbacks(object _param) { if (OnSyncedObjectInstantiatePacket != null) { OnSyncedObjectInstantiatePacket(_param); } }
    public static void CallOnSyncedObjectDestroyPacketCallbacks(object _param) { if (OnSyncedObjectDestroyPacket != null) { OnSyncedObjectDestroyPacket(_param); } }
    public static void CallOnSyncedObjectUpdatePacketCallbacks(object _param) { if (OnSyncedObjectUpdatePacket != null) { OnSyncedObjectUpdatePacket(_param); } }
    public static void CallOnPingPacketCallbacks(object _param) { if (OnPingPacket != null) { OnPingPacket(_param); } }
}

#endregion
