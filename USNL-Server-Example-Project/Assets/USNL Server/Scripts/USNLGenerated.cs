using System.Collections.Generic;
using UnityEngine;

#region Packets
namespace USNL {
    #region Packet Enums

    public enum ClientPackets {
        WelcomeReceived,
        Ping,
        ClientInput,
        RaycastFromCamera,
    }

    public enum ServerPackets {
        Welcome,
        Ping,
        ServerInfo,
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
        PlayerSpawned,
        MatchUpdate,
        Countdown,
    }

    #endregion

    #region Packet Structs

    public struct RaycastFromCameraPacket {
        private int fromClient;

        private Vector3 cameraPosition;
        private Quaternion cameraRotation;
        private Vector2 resolution;
        private float fieldOfView;
        private Vector2 mousePosition;

        public RaycastFromCameraPacket(int _fromClient, Vector3 _cameraPosition, Quaternion _cameraRotation, Vector2 _resolution, float _fieldOfView, Vector2 _mousePosition) {
            fromClient = _fromClient;
            cameraPosition = _cameraPosition;
            cameraRotation = _cameraRotation;
            resolution = _resolution;
            fieldOfView = _fieldOfView;
            mousePosition = _mousePosition;
        }

        public int FromClient { get => fromClient; set => fromClient = value; }
        public Vector3 CameraPosition { get => cameraPosition; set => cameraPosition = value; }
        public Quaternion CameraRotation { get => cameraRotation; set => cameraRotation = value; }
        public Vector2 Resolution { get => resolution; set => resolution = value; }
        public float FieldOfView { get => fieldOfView; set => fieldOfView = value; }
        public Vector2 MousePosition { get => mousePosition; set => mousePosition = value; }
    }


    #endregion

    #region Packet Send

    public static class PacketSend {
        #region TCP & UDP Send Functions
    
        private static void SendTCPData(int _toClient, USNL.Package.Packet _packet) {
            _packet.WriteLength();
            USNL.Package.Server.Clients[_toClient].Tcp.SendData(_packet);
            if (USNL.Package.Server.Clients[_toClient].IsConnected) { NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length()); }
        }
    
        private static void SendTCPDataToAll(USNL.Package.Packet _packet) {
            _packet.WriteLength();
            for (int i = 0; i < USNL.Package.Server.MaxClients; i++) {
                USNL.Package.Server.Clients[i].Tcp.SendData(_packet);
                if (USNL.Package.Server.Clients[i].IsConnected) { NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length()); }
            }
        }
    
        private static void SendTCPDataToAll(int _excpetClient, USNL.Package.Packet _packet) {
            _packet.WriteLength();
            for (int i = 0; i < USNL.Package.Server.MaxClients; i++) {
                if (i != _excpetClient) {
                    USNL.Package.Server.Clients[i].Tcp.SendData(_packet);
                    if (USNL.Package.Server.Clients[i].IsConnected) { NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length()); }
                }
            }
        }
    
        private static void SendUDPData(int _toClient, USNL.Package.Packet _packet) {
            _packet.WriteLength();
            USNL.Package.Server.Clients[_toClient].Udp.SendData(_packet);
            if (USNL.Package.Server.Clients[_toClient].IsConnected) { NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length()); }
        }
    
        private static void SendUDPDataToAll(USNL.Package.Packet _packet) {
            _packet.WriteLength();
            for (int i = 0; i < USNL.Package.Server.MaxClients; i++) {
                USNL.Package.Server.Clients[i].Udp.SendData(_packet);
                if (USNL.Package.Server.Clients[i].IsConnected) { NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length()); }
            }
        }
    
        private static void SendUDPDataToAll(int _excpetClient, USNL.Package.Packet _packet) {
            _packet.WriteLength();
            for (int i = 0; i < USNL.Package.Server.MaxClients; i++) {
                if (i != _excpetClient) {
                    USNL.Package.Server.Clients[i].Udp.SendData(_packet);
                    if (USNL.Package.Server.Clients[i].IsConnected) { NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length()); }
                }
            }
        }
    
        #endregion
    
        public static void PlayerSpawned(int _toClient, int _matchState, int _playerSyncedObjectUUID) {
            using (USNL.Package.Packet _packet = new USNL.Package.Packet((int)ServerPackets.PlayerSpawned)) {
                _packet.Write(_matchState);
                _packet.Write(_playerSyncedObjectUUID);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void MatchUpdate(int _matchState) {
            using (USNL.Package.Packet _packet = new USNL.Package.Packet((int)ServerPackets.MatchUpdate)) {
                _packet.Write(_matchState);

                SendTCPDataToAll(_packet);
            }
        }

        public static void Countdown(int[] _startTime, float _duration, string _countdownTag) {
            using (USNL.Package.Packet _packet = new USNL.Package.Packet((int)ServerPackets.Countdown)) {
                _packet.Write(_startTime);
                _packet.Write(_duration);
                _packet.Write(_countdownTag);

                SendTCPDataToAll(_packet);
            }
        }
        }

    #endregion
}

namespace USNL.Package {
    #region Packet Enums
    public enum ClientPackets {
        WelcomeReceived,
        Ping,
        ClientInput,
        RaycastFromCamera,
    }

    public enum ServerPackets {
        Welcome,
        Ping,
        ServerInfo,
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
        PlayerSpawned,
        MatchUpdate,
        Countdown,
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
        private int previousPingValue;

        public PingPacket(int _fromClient, bool _sendPingBack, int _previousPingValue) {
            fromClient = _fromClient;
            sendPingBack = _sendPingBack;
            previousPingValue = _previousPingValue;
        }

        public int FromClient { get => fromClient; set => fromClient = value; }
        public bool SendPingBack { get => sendPingBack; set => sendPingBack = value; }
        public int PreviousPingValue { get => previousPingValue; set => previousPingValue = value; }
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
       public delegate void PacketHandler(USNL.Package.Packet _packet);
        public static List<PacketHandler> packetHandlers = new List<PacketHandler>() {
            { WelcomeReceived },
            { Ping },
            { ClientInput },
            { RaycastFromCamera },
        };

        public static void WelcomeReceived(Packet _packet) {
            int clientIdCheck = _packet.ReadInt();

            WelcomeReceivedPacket welcomeReceivedPacket = new WelcomeReceivedPacket(_packet.FromClient, clientIdCheck);
            PacketManager.instance.PacketReceived(_packet, welcomeReceivedPacket);
        }

        public static void Ping(Packet _packet) {
            bool sendPingBack = _packet.ReadBool();
            int previousPingValue = _packet.ReadInt();

            PingPacket pingPacket = new PingPacket(_packet.FromClient, sendPingBack, previousPingValue);
            PacketManager.instance.PacketReceived(_packet, pingPacket);
        }

        public static void ClientInput(Packet _packet) {
            int[] keycodesDown = _packet.ReadInts();
            int[] keycodesUp = _packet.ReadInts();

            ClientInputPacket clientInputPacket = new ClientInputPacket(_packet.FromClient, keycodesDown, keycodesUp);
            PacketManager.instance.PacketReceived(_packet, clientInputPacket);
        }

        public static void RaycastFromCamera(Packet _packet) {
            Vector3 cameraPosition = _packet.ReadVector3();
            Quaternion cameraRotation = _packet.ReadQuaternion();
            Vector2 resolution = _packet.ReadVector2();
            float fieldOfView = _packet.ReadFloat();
            Vector2 mousePosition = _packet.ReadVector2();

            RaycastFromCameraPacket raycastFromCameraPacket = new RaycastFromCameraPacket(_packet.FromClient, cameraPosition, cameraRotation, resolution, fieldOfView, mousePosition);
            PacketManager.instance.PacketReceived(_packet, raycastFromCameraPacket);
        }
    }

    #endregion

    #region Packet Send

    public static class PacketSend {
        #region TCP & UDP Send Functions
    
        private static void SendTCPData(int _toClient, USNL.Package.Packet _packet) {
            _packet.WriteLength();
            USNL.Package.Server.Clients[_toClient].Tcp.SendData(_packet);
            if (USNL.Package.Server.Clients[_toClient].IsConnected) { NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length()); }
        }
    
        private static void SendTCPDataToAll(USNL.Package.Packet _packet) {
            _packet.WriteLength();
            for (int i = 0; i < USNL.Package.Server.MaxClients; i++) {
                USNL.Package.Server.Clients[i].Tcp.SendData(_packet);
                if (USNL.Package.Server.Clients[i].IsConnected) { NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length()); }
            }
        }
    
        private static void SendTCPDataToAll(int _excpetClient, USNL.Package.Packet _packet) {
            _packet.WriteLength();
            for (int i = 0; i < USNL.Package.Server.MaxClients; i++) {
                if (i != _excpetClient) {
                    USNL.Package.Server.Clients[i].Tcp.SendData(_packet);
                    if (USNL.Package.Server.Clients[i].IsConnected) { NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length()); }
                }
            }
        }
    
        private static void SendUDPData(int _toClient, USNL.Package.Packet _packet) {
            _packet.WriteLength();
            USNL.Package.Server.Clients[_toClient].Udp.SendData(_packet);
            if (USNL.Package.Server.Clients[_toClient].IsConnected) { NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length()); }
        }
    
        private static void SendUDPDataToAll(USNL.Package.Packet _packet) {
            _packet.WriteLength();
            for (int i = 0; i < USNL.Package.Server.MaxClients; i++) {
                USNL.Package.Server.Clients[i].Udp.SendData(_packet);
                if (USNL.Package.Server.Clients[i].IsConnected) { NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length()); }
            }
        }
    
        private static void SendUDPDataToAll(int _excpetClient, USNL.Package.Packet _packet) {
            _packet.WriteLength();
            for (int i = 0; i < USNL.Package.Server.MaxClients; i++) {
                if (i != _excpetClient) {
                    USNL.Package.Server.Clients[i].Udp.SendData(_packet);
                    if (USNL.Package.Server.Clients[i].IsConnected) { NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length()); }
                }
            }
        }
    
        #endregion
    
        public static void Welcome(int _toClient, string _welcomeMessage, string _serverName, int _clientId) {
            using (USNL.Package.Packet _packet = new USNL.Package.Packet((int)ServerPackets.Welcome)) {
                _packet.Write(_welcomeMessage);
                _packet.Write(_serverName);
                _packet.Write(_clientId);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void Ping(int _toClient, bool _placeholder) {
            using (USNL.Package.Packet _packet = new USNL.Package.Packet((int)ServerPackets.Ping)) {
                _packet.Write(_placeholder);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void ServerInfo(int _toClient, string _serverName, int[] _connectedClientsIds, int _maxClients, bool _serverFull) {
            using (USNL.Package.Packet _packet = new USNL.Package.Packet((int)ServerPackets.ServerInfo)) {
                _packet.Write(_serverName);
                _packet.Write(_connectedClientsIds);
                _packet.Write(_maxClients);
                _packet.Write(_serverFull);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void DisconnectClient(int _toClient, string _disconnectMessage) {
            using (USNL.Package.Packet _packet = new USNL.Package.Packet((int)ServerPackets.DisconnectClient)) {
                _packet.Write(_disconnectMessage);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void SyncedObjectInstantiate(int _toClient, string _syncedObjectTag, int _syncedObjectUUID, Vector3 _position, Quaternion _rotation, Vector3 _scale) {
            using (USNL.Package.Packet _packet = new USNL.Package.Packet((int)ServerPackets.SyncedObjectInstantiate)) {
                _packet.Write(_syncedObjectTag);
                _packet.Write(_syncedObjectUUID);
                _packet.Write(_position);
                _packet.Write(_rotation);
                _packet.Write(_scale);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void SyncedObjectDestroy(int _toClient, int _syncedObjectUUID) {
            using (USNL.Package.Packet _packet = new USNL.Package.Packet((int)ServerPackets.SyncedObjectDestroy)) {
                _packet.Write(_syncedObjectUUID);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void SyncedObjectInterpolationMode(int _toClient, bool _serverInterpolation) {
            using (USNL.Package.Packet _packet = new USNL.Package.Packet((int)ServerPackets.SyncedObjectInterpolationMode)) {
                _packet.Write(_serverInterpolation);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void SyncedObjectVec2PosUpdate(int[] _syncedObjectUUIDs, Vector2[] _positions) {
            using (USNL.Package.Packet _packet = new USNL.Package.Packet((int)ServerPackets.SyncedObjectVec2PosUpdate)) {
                _packet.Write(_syncedObjectUUIDs);
                _packet.Write(_positions);

                SendUDPDataToAll(_packet);
            }
        }

        public static void SyncedObjectVec3PosUpdate(int[] _syncedObjectUUIDs, Vector3[] _positions) {
            using (USNL.Package.Packet _packet = new USNL.Package.Packet((int)ServerPackets.SyncedObjectVec3PosUpdate)) {
                _packet.Write(_syncedObjectUUIDs);
                _packet.Write(_positions);

                SendUDPDataToAll(_packet);
            }
        }

        public static void SyncedObjectRotZUpdate(int[] _syncedObjectUUIDs, float[] _rotations) {
            using (USNL.Package.Packet _packet = new USNL.Package.Packet((int)ServerPackets.SyncedObjectRotZUpdate)) {
                _packet.Write(_syncedObjectUUIDs);
                _packet.Write(_rotations);

                SendUDPDataToAll(_packet);
            }
        }

        public static void SyncedObjectRotUpdate(int[] _syncedObjectUUIDs, Vector3[] _rotations) {
            using (USNL.Package.Packet _packet = new USNL.Package.Packet((int)ServerPackets.SyncedObjectRotUpdate)) {
                _packet.Write(_syncedObjectUUIDs);
                _packet.Write(_rotations);

                SendUDPDataToAll(_packet);
            }
        }

        public static void SyncedObjectVec2ScaleUpdate(int[] _syncedObjectUUIDs, Vector2[] _scales) {
            using (USNL.Package.Packet _packet = new USNL.Package.Packet((int)ServerPackets.SyncedObjectVec2ScaleUpdate)) {
                _packet.Write(_syncedObjectUUIDs);
                _packet.Write(_scales);

                SendUDPDataToAll(_packet);
            }
        }

        public static void SyncedObjectVec3ScaleUpdate(int[] _syncedObjectUUIDs, Vector3[] _scales) {
            using (USNL.Package.Packet _packet = new USNL.Package.Packet((int)ServerPackets.SyncedObjectVec3ScaleUpdate)) {
                _packet.Write(_syncedObjectUUIDs);
                _packet.Write(_scales);

                SendUDPDataToAll(_packet);
            }
        }

        public static void SyncedObjectVec2PosInterpolation(int[] _syncedObjectUUIDs, Vector2[] _interpolatePositions) {
            using (USNL.Package.Packet _packet = new USNL.Package.Packet((int)ServerPackets.SyncedObjectVec2PosInterpolation)) {
                _packet.Write(_syncedObjectUUIDs);
                _packet.Write(_interpolatePositions);

                SendUDPDataToAll(_packet);
            }
        }

        public static void SyncedObjectVec3PosInterpolation(int[] _syncedObjectUUIDs, Vector3[] _interpolatePositions) {
            using (USNL.Package.Packet _packet = new USNL.Package.Packet((int)ServerPackets.SyncedObjectVec3PosInterpolation)) {
                _packet.Write(_syncedObjectUUIDs);
                _packet.Write(_interpolatePositions);

                SendUDPDataToAll(_packet);
            }
        }

        public static void SyncedObjectRotZInterpolation(int[] _syncedObjectUUIDs, float[] _interpolateRotations) {
            using (USNL.Package.Packet _packet = new USNL.Package.Packet((int)ServerPackets.SyncedObjectRotZInterpolation)) {
                _packet.Write(_syncedObjectUUIDs);
                _packet.Write(_interpolateRotations);

                SendUDPDataToAll(_packet);
            }
        }

        public static void SyncedObjectRotInterpolation(int[] _syncedObjectUUIDs, Vector3[] _interpolateRotations) {
            using (USNL.Package.Packet _packet = new USNL.Package.Packet((int)ServerPackets.SyncedObjectRotInterpolation)) {
                _packet.Write(_syncedObjectUUIDs);
                _packet.Write(_interpolateRotations);

                SendUDPDataToAll(_packet);
            }
        }

        public static void SyncedObjectVec2ScaleInterpolation(int[] _syncedObjectUUIDs, Vector2[] _interpolateScales) {
            using (USNL.Package.Packet _packet = new USNL.Package.Packet((int)ServerPackets.SyncedObjectVec2ScaleInterpolation)) {
                _packet.Write(_syncedObjectUUIDs);
                _packet.Write(_interpolateScales);

                SendUDPDataToAll(_packet);
            }
        }

        public static void SyncedObjectVec3ScaleInterpolation(int[] _syncedObjectUUIDs, Vector3[] _interpolateScales) {
            using (USNL.Package.Packet _packet = new USNL.Package.Packet((int)ServerPackets.SyncedObjectVec3ScaleInterpolation)) {
                _packet.Write(_syncedObjectUUIDs);
                _packet.Write(_interpolateScales);

                SendUDPDataToAll(_packet);
            }
        }
    }

    #endregion
}

#endregion Packets

#region Callbacks

namespace USNL {
    public static class CallbackEvents {
        public delegate void CallbackEvent(object _param);

        public static CallbackEvent[] PacketCallbackEvents = {
            CallOnWelcomeReceivedPacketCallbacks,
            CallOnPingPacketCallbacks,
            CallOnClientInputPacketCallbacks,
            CallOnRaycastFromCameraPacketCallbacks,
        };

        public static event CallbackEvent OnServerStarted;
        public static event CallbackEvent OnServerStopped;
        public static event CallbackEvent OnClientConnected;
        public static event CallbackEvent OnClientDisconnected;

        public static event CallbackEvent OnWelcomeReceivedPacket;
        public static event CallbackEvent OnPingPacket;
        public static event CallbackEvent OnClientInputPacket;
        public static event CallbackEvent OnRaycastFromCameraPacket;

        public static void CallOnServerStartedCallbacks(object _param) { if (OnServerStarted != null) { OnServerStarted(_param); } }
        public static void CallOnServerStoppedCallbacks(object _param) { if (OnServerStopped != null) { OnServerStopped(_param); } }
        public static void CallOnClientConnectedCallbacks(object _param) { if (OnClientConnected != null) { OnClientConnected(_param); } }
        public static void CallOnClientDisconnectedCallbacks(object _param) { if (OnClientDisconnected != null) { OnClientDisconnected(_param); } }

        public static void CallOnWelcomeReceivedPacketCallbacks(object _param) { if (OnWelcomeReceivedPacket != null) { OnWelcomeReceivedPacket(_param); } }
        public static void CallOnPingPacketCallbacks(object _param) { if (OnPingPacket != null) { OnPingPacket(_param); } }
        public static void CallOnClientInputPacketCallbacks(object _param) { if (OnClientInputPacket != null) { OnClientInputPacket(_param); } }
        public static void CallOnRaycastFromCameraPacketCallbacks(object _param) { if (OnRaycastFromCameraPacket != null) { OnRaycastFromCameraPacket(_param); } }
    }
}

#endregion
