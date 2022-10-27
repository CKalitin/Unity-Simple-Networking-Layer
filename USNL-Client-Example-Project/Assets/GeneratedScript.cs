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


public static class PacketHandlers {
    public delegate void PacketHandler(Packet _packet);
    public static List<PacketHandler> packetHandlers = new List<PacketHandler>() {
        { Welcome },
        { SyncedObjectInstantiate },
        { SyncedObjectDestroy },
        { SyncedObjectUpdate },
    };

    public static void Welcome(Packet _packet) {
        WelcomePacket welcomePacket = new WelcomePacket(_packet.ReadString(), _packet.ReadInt());
        PacketManager.instance.PacketReceived(_packet, welcomePacket);
    }

    public static void SyncedObjectInstantiate(Packet _packet) {
        SyncedObjectInstantiatePacket syncedObjectInstantiatePacket = new SyncedObjectInstantiatePacket(_packet.ReadInt(), _packet.ReadInt(), _packet.ReadVector3(), _packet.ReadQuaternion(), _packet.ReadVector3());
        PacketManager.instance.PacketReceived(_packet, syncedObjectInstantiatePacket);
    }

    public static void SyncedObjectDestroy(Packet _packet) {
        SyncedObjectDestroyPacket syncedObjectDestroyPacket = new SyncedObjectDestroyPacket(_packet.ReadInt());
        PacketManager.instance.PacketReceived(_packet, syncedObjectDestroyPacket);
    }

    public static void SyncedObjectUpdate(Packet _packet) {
        SyncedObjectUpdatePacket syncedObjectUpdatePacket = new SyncedObjectUpdatePacket(_packet.ReadInt(), _packet.ReadVector3(), _packet.ReadQuaternion(), _packet.ReadVector3());
        PacketManager.instance.PacketReceived(_packet, syncedObjectUpdatePacket);
    }
}

public static class PacketSend {
    private static void SendTCPData(Packet _packet) {
        _packet.WriteLength();
        if (Client.instance.IsConnected) {
            Client.instance.Tcp.SendData(_packet);
        }
    }

    private static void SendUDPData(Packet _packet) {
        _packet.WriteLength();
        if (Client.instance.IsConnected) {
            Client.instance.Udp.SendData(_packet);
        }
    }

    public static void WelcomeReceived(int _clientIdCheck) {
        using (Packet _packet = new Packet((int)ClientPackets.WelcomeReceived)) {
            _packet.Write(_clientIdCheck);

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
    };

    public static event USNLCallbackEvent OnConnected;
    public static event USNLCallbackEvent OnDisconnected;

    public static event USNLCallbackEvent OnWelcomePacket;
    public static event USNLCallbackEvent OnSyncedObjectInstantiatePacket;
    public static event USNLCallbackEvent OnSyncedObjectDestroyPacket;
    public static event USNLCallbackEvent OnSyncedObjectUpdatePacket;

    public static void CallOnConnectedCallbacks(object _param) { OnConnected(_param); }
    public static void CallOnDisconnectedCallbacks(object _param) { OnDisconnected(_param); }

    public static void CallOnWelcomePacketCallbacks(object _param) { OnWelcomePacket(_param); }
    public static void CallOnSyncedObjectInstantiatePacketCallbacks(object _param) { OnSyncedObjectInstantiatePacket(_param); }
    public static void CallOnSyncedObjectDestroyPacketCallbacks(object _param) { OnSyncedObjectDestroyPacket(_param); }
    public static void CallOnSyncedObjectUpdatePacketCallbacks(object _param) { OnSyncedObjectUpdatePacket(_param); }
}

#endregion
