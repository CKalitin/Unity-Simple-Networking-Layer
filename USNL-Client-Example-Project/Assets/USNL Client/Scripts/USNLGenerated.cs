using System.Collections.Generic;
using UnityEngine;

namespace USNL {
    #region Packet Enums

    public enum ClientPackets {
        WelcomeReceived,
        Connect,
        ConnectionConfirmed,
        Ping,
        ClientInput,
    }

    public enum ServerPackets {
        Welcome,
        ConnectReceived,
        ServerInfo,
        Ping,
        DisconnectClient,
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

    #endregion

    #region Packet Structs



    #endregion

    #region Packet Handlers

    public static class PacketHandlers {
       public delegate void PacketHandler(USNL.Package.Packet _packet);
        public static List<PacketHandler> packetHandlers = new List<PacketHandler>() {
            { USNL.Package.PacketHandlers.Welcome },
            { USNL.Package.PacketHandlers.ConnectReceived },
            { USNL.Package.PacketHandlers.ServerInfo },
            { USNL.Package.PacketHandlers.Ping },
            { USNL.Package.PacketHandlers.DisconnectClient },
            { USNL.Package.PacketHandlers.SyncedObjectInstantiate },
            { USNL.Package.PacketHandlers.SyncedObjectDestroy },
            { USNL.Package.PacketHandlers.SyncedObjectInterpolationMode },
            { USNL.Package.PacketHandlers.SyncedObjectVec2PosUpdate },
            { USNL.Package.PacketHandlers.SyncedObjectVec3PosUpdate },
            { USNL.Package.PacketHandlers.SyncedObjectRotZUpdate },
            { USNL.Package.PacketHandlers.SyncedObjectRotUpdate },
            { USNL.Package.PacketHandlers.SyncedObjectVec2ScaleUpdate },
            { USNL.Package.PacketHandlers.SyncedObjectVec3ScaleUpdate },
            { USNL.Package.PacketHandlers.SyncedObjectVec2PosInterpolation },
            { USNL.Package.PacketHandlers.SyncedObjectVec3PosInterpolation },
            { USNL.Package.PacketHandlers.SyncedObjectRotZInterpolation },
            { USNL.Package.PacketHandlers.SyncedObjectRotInterpolation },
            { USNL.Package.PacketHandlers.SyncedObjectVec2ScaleInterpolation },
            { USNL.Package.PacketHandlers.SyncedObjectVec3ScaleInterpolation },
        };

    }

    #endregion

    #region Packet Send

    public static class PacketSend {

    }

#endregion
}

namespace USNL.Package {
    #region Packet Enums
    public enum ClientPackets {
        WelcomeReceived,
        Connect,
        ConnectionConfirmed,
        Ping,
        ClientInput,
    }

    public enum ServerPackets {
        Welcome,
        ConnectReceived,
        ServerInfo,
        Ping,
        DisconnectClient,
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
    #endregion

    #region Packet Structs

    public struct WelcomePacket {
        private int lobbyClientId;
        private string welcomeMessage;

        public WelcomePacket(int _lobbyClientId, string _welcomeMessage) {
            lobbyClientId = _lobbyClientId;
            welcomeMessage = _welcomeMessage;
        }

        public int LobbyClientId { get => lobbyClientId; set => lobbyClientId = value; }
        public string WelcomeMessage { get => welcomeMessage; set => welcomeMessage = value; }
    }

    public struct ConnectReceivedPacket {
        private int clientId;
        private string connectMessage;

        public ConnectReceivedPacket(int _clientId, string _connectMessage) {
            clientId = _clientId;
            connectMessage = _connectMessage;
        }

        public int ClientId { get => clientId; set => clientId = value; }
        public string ConnectMessage { get => connectMessage; set => connectMessage = value; }
    }

    public struct ServerInfoPacket {
        private string serverName;
        private int[] connectedClientsIds;
        private int maxClients;
        private bool serverFull;

        public ServerInfoPacket(string _serverName, int[] _connectedClientsIds, int _maxClients, bool _serverFull) {
            serverName = _serverName;
            connectedClientsIds = _connectedClientsIds;
            maxClients = _maxClients;
            serverFull = _serverFull;
        }

        public string ServerName { get => serverName; set => serverName = value; }
        public int[] ConnectedClientsIds { get => connectedClientsIds; set => connectedClientsIds = value; }
        public int MaxClients { get => maxClients; set => maxClients = value; }
        public bool ServerFull { get => serverFull; set => serverFull = value; }
    }

    public struct PingPacket {
        private bool sendPingBack;

        public PingPacket(bool _sendPingBack) {
            sendPingBack = _sendPingBack;
        }

        public bool SendPingBack { get => sendPingBack; set => sendPingBack = value; }
    }

    public struct DisconnectClientPacket {
        private string disconnectMessage;

        public DisconnectClientPacket(string _disconnectMessage) {
            disconnectMessage = _disconnectMessage;
        }

        public string DisconnectMessage { get => disconnectMessage; set => disconnectMessage = value; }
    }

    public struct SyncedObjectInstantiatePacket {
        private string syncedObjectTag;
        private int syncedObjectUUID;
        private Vector3 position;
        private Quaternion rotation;
        private Vector3 scale;

        public SyncedObjectInstantiatePacket(string _syncedObjectTag, int _syncedObjectUUID, Vector3 _position, Quaternion _rotation, Vector3 _scale) {
            syncedObjectTag = _syncedObjectTag;
            syncedObjectUUID = _syncedObjectUUID;
            position = _position;
            rotation = _rotation;
            scale = _scale;
        }

        public string SyncedObjectTag { get => syncedObjectTag; set => syncedObjectTag = value; }
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


    #endregion

    #region Packet Handlers

    public static class PacketHandlers {
       public delegate void PacketHandler(USNL.Package.Packet _packet);
        public static List<PacketHandler> packetHandlers = new List<PacketHandler>() {
            { Welcome },
            { ConnectReceived },
            { ServerInfo },
            { Ping },
            { DisconnectClient },
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
        };

        public static void Welcome(USNL.Package.Packet _packet) {
            int lobbyClientId = _packet.ReadInt();
            string welcomeMessage = _packet.ReadString();

            USNL.Package.WelcomePacket welcomePacket = new USNL.Package.WelcomePacket(lobbyClientId, welcomeMessage);
            USNL.Package.PacketManager.instance.PacketReceived(_packet, welcomePacket);
        }

        public static void ConnectReceived(USNL.Package.Packet _packet) {
            int clientId = _packet.ReadInt();
            string connectMessage = _packet.ReadString();

            USNL.Package.ConnectReceivedPacket connectReceivedPacket = new USNL.Package.ConnectReceivedPacket(clientId, connectMessage);
            USNL.Package.PacketManager.instance.PacketReceived(_packet, connectReceivedPacket);
        }

        public static void ServerInfo(USNL.Package.Packet _packet) {
            string serverName = _packet.ReadString();
            int[] connectedClientsIds = _packet.ReadInts();
            int maxClients = _packet.ReadInt();
            bool serverFull = _packet.ReadBool();

            USNL.Package.ServerInfoPacket serverInfoPacket = new USNL.Package.ServerInfoPacket(serverName, connectedClientsIds, maxClients, serverFull);
            USNL.Package.PacketManager.instance.PacketReceived(_packet, serverInfoPacket);
        }

        public static void Ping(USNL.Package.Packet _packet) {
            bool sendPingBack = _packet.ReadBool();

            USNL.Package.PingPacket pingPacket = new USNL.Package.PingPacket(sendPingBack);
            USNL.Package.PacketManager.instance.PacketReceived(_packet, pingPacket);
        }

        public static void DisconnectClient(USNL.Package.Packet _packet) {
            string disconnectMessage = _packet.ReadString();

            USNL.Package.DisconnectClientPacket disconnectClientPacket = new USNL.Package.DisconnectClientPacket(disconnectMessage);
            USNL.Package.PacketManager.instance.PacketReceived(_packet, disconnectClientPacket);
        }

        public static void SyncedObjectInstantiate(USNL.Package.Packet _packet) {
            string syncedObjectTag = _packet.ReadString();
            int syncedObjectUUID = _packet.ReadInt();
            Vector3 position = _packet.ReadVector3();
            Quaternion rotation = _packet.ReadQuaternion();
            Vector3 scale = _packet.ReadVector3();

            USNL.Package.SyncedObjectInstantiatePacket syncedObjectInstantiatePacket = new USNL.Package.SyncedObjectInstantiatePacket(syncedObjectTag, syncedObjectUUID, position, rotation, scale);
            USNL.Package.PacketManager.instance.PacketReceived(_packet, syncedObjectInstantiatePacket);
        }

        public static void SyncedObjectDestroy(USNL.Package.Packet _packet) {
            int syncedObjectUUID = _packet.ReadInt();

            USNL.Package.SyncedObjectDestroyPacket syncedObjectDestroyPacket = new USNL.Package.SyncedObjectDestroyPacket(syncedObjectUUID);
            USNL.Package.PacketManager.instance.PacketReceived(_packet, syncedObjectDestroyPacket);
        }

        public static void SyncedObjectInterpolationMode(USNL.Package.Packet _packet) {
            bool serverInterpolation = _packet.ReadBool();

            USNL.Package.SyncedObjectInterpolationModePacket syncedObjectInterpolationModePacket = new USNL.Package.SyncedObjectInterpolationModePacket(serverInterpolation);
            USNL.Package.PacketManager.instance.PacketReceived(_packet, syncedObjectInterpolationModePacket);
        }

        public static void SyncedObjectVec2PosUpdate(USNL.Package.Packet _packet) {
            int[] syncedObjectUUIDs = _packet.ReadInts();
            Vector2[] positions = _packet.ReadVector2s();

            USNL.Package.SyncedObjectVec2PosUpdatePacket syncedObjectVec2PosUpdatePacket = new USNL.Package.SyncedObjectVec2PosUpdatePacket(syncedObjectUUIDs, positions);
            USNL.Package.PacketManager.instance.PacketReceived(_packet, syncedObjectVec2PosUpdatePacket);
        }

        public static void SyncedObjectVec3PosUpdate(USNL.Package.Packet _packet) {
            int[] syncedObjectUUIDs = _packet.ReadInts();
            Vector3[] positions = _packet.ReadVector3s();

            USNL.Package.SyncedObjectVec3PosUpdatePacket syncedObjectVec3PosUpdatePacket = new USNL.Package.SyncedObjectVec3PosUpdatePacket(syncedObjectUUIDs, positions);
            USNL.Package.PacketManager.instance.PacketReceived(_packet, syncedObjectVec3PosUpdatePacket);
        }

        public static void SyncedObjectRotZUpdate(USNL.Package.Packet _packet) {
            int[] syncedObjectUUIDs = _packet.ReadInts();
            float[] rotations = _packet.ReadFloats();

            USNL.Package.SyncedObjectRotZUpdatePacket syncedObjectRotZUpdatePacket = new USNL.Package.SyncedObjectRotZUpdatePacket(syncedObjectUUIDs, rotations);
            USNL.Package.PacketManager.instance.PacketReceived(_packet, syncedObjectRotZUpdatePacket);
        }

        public static void SyncedObjectRotUpdate(USNL.Package.Packet _packet) {
            int[] syncedObjectUUIDs = _packet.ReadInts();
            Vector3[] rotations = _packet.ReadVector3s();

            USNL.Package.SyncedObjectRotUpdatePacket syncedObjectRotUpdatePacket = new USNL.Package.SyncedObjectRotUpdatePacket(syncedObjectUUIDs, rotations);
            USNL.Package.PacketManager.instance.PacketReceived(_packet, syncedObjectRotUpdatePacket);
        }

        public static void SyncedObjectVec2ScaleUpdate(USNL.Package.Packet _packet) {
            int[] syncedObjectUUIDs = _packet.ReadInts();
            Vector2[] scales = _packet.ReadVector2s();

            USNL.Package.SyncedObjectVec2ScaleUpdatePacket syncedObjectVec2ScaleUpdatePacket = new USNL.Package.SyncedObjectVec2ScaleUpdatePacket(syncedObjectUUIDs, scales);
            USNL.Package.PacketManager.instance.PacketReceived(_packet, syncedObjectVec2ScaleUpdatePacket);
        }

        public static void SyncedObjectVec3ScaleUpdate(USNL.Package.Packet _packet) {
            int[] syncedObjectUUIDs = _packet.ReadInts();
            Vector3[] scales = _packet.ReadVector3s();

            USNL.Package.SyncedObjectVec3ScaleUpdatePacket syncedObjectVec3ScaleUpdatePacket = new USNL.Package.SyncedObjectVec3ScaleUpdatePacket(syncedObjectUUIDs, scales);
            USNL.Package.PacketManager.instance.PacketReceived(_packet, syncedObjectVec3ScaleUpdatePacket);
        }

        public static void SyncedObjectVec2PosInterpolation(USNL.Package.Packet _packet) {
            int[] syncedObjectUUIDs = _packet.ReadInts();
            Vector2[] interpolatePositions = _packet.ReadVector2s();

            USNL.Package.SyncedObjectVec2PosInterpolationPacket syncedObjectVec2PosInterpolationPacket = new USNL.Package.SyncedObjectVec2PosInterpolationPacket(syncedObjectUUIDs, interpolatePositions);
            USNL.Package.PacketManager.instance.PacketReceived(_packet, syncedObjectVec2PosInterpolationPacket);
        }

        public static void SyncedObjectVec3PosInterpolation(USNL.Package.Packet _packet) {
            int[] syncedObjectUUIDs = _packet.ReadInts();
            Vector3[] interpolatePositions = _packet.ReadVector3s();

            USNL.Package.SyncedObjectVec3PosInterpolationPacket syncedObjectVec3PosInterpolationPacket = new USNL.Package.SyncedObjectVec3PosInterpolationPacket(syncedObjectUUIDs, interpolatePositions);
            USNL.Package.PacketManager.instance.PacketReceived(_packet, syncedObjectVec3PosInterpolationPacket);
        }

        public static void SyncedObjectRotZInterpolation(USNL.Package.Packet _packet) {
            int[] syncedObjectUUIDs = _packet.ReadInts();
            float[] interpolateRotations = _packet.ReadFloats();

            USNL.Package.SyncedObjectRotZInterpolationPacket syncedObjectRotZInterpolationPacket = new USNL.Package.SyncedObjectRotZInterpolationPacket(syncedObjectUUIDs, interpolateRotations);
            USNL.Package.PacketManager.instance.PacketReceived(_packet, syncedObjectRotZInterpolationPacket);
        }

        public static void SyncedObjectRotInterpolation(USNL.Package.Packet _packet) {
            int[] syncedObjectUUIDs = _packet.ReadInts();
            Vector3[] interpolateRotations = _packet.ReadVector3s();

            USNL.Package.SyncedObjectRotInterpolationPacket syncedObjectRotInterpolationPacket = new USNL.Package.SyncedObjectRotInterpolationPacket(syncedObjectUUIDs, interpolateRotations);
            USNL.Package.PacketManager.instance.PacketReceived(_packet, syncedObjectRotInterpolationPacket);
        }

        public static void SyncedObjectVec2ScaleInterpolation(USNL.Package.Packet _packet) {
            int[] syncedObjectUUIDs = _packet.ReadInts();
            Vector2[] interpolateScales = _packet.ReadVector2s();

            USNL.Package.SyncedObjectVec2ScaleInterpolationPacket syncedObjectVec2ScaleInterpolationPacket = new USNL.Package.SyncedObjectVec2ScaleInterpolationPacket(syncedObjectUUIDs, interpolateScales);
            USNL.Package.PacketManager.instance.PacketReceived(_packet, syncedObjectVec2ScaleInterpolationPacket);
        }

        public static void SyncedObjectVec3ScaleInterpolation(USNL.Package.Packet _packet) {
            int[] syncedObjectUUIDs = _packet.ReadInts();
            Vector3[] interpolateScales = _packet.ReadVector3s();

            USNL.Package.SyncedObjectVec3ScaleInterpolationPacket syncedObjectVec3ScaleInterpolationPacket = new USNL.Package.SyncedObjectVec3ScaleInterpolationPacket(syncedObjectUUIDs, interpolateScales);
            USNL.Package.PacketManager.instance.PacketReceived(_packet, syncedObjectVec3ScaleInterpolationPacket);
        }
    }

    #endregion

    #region Packet Send

    public static class PacketSend {
        private static void SendTCPData(USNL.Package.Packet _packet) {
            _packet.WriteLength();
            if (USNL.Package.Client.instance.IsConnected) {
                USNL.Package.Client.instance.Tcp.SendData(_packet);
                USNL.NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length());
            }
        }
    
        private static void SendUDPData(USNL.Package.Packet _packet) {
            _packet.WriteLength();
            if (USNL.Package.Client.instance.IsConnected) {
                USNL.Package.Client.instance.Udp.SendData(_packet);
                USNL.NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length());
            }
        }
    
        public static void WelcomeReceived(int _lobbyClientIdCheck) {
            using (USNL.Package.Packet _packet = new USNL.Package.Packet((int)USNL.Package.ClientPackets.WelcomeReceived)) {
                _packet.Write(_lobbyClientIdCheck);

                SendTCPData(_packet);
            }
        }

        public static void Connect(int _variablesListCantBeNull) {
            using (USNL.Package.Packet _packet = new USNL.Package.Packet((int)USNL.Package.ClientPackets.Connect)) {
                _packet.Write(_variablesListCantBeNull);

                SendTCPData(_packet);
            }
        }

        public static void ConnectionConfirmed(int _clientIdCheck) {
            using (USNL.Package.Packet _packet = new USNL.Package.Packet((int)USNL.Package.ClientPackets.ConnectionConfirmed)) {
                _packet.Write(_clientIdCheck);

                SendTCPData(_packet);
            }
        }

        public static void Ping(bool _sendPingBack) {
            using (USNL.Package.Packet _packet = new USNL.Package.Packet((int)USNL.Package.ClientPackets.Ping)) {
                _packet.Write(_sendPingBack);

                SendTCPData(_packet);
            }
        }

        public static void ClientInput(int[] _keycodesDown, int[] _keycodesUp) {
            using (USNL.Package.Packet _packet = new USNL.Package.Packet((int)USNL.Package.ClientPackets.ClientInput)) {
                _packet.Write(_keycodesDown);
                _packet.Write(_keycodesUp);

                if (!USNL.ClientManager.instance.InLobby) {
                    Debug.Log("Here");
                    SendTCPData(_packet);
                }
            }
        }
    }

    #endregion
}

#region Callbacks

namespace USNL {
    public static class CallbackEvents {
        public delegate void CallbackEvent(object _param);

        public static CallbackEvent[] PacketCallbackEvents = {
            CallOnWelcomePacketCallbacks,
            CallOnConnectReceivedPacketCallbacks,
            CallOnServerInfoPacketCallbacks,
            CallOnPingPacketCallbacks,
            CallOnDisconnectClientPacketCallbacks,
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
        };

        public static event CallbackEvent OnConnected;
        public static event CallbackEvent OnDisconnected;

        public static event CallbackEvent OnWelcomePacket;
        public static event CallbackEvent OnConnectReceivedPacket;
        public static event CallbackEvent OnServerInfoPacket;
        public static event CallbackEvent OnPingPacket;
        public static event CallbackEvent OnDisconnectClientPacket;
        public static event CallbackEvent OnSyncedObjectInstantiatePacket;
        public static event CallbackEvent OnSyncedObjectDestroyPacket;
        public static event CallbackEvent OnSyncedObjectInterpolationModePacket;
        public static event CallbackEvent OnSyncedObjectVec2PosUpdatePacket;
        public static event CallbackEvent OnSyncedObjectVec3PosUpdatePacket;
        public static event CallbackEvent OnSyncedObjectRotZUpdatePacket;
        public static event CallbackEvent OnSyncedObjectRotUpdatePacket;
        public static event CallbackEvent OnSyncedObjectVec2ScaleUpdatePacket;
        public static event CallbackEvent OnSyncedObjectVec3ScaleUpdatePacket;
        public static event CallbackEvent OnSyncedObjectVec2PosInterpolationPacket;
        public static event CallbackEvent OnSyncedObjectVec3PosInterpolationPacket;
        public static event CallbackEvent OnSyncedObjectRotZInterpolationPacket;
        public static event CallbackEvent OnSyncedObjectRotInterpolationPacket;
        public static event CallbackEvent OnSyncedObjectVec2ScaleInterpolationPacket;
        public static event CallbackEvent OnSyncedObjectVec3ScaleInterpolationPacket;

        public static void CallOnConnectedCallbacks(object _param) { if (OnConnected != null) { OnConnected(_param); } }
        public static void CallOnDisconnectedCallbacks(object _param) { if (OnDisconnected != null) { OnDisconnected(_param); } }

        public static void CallOnWelcomePacketCallbacks(object _param) { if (OnWelcomePacket != null) { OnWelcomePacket(_param); } }
        public static void CallOnConnectReceivedPacketCallbacks(object _param) { if (OnConnectReceivedPacket != null) { OnConnectReceivedPacket(_param); } }
        public static void CallOnServerInfoPacketCallbacks(object _param) { if (OnServerInfoPacket != null) { OnServerInfoPacket(_param); } }
        public static void CallOnPingPacketCallbacks(object _param) { if (OnPingPacket != null) { OnPingPacket(_param); } }
        public static void CallOnDisconnectClientPacketCallbacks(object _param) { if (OnDisconnectClientPacket != null) { OnDisconnectClientPacket(_param); } }
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
    }
}

#endregion
