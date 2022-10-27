using System.Collections.Generic;using UnityEngine;

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
        if (!ClientManager.instance.PacketHandlers) { return; }
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
