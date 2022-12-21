using System.Collections.Generic;
using UnityEngine;

namespace USNL {
    #region Packet Enums

    public enum ClientPackets {
        WelcomeReceived,
        Ping,
        ClientInput,
    }

    public enum ServerPackets {
        Welcome,
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
            { Package.PacketHandlers.Welcome },
            { Package.PacketHandlers.Ping },
            { Package.PacketHandlers.DisconnectClient },
            { Package.PacketHandlers.SyncedObjectInstantiate },
            { Package.PacketHandlers.SyncedObjectDestroy },
            { Package.PacketHandlers.SyncedObjectInterpolationMode },
            { Package.PacketHandlers.SyncedObjectVec2PosUpdate },
            { Package.PacketHandlers.SyncedObjectVec3PosUpdate },
            { Package.PacketHandlers.SyncedObjectRotZUpdate },
            { Package.PacketHandlers.SyncedObjectRotUpdate },
            { Package.PacketHandlers.SyncedObjectVec2ScaleUpdate },
            { Package.PacketHandlers.SyncedObjectVec3ScaleUpdate },
            { Package.PacketHandlers.SyncedObjectVec2PosInterpolation },
            { Package.PacketHandlers.SyncedObjectVec3PosInterpolation },
            { Package.PacketHandlers.SyncedObjectRotZInterpolation },
            { Package.PacketHandlers.SyncedObjectRotInterpolation },
            { Package.PacketHandlers.SyncedObjectVec2ScaleInterpolation },
            { Package.PacketHandlers.SyncedObjectVec3ScaleInterpolation },
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
        Ping,
        ClientInput,
    }

    public enum ServerPackets {
        Welcome,
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

        public static void Welcome(Package.Packet _packet) {
            string welcomeMessage = _packet.ReadString();
            string serverName = _packet.ReadString();
            int clientId = _packet.ReadInt();

            Package.WelcomePacket welcomePacket = new Package.WelcomePacket(welcomeMessage, serverName, clientId);
            Package.PacketManager.instance.PacketReceived(_packet, welcomePacket);
        }

        public static void Ping(Package.Packet _packet) {
            bool sendPingBack = _packet.ReadBool();

            Package.PingPacket pingPacket = new Package.PingPacket(sendPingBack);
            Package.PacketManager.instance.PacketReceived(_packet, pingPacket);
        }

        public static void DisconnectClient(Package.Packet _packet) {
            string disconnectMessage = _packet.ReadString();

            Package.DisconnectClientPacket disconnectClientPacket = new Package.DisconnectClientPacket(disconnectMessage);
            Package.PacketManager.instance.PacketReceived(_packet, disconnectClientPacket);
        }

        public static void SyncedObjectInstantiate(Package.Packet _packet) {
            string syncedObjectTag = _packet.ReadString();
            int syncedObjectUUID = _packet.ReadInt();
            Vector3 position = _packet.ReadVector3();
            Quaternion rotation = _packet.ReadQuaternion();
            Vector3 scale = _packet.ReadVector3();

            Package.SyncedObjectInstantiatePacket syncedObjectInstantiatePacket = new Package.SyncedObjectInstantiatePacket(syncedObjectTag, syncedObjectUUID, position, rotation, scale);
            Package.PacketManager.instance.PacketReceived(_packet, syncedObjectInstantiatePacket);
        }

        public static void SyncedObjectDestroy(Package.Packet _packet) {
            int syncedObjectUUID = _packet.ReadInt();

            Package.SyncedObjectDestroyPacket syncedObjectDestroyPacket = new Package.SyncedObjectDestroyPacket(syncedObjectUUID);
            Package.PacketManager.instance.PacketReceived(_packet, syncedObjectDestroyPacket);
        }

        public static void SyncedObjectInterpolationMode(Package.Packet _packet) {
            bool serverInterpolation = _packet.ReadBool();

            Package.SyncedObjectInterpolationModePacket syncedObjectInterpolationModePacket = new Package.SyncedObjectInterpolationModePacket(serverInterpolation);
            Package.PacketManager.instance.PacketReceived(_packet, syncedObjectInterpolationModePacket);
        }

        public static void SyncedObjectVec2PosUpdate(Package.Packet _packet) {
            int[] syncedObjectUUIDs = _packet.ReadInts();
            Vector2[] positions = _packet.ReadVector2s();

            Package.SyncedObjectVec2PosUpdatePacket syncedObjectVec2PosUpdatePacket = new Package.SyncedObjectVec2PosUpdatePacket(syncedObjectUUIDs, positions);
            Package.PacketManager.instance.PacketReceived(_packet, syncedObjectVec2PosUpdatePacket);
        }

        public static void SyncedObjectVec3PosUpdate(Package.Packet _packet) {
            int[] syncedObjectUUIDs = _packet.ReadInts();
            Vector3[] positions = _packet.ReadVector3s();

            Package.SyncedObjectVec3PosUpdatePacket syncedObjectVec3PosUpdatePacket = new Package.SyncedObjectVec3PosUpdatePacket(syncedObjectUUIDs, positions);
            Package.PacketManager.instance.PacketReceived(_packet, syncedObjectVec3PosUpdatePacket);
        }

        public static void SyncedObjectRotZUpdate(Package.Packet _packet) {
            int[] syncedObjectUUIDs = _packet.ReadInts();
            float[] rotations = _packet.ReadFloats();

            Package.SyncedObjectRotZUpdatePacket syncedObjectRotZUpdatePacket = new Package.SyncedObjectRotZUpdatePacket(syncedObjectUUIDs, rotations);
            Package.PacketManager.instance.PacketReceived(_packet, syncedObjectRotZUpdatePacket);
        }

        public static void SyncedObjectRotUpdate(Package.Packet _packet) {
            int[] syncedObjectUUIDs = _packet.ReadInts();
            Vector3[] rotations = _packet.ReadVector3s();

            Package.SyncedObjectRotUpdatePacket syncedObjectRotUpdatePacket = new Package.SyncedObjectRotUpdatePacket(syncedObjectUUIDs, rotations);
            Package.PacketManager.instance.PacketReceived(_packet, syncedObjectRotUpdatePacket);
        }

        public static void SyncedObjectVec2ScaleUpdate(Package.Packet _packet) {
            int[] syncedObjectUUIDs = _packet.ReadInts();
            Vector2[] scales = _packet.ReadVector2s();

            Package.SyncedObjectVec2ScaleUpdatePacket syncedObjectVec2ScaleUpdatePacket = new Package.SyncedObjectVec2ScaleUpdatePacket(syncedObjectUUIDs, scales);
            Package.PacketManager.instance.PacketReceived(_packet, syncedObjectVec2ScaleUpdatePacket);
        }

        public static void SyncedObjectVec3ScaleUpdate(Package.Packet _packet) {
            int[] syncedObjectUUIDs = _packet.ReadInts();
            Vector3[] scales = _packet.ReadVector3s();

            Package.SyncedObjectVec3ScaleUpdatePacket syncedObjectVec3ScaleUpdatePacket = new Package.SyncedObjectVec3ScaleUpdatePacket(syncedObjectUUIDs, scales);
            Package.PacketManager.instance.PacketReceived(_packet, syncedObjectVec3ScaleUpdatePacket);
        }

        public static void SyncedObjectVec2PosInterpolation(Package.Packet _packet) {
            int[] syncedObjectUUIDs = _packet.ReadInts();
            Vector2[] interpolatePositions = _packet.ReadVector2s();

            Package.SyncedObjectVec2PosInterpolationPacket syncedObjectVec2PosInterpolationPacket = new Package.SyncedObjectVec2PosInterpolationPacket(syncedObjectUUIDs, interpolatePositions);
            Package.PacketManager.instance.PacketReceived(_packet, syncedObjectVec2PosInterpolationPacket);
        }

        public static void SyncedObjectVec3PosInterpolation(Package.Packet _packet) {
            int[] syncedObjectUUIDs = _packet.ReadInts();
            Vector3[] interpolatePositions = _packet.ReadVector3s();

            Package.SyncedObjectVec3PosInterpolationPacket syncedObjectVec3PosInterpolationPacket = new Package.SyncedObjectVec3PosInterpolationPacket(syncedObjectUUIDs, interpolatePositions);
            Package.PacketManager.instance.PacketReceived(_packet, syncedObjectVec3PosInterpolationPacket);
        }

        public static void SyncedObjectRotZInterpolation(Package.Packet _packet) {
            int[] syncedObjectUUIDs = _packet.ReadInts();
            float[] interpolateRotations = _packet.ReadFloats();

            Package.SyncedObjectRotZInterpolationPacket syncedObjectRotZInterpolationPacket = new Package.SyncedObjectRotZInterpolationPacket(syncedObjectUUIDs, interpolateRotations);
            Package.PacketManager.instance.PacketReceived(_packet, syncedObjectRotZInterpolationPacket);
        }

        public static void SyncedObjectRotInterpolation(Package.Packet _packet) {
            int[] syncedObjectUUIDs = _packet.ReadInts();
            Vector3[] interpolateRotations = _packet.ReadVector3s();

            Package.SyncedObjectRotInterpolationPacket syncedObjectRotInterpolationPacket = new Package.SyncedObjectRotInterpolationPacket(syncedObjectUUIDs, interpolateRotations);
            Package.PacketManager.instance.PacketReceived(_packet, syncedObjectRotInterpolationPacket);
        }

        public static void SyncedObjectVec2ScaleInterpolation(Package.Packet _packet) {
            int[] syncedObjectUUIDs = _packet.ReadInts();
            Vector2[] interpolateScales = _packet.ReadVector2s();

            Package.SyncedObjectVec2ScaleInterpolationPacket syncedObjectVec2ScaleInterpolationPacket = new Package.SyncedObjectVec2ScaleInterpolationPacket(syncedObjectUUIDs, interpolateScales);
            Package.PacketManager.instance.PacketReceived(_packet, syncedObjectVec2ScaleInterpolationPacket);
        }

        public static void SyncedObjectVec3ScaleInterpolation(Package.Packet _packet) {
            int[] syncedObjectUUIDs = _packet.ReadInts();
            Vector3[] interpolateScales = _packet.ReadVector3s();

            Package.SyncedObjectVec3ScaleInterpolationPacket syncedObjectVec3ScaleInterpolationPacket = new Package.SyncedObjectVec3ScaleInterpolationPacket(syncedObjectUUIDs, interpolateScales);
            Package.PacketManager.instance.PacketReceived(_packet, syncedObjectVec3ScaleInterpolationPacket);
        }
    }

    #endregion

    #region Packet Send

    public static class PacketSend {
        private static void SendTCPData(USNL.Package.Packet _packet) {
            _packet.WriteLength();
            if (USNL.Package.Client.instance.IsConnected) {
                USNL.Package.Client.instance.Tcp.SendData(_packet);
                NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length());
            }
        }
    
        private static void SendUDPData(USNL.Package.Packet _packet) {
            _packet.WriteLength();
            if (USNL.Package.Client.instance.IsConnected) {
                USNL.Package.Client.instance.Udp.SendData(_packet);
                NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length());
            }
        }
    
        public static void WelcomeReceived(int _clientIdCheck) {
            using (Package.Packet _packet = new Package.Packet((int)USNL.Package.ClientPackets.WelcomeReceived)) {
                _packet.Write(_clientIdCheck);

                SendTCPData(_packet);
            }
        }

        public static void Ping(bool _sendPingBack) {
            using (Package.Packet _packet = new Package.Packet((int)USNL.Package.ClientPackets.Ping)) {
                _packet.Write(_sendPingBack);

                SendTCPData(_packet);
            }
        }

        public static void ClientInput(int[] _keycodesDown, int[] _keycodesUp) {
            using (Package.Packet _packet = new Package.Packet((int)USNL.Package.ClientPackets.ClientInput)) {
                _packet.Write(_keycodesDown);
                _packet.Write(_keycodesUp);

                SendTCPData(_packet);
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
