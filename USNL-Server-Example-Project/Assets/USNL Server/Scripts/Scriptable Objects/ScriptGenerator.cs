using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace USNL.Package {
    [CreateAssetMenu(fileName = "ScriptGenerator", menuName = "USNL/Script Generator", order = 0)]
    public class ScriptGenerator : ScriptableObject {
        #region Variables

        [Header("Packets")]
        [SerializeField] private ClientPacketConfig[] clientPackets;
        [SerializeField] private ServerPacketConfig[] serverPackets;

        private string usnlPath = "Assets/USNL Server/Scripts/";

        // Custom Callbacks for library functions (Specified by me)
        private string[] libCallbacks = {
        "OnServerStarted",
        "OnServerStopped",
        "OnClientConnected",
        "OnClientDisconnected"
    };

        // For editor script:
        public bool[] ClientPacketFoldouts;
        public bool[] ServerPacketFoldouts;

        public ClientPacketConfig[] ClientPackets { get => clientPackets; set => clientPackets = value; }
        public ServerPacketConfig[] ServerPackets { get => serverPackets; set => serverPackets = value; }

        #region Packet Variables

        /*** Library Packets (Not for user) ***/
        private ServerPacketConfig[] libServerPackets = {
        new ServerPacketConfig(
            "Welcome",
            new PacketVariable[] { new PacketVariable("lobbyClientId", PacketVarType.Int), new PacketVariable("welcomeMessage", PacketVarType.String) },
            ServerPacketType.SendToClient,
            Protocol.TCP),
        new ServerPacketConfig(
            "ConnectReceived",
            new PacketVariable[] { new PacketVariable("clientId", PacketVarType.Int), new PacketVariable("connectMessage", PacketVarType.String) },
            ServerPacketType.SendToClient,
            Protocol.TCP),
        new ServerPacketConfig(
            "ServerInfo",
            new PacketVariable[] { new PacketVariable("Server Name", PacketVarType.String), new PacketVariable("connectedClientsIds", PacketVarType.IntArray), new PacketVariable("maxClients", PacketVarType.Int), new PacketVariable("serverFull", PacketVarType.Bool) },
            ServerPacketType.SendToClient,
            Protocol.TCP),
        new ServerPacketConfig(
            "Ping",
            new PacketVariable[] { new PacketVariable("Send Ping Back", PacketVarType.Bool) },
            ServerPacketType.SendToClient,
            Protocol.TCP),
        new ServerPacketConfig(
            "DisconnectClient",
            new PacketVariable[] { new PacketVariable("Disconnect Message", PacketVarType.String) },
            ServerPacketType.SendToClient,
            Protocol.TCP),
        #region Synced Objects
        new ServerPacketConfig(
            "SyncedObjectInstantiate",
            new PacketVariable[] { new PacketVariable("Synced Object Tag", PacketVarType.String), new PacketVariable("Synced Object UUID", PacketVarType.Int), new PacketVariable("Position", PacketVarType.Vector3), new PacketVariable("Rotation", PacketVarType.Quaternion), new PacketVariable("Scale", PacketVarType.Vector3) },
            ServerPacketType.SendToClient,
            Protocol.TCP),
        new ServerPacketConfig(
            "SyncedObjectDestroy",
            new PacketVariable[] { new PacketVariable("Synced Object UUID", PacketVarType.Int) },
            ServerPacketType.SendToClient,
            Protocol.TCP),
        new ServerPacketConfig(
            "SyncedObjectInterpolationMode",
            new PacketVariable[] { new PacketVariable("Server Interpolation", PacketVarType.Bool) },
            ServerPacketType.SendToClient,
            Protocol.TCP
            ),
        #region Synced Object Updates
        new ServerPacketConfig(
            "SyncedObjectVec2PosUpdate",
            new PacketVariable[] { new PacketVariable("Synced Object UUIDs", PacketVarType.IntArray), new PacketVariable("Positions", PacketVarType.Vector2Array) },
            ServerPacketType.SendToAllClients,
            Protocol.UDP),
        new ServerPacketConfig(
            "SyncedObjectVec3PosUpdate",
            new PacketVariable[] { new PacketVariable("Synced Object UUIDs", PacketVarType.IntArray), new PacketVariable("Positions", PacketVarType.Vector3Array) },
            ServerPacketType.SendToAllClients,
            Protocol.UDP),
        new ServerPacketConfig(
            "SyncedObjectRotZUpdate",
            new PacketVariable[] { new PacketVariable("Synced Object UUIDs", PacketVarType.IntArray), new PacketVariable("Rotations", PacketVarType.FloatArray) },
            ServerPacketType.SendToAllClients,
            Protocol.UDP),
        new ServerPacketConfig(
            "SyncedObjectRotUpdate",
            new PacketVariable[] { new PacketVariable("Synced Object UUIDs", PacketVarType.IntArray), new PacketVariable("Rotations", PacketVarType.Vector3Array) },
            ServerPacketType.SendToAllClients,
            Protocol.UDP),
        new ServerPacketConfig(
            "SyncedObjectVec2ScaleUpdate",
            new PacketVariable[] { new PacketVariable("Synced Object UUIDs", PacketVarType.IntArray), new PacketVariable("Scales", PacketVarType.Vector2Array) },
            ServerPacketType.SendToAllClients,
            Protocol.UDP),
        new ServerPacketConfig(
            "SyncedObjectVec3ScaleUpdate",
            new PacketVariable[] { new PacketVariable("Synced Object UUIDs", PacketVarType.IntArray), new PacketVariable("Scales", PacketVarType.Vector3Array) },
            ServerPacketType.SendToAllClients,
            Protocol.UDP),
        #endregion
        #region Synced Object Interpolation
        new ServerPacketConfig(
            "SyncedObjectVec2PosInterpolation",
            new PacketVariable[] { new PacketVariable("Synced Object UUIDs", PacketVarType.IntArray), new PacketVariable("InterpolatePositions", PacketVarType.Vector2Array) },
            ServerPacketType.SendToAllClients,
            Protocol.UDP),
        new ServerPacketConfig(
            "SyncedObjectVec3PosInterpolation",
            new PacketVariable[] { new PacketVariable("Synced Object UUIDs", PacketVarType.IntArray), new PacketVariable("InterpolatePositions", PacketVarType.Vector3Array) },
            ServerPacketType.SendToAllClients,
            Protocol.UDP),
        new ServerPacketConfig(
            "SyncedObjectRotZInterpolation",
            new PacketVariable[] { new PacketVariable("Synced Object UUIDs", PacketVarType.IntArray), new PacketVariable("InterpolateRotations", PacketVarType.FloatArray) },
            ServerPacketType.SendToAllClients,
            Protocol.UDP),
        new ServerPacketConfig(
            "SyncedObjectRotInterpolation",
            new PacketVariable[] { new PacketVariable("Synced Object UUIDs", PacketVarType.IntArray), new PacketVariable("InterpolateRotations", PacketVarType.Vector3Array) },
            ServerPacketType.SendToAllClients,
            Protocol.UDP),
        new ServerPacketConfig(
            "SyncedObjectVec2ScaleInterpolation",
            new PacketVariable[] { new PacketVariable("Synced Object UUIDs", PacketVarType.IntArray), new PacketVariable("InterpolateScales", PacketVarType.Vector2Array) },
            ServerPacketType.SendToAllClients,
            Protocol.UDP),
        new ServerPacketConfig(
            "SyncedObjectVec3ScaleInterpolation",
            new PacketVariable[] { new PacketVariable("Synced Object UUIDs", PacketVarType.IntArray), new PacketVariable("InterpolateScales", PacketVarType.Vector3Array) },
            ServerPacketType.SendToAllClients,
            Protocol.UDP),
        #endregion
        #endregion
        };
        private ClientPacketConfig[] libClientPackets = {
        new ClientPacketConfig(
            "WelcomeReceived",
            new PacketVariable[] { new PacketVariable("lobbyClientIdCheck", PacketVarType.Int) },
            Protocol.TCP),
        new ClientPacketConfig(
            "Connect",
            new PacketVariable[] { new PacketVariable("VariablesListCantBeNull", PacketVarType.Int) },
            Protocol.TCP),
        new ClientPacketConfig(
            "ConnectionConfirmed",
            new PacketVariable[] { new PacketVariable("clientIdCheck", PacketVarType.Int) },
            Protocol.TCP),
        new ClientPacketConfig(
            "Ping",
            new PacketVariable[] { new PacketVariable("Send Ping Back", PacketVarType.Bool) },
            Protocol.TCP),
        new ClientPacketConfig(
            "ClientInput",
            new PacketVariable[] { new PacketVariable("KeycodesDown", PacketVarType.IntArray), new PacketVariable("KeycodesUp", PacketVarType.IntArray) },
            Protocol.TCP),
        };

        Dictionary<PacketVarType, string> packetTypes = new Dictionary<PacketVarType, string>()
        { { PacketVarType.Byte, "byte"},
    { PacketVarType.Short, "short"},
    { PacketVarType.Int, "int"},
    { PacketVarType.Long, "long"},
    { PacketVarType.Float, "float"},
    { PacketVarType.Bool, "bool"},
    { PacketVarType.String, "string"},
    { PacketVarType.Vector2, "Vector2"},
    { PacketVarType.Vector3, "Vector3"},
    { PacketVarType.Quaternion, "Quaternion"},
    { PacketVarType.ByteArray, "byte[]"},
    { PacketVarType.ShortArray, "short[]"},
    { PacketVarType.IntArray, "int[]"},
    { PacketVarType.LongArray, "long[]"},
    { PacketVarType.FloatArray, "float[]"},
    { PacketVarType.BoolArray, "bool[]"},
    { PacketVarType.StringArray, "string[]"},
    { PacketVarType.Vector2Array, "Vector2[]"},
    { PacketVarType.Vector3Array, "Vector3[]"},
    { PacketVarType.QuaternionArray, "Quaternion[]"},
    };
        Dictionary<PacketVarType, string> packetReadTypes = new Dictionary<PacketVarType, string>()
        { { PacketVarType.Byte, "Byte"},
    { PacketVarType.Short, "Short"},
    { PacketVarType.Int, "Int"},
    { PacketVarType.Long, "Long"},
    { PacketVarType.Float, "Float"},
    { PacketVarType.Bool, "Bool"},
    { PacketVarType.String, "String"},
    { PacketVarType.Vector2, "Vector2"},
    { PacketVarType.Vector3, "Vector3"},
    { PacketVarType.Quaternion, "Quaternion"},
    { PacketVarType.ByteArray, "Bytes"},
    { PacketVarType.ShortArray, "Shorts"},
    { PacketVarType.IntArray, "Ints"},
    { PacketVarType.LongArray, "Longs"},
    { PacketVarType.FloatArray, "Floats"},
    { PacketVarType.BoolArray, "Bools"},
    { PacketVarType.StringArray, "Strings"},
    { PacketVarType.Vector2Array, "Vector2s"},
    { PacketVarType.Vector3Array, "Vector3s"},
    { PacketVarType.QuaternionArray, "Quaternions"}
    };

        public enum PacketVarType {
            Byte,
            Short,
            Int,
            Long,
            Float,
            Bool,
            String,
            Vector2,
            Vector3,
            Quaternion,
            ByteArray,
            ShortArray,
            IntArray,
            LongArray,
            FloatArray,
            BoolArray,
            StringArray,
            Vector2Array,
            Vector3Array,
            QuaternionArray
        }
        public enum ServerPacketType {
            SendToClient,
            SendToAllClients,
            SendToAllClientsExcept
        }
        public enum Protocol {
            TCP,
            UDP
        }

        [Serializable]
        public struct PacketVariable {
            [SerializeField] private string variableName;
            [SerializeField] private PacketVarType variableType;

            public PacketVariable(string variableName, PacketVarType variableType) {
                this.variableName = variableName;
                this.variableType = variableType;
            }

            public string VariableName { get => variableName; set => variableName = value; }
            public PacketVarType VariableType { get => variableType; set => variableType = value; }
        }

        [Serializable]
        public struct ServerPacketConfig {
            [Tooltip("Can be done in any formatting (eg. 'exampleName' & 'Example Name' both work)")]
            [SerializeField] private string packetName;
            [SerializeField] private PacketVariable[] packetVariables;
            [Tooltip("SendToClient: Send Packet to a single client specified by you.\n" +
                "SendToAllClients: Send Packet to all clients.\n" +
                "SendToAllClientsExcept: Send Packet to all clients execpt one specified by you.")]
            [SerializeField] private ServerPacketType sendType;
            [SerializeField] private Protocol protocol;
            [Space]
            [Tooltip("This is only for the user.")]
            [SerializeField] private string notes;

            public ServerPacketConfig(string packetName, PacketVariable[] packetVariables, ServerPacketType sendType, Protocol protocol) : this() {
                this.packetName = packetName;
                this.packetVariables = packetVariables;
                this.sendType = sendType;
                this.protocol = protocol;
            }

            public string PacketName { get => packetName; set => packetName = value; }
            public PacketVariable[] PacketVariables { get => packetVariables; set => packetVariables = value; }
            public ServerPacketType SendType { get => sendType; set => sendType = value; }
            public Protocol Protocol { get => protocol; set => protocol = value; }
        }

        [Serializable]
        public struct ClientPacketConfig {
            [Tooltip("Can be done in any formatting (eg. 'exampleName' & 'Example Name' both work)")]
            [SerializeField] private string packetName; // In C# formatting: eg. "examplePacket"
            [SerializeField] private PacketVariable[] packetVariables;
            [SerializeField] private Protocol protocol;
            [Space]
            [Tooltip("This is only for the user.")]
            [SerializeField] private string notes;

            public ClientPacketConfig(string packetName, PacketVariable[] packetVariables, Protocol protocol) : this() {
                this.packetName = packetName;
                this.packetVariables = packetVariables;
                this.protocol = protocol;
            }

            public string PacketName { get => packetName; set => packetName = value; }
            public PacketVariable[] PacketVariables { get => packetVariables; set => packetVariables = value; }
            public Protocol Protocol { get => protocol; set => protocol = value; }
        }

        #endregion

        #endregion

        #region Generation

        public void GenerateScript() {
            if (!CheckUserPacketsValid()) return;

            string scriptText = "";

            #region Using Statements
            scriptText +=
                "using System.Collections.Generic;" +
                "\nusing UnityEngine;" +
                "\n";
            #endregion

            #region Packets
            scriptText +=
                "\n#region Packets" +
                $"\n{GeneratePacketText()}" +
                "\n" +
                "\n#endregion Packets" +
                "\n";
            #endregion

            #region USNL Callback Events
            scriptText +=
                $"\n{GenerateUSNLCallbackEventsText()}" +
                "\n";
            #endregion

            StreamWriter sw = new StreamWriter($"{usnlPath}USNLGenerated.cs");
            sw.Write(scriptText);
            sw.Flush();
            sw.Close();
        }

        private string GeneratePacketText() {
            ServerPacketConfig[] _serverPackets = new ServerPacketConfig[libServerPackets.Length + serverPackets.Length];
            ClientPacketConfig[] _clientPackets = new ClientPacketConfig[libClientPackets.Length + clientPackets.Length];

            for (int i = 0; i < libServerPackets.Length; i++) { _serverPackets[i] = libServerPackets[i]; }
            for (int i = 0; i < serverPackets.Length; i++) { _serverPackets[i + libServerPackets.Length] = serverPackets[i]; }
            for (int i = 0; i < libClientPackets.Length; i++) { _clientPackets[i] = libClientPackets[i]; }
            for (int i = 0; i < clientPackets.Length; i++) { _clientPackets[i + libClientPackets.Length] = clientPackets[i]; }

            string serverPacketsString = GeneratePacketEnums(_serverPackets);
            string clientPacketsString = GeneratePacketEnums(_clientPackets);

            string packetStructs = GeneratePacketStructs(clientPackets);
            string pPacketStructs = GeneratePacketStructs(libClientPackets);

            string packetHandlers = GeneratePacketHandlers(_clientPackets);

            string packetSends = GeneratePacketSends(serverPackets, "USNL.");
            string pPacketSends = GeneratePacketSends(libServerPackets, "USNL.Package.");

            return
                "namespace USNL {" +
                "\n    #region Packet Enums" +
                "\n" +
                "\n    public enum ClientPackets {" +
                $"\n{clientPacketsString}" +
                "\n    }" +
                "\n" +
                "\n    public enum ServerPackets {" +
                $"\n{serverPacketsString}" +
                "\n    }" +
                "\n" +
                "\n    #endregion" +
                "\n" +
                "\n    #region Packet Structs" +
                $"\n{packetStructs}" +
                "\n" +
                "\n    #endregion" +
                "\n" +
                "\n    #region Packet Send" +
                "\n" +
                "\n    public static class PacketSend {" +
                $"\n{packetSends}" +
                "\n        }" +
                "\n" +
                $"\n    #endregion" +
                "\n}" +
                "\n" +
                "\nnamespace USNL.Package {" +
                "\n    #region Packet Enums" +
                "\n    public enum ClientPackets {" +
                $"\n{clientPacketsString}" +
                "\n    }" +
                "\n" +
                "\n    public enum ServerPackets {" +
                $"\n{serverPacketsString}" +
                "\n    }" +
                "\n    #endregion" +
                "\n" +
                "\n    #region Packet Structs" +
                $"\n{pPacketStructs}" +
                "\n" +
                "\n    #endregion" +
                "\n" +
                "\n    #region Packet Handlers" +
                "\n" +
                "\n    public static class PacketHandlers {" +
                $"\n{packetHandlers}" +
                "\n    }" +
                "\n" +
                "\n    #endregion" +
                "\n" +
                "\n    #region Packet Send" +
                "\n" +
                "\n    public static class PacketSend {" +
                $"\n{pPacketSends}" +
                "\n    }" +
                "\n" +
                $"\n    #endregion" +
                "\n}";
        }

        private string GenerateUSNLCallbackEventsText() {
            ClientPacketConfig[] _clientPackets = new ClientPacketConfig[libClientPackets.Length + clientPackets.Length];

            libClientPackets.CopyTo(_clientPackets, 0);
            clientPackets.CopyTo(_clientPackets, libClientPackets.Length);

            // Function declaration
            string output = "#region Callbacks\n" +
                "\nnamespace USNL {" +
                "\n    public static class CallbackEvents {" +
                "\n        public delegate void CallbackEvent(object _param);" +
                "\n";

            // Packet Callback Events for Packet Handling
            output += "\n        public static CallbackEvent[] PacketCallbackEvents = {";
            for (int i = 0; i < _clientPackets.Length; i++) {
                output += $"\n            CallOn{Upper(_clientPackets[i].PacketName)}PacketCallbacks,";
            }
            output += "\n        };";
            output += "\n";

            // Standard Callback events
            for (int i = 0; i < libCallbacks.Length; i++) {
                output += $"\n        public static event CallbackEvent {libCallbacks[i]};";
            }
            output += "\n";

            // Packet Callback events
            for (int i = 0; i < _clientPackets.Length; i++) {
                output += $"\n        public static event CallbackEvent On{Upper(_clientPackets[i].PacketName)}Packet;";
            }

            output += "\n";

            // Standard Callback Functions
            for (int i = 0; i < libCallbacks.Length; i++) {
                output += $"\n        public static void Call{libCallbacks[i]}Callbacks(object _param) {{ if ({libCallbacks[i]} != null) {{ {libCallbacks[i]}(_param); }} }}";
            }
            output += "\n";

            // Packet Callback Functions
            for (int i = 0; i < _clientPackets.Length; i++) {
                output += $"\n        public static void CallOn{Upper(_clientPackets[i].PacketName)}PacketCallbacks(object _param) {{ if (On{Upper(_clientPackets[i].PacketName)}Packet != null) {{ On{Upper(_clientPackets[i].PacketName)}Packet(_param); }} }}";
            }

            output += "\n    }";
            output += "\n}";
            output += "\n";
            output += "\n#endregion";

            return output;
        }

        #endregion

        #region Generation Utils

        private string GeneratePacketEnums(ClientPacketConfig[] cpcs) {
            string cps = ""; // Client packets string
            for (int i = 0; i < cpcs.Length; i++) {
                cps += $"\n        {Upper(cpcs[i].PacketName.ToString())},";
            }
            cps = cps.Substring(1, cps.Length - 1); // Remove last ", " & first \n
            return cps;
        }

        private string GeneratePacketEnums(ServerPacketConfig[] spcs) {
            string sps = ""; // Server packets string
            for (int i = 0; i < spcs.Length; i++) {
                sps += $"\n        {Upper(spcs[i].PacketName.ToString())},";
            }
            sps = sps.Substring(1, sps.Length - 1); // Remove last ", " & first \n
            return sps;
        }

        private string GeneratePacketStructs(ClientPacketConfig[] cpcs) {
            string psts = ""; // Packet Structs String
            for (int i = 0; i < cpcs.Length; i++) {
                psts += $"\n    public struct {Upper(cpcs[i].PacketName.ToString())}Packet {{";

                psts += $"\n        private int fromClient;";
                psts += "\n";
                for (int x = 0; x < cpcs[i].PacketVariables.Length; x++) {
                    string varName = Lower(cpcs[i].PacketVariables[x].VariableName); // Lower case variable name (C# formatting)
                    string varType = packetTypes[cpcs[i].PacketVariables[x].VariableType]; // Variable type string
                    psts += $"\n        private {varType} {varName};";
                }

                // Constructor:
                psts += "\n";
                string constructorParameters = "int _fromClient, ";
                for (int x = 0; x < cpcs[i].PacketVariables.Length; x++) {
                    constructorParameters += $"{packetTypes[cpcs[i].PacketVariables[x].VariableType]} _{Lower(cpcs[i].PacketVariables[x].VariableName)}, ";
                }
                constructorParameters = constructorParameters.Substring(0, constructorParameters.Length - 2);

                psts += $"\n        public {Upper(cpcs[i].PacketName)}Packet({constructorParameters}) {{";
                psts += "\n            fromClient = _fromClient;";
                for (int x = 0; x < cpcs[i].PacketVariables.Length; x++) {
                    psts += $"\n            {Lower(cpcs[i].PacketVariables[x].VariableName)} = _{Lower(cpcs[i].PacketVariables[x].VariableName)};";
                }
                psts += "\n        }";
                psts += "\n";


                psts += "\n        public int FromClient { get => fromClient; set => fromClient = value; }";
                for (int x = 0; x < cpcs[i].PacketVariables.Length; x++) {
                    string varName = Lower(cpcs[i].PacketVariables[x].VariableName); // Lower case variable name (C# formatting)
                    string varType = packetTypes[cpcs[i].PacketVariables[x].VariableType]; // Variable type string

                    psts += $"\n        public {varType} {Upper(varName)} {{ get => {varName}; set => {varName} = value; }}";
                }
                psts += "\n    }";
                psts += "\n";
            }
            return psts;
        }

        private string GeneratePacketHandlers(ClientPacketConfig[] spcs) {
            string phs = ""; // Packet Handlers String

            phs += "        public delegate void PacketHandler(USNL.Package.Packet _packet);";
            phs = phs.Substring(1, phs.Length - 1);
            phs += "\n        public static List<PacketHandler> packetHandlers = new List<PacketHandler>() {";
            string packetHandlerFunctionsString = "";
            for (int i = 0; i < spcs.Length; i++) phs += $"\n            {{ {Upper(spcs[i].PacketName)} }},";
            phs += packetHandlerFunctionsString;
            phs += "\n        };";

            phs += "\n";

            for (int i = 0; i < spcs.Length; i++) {
                string upPacketName = Upper(spcs[i].PacketName);
                string loPacketName = Lower(spcs[i].PacketName);
                phs += $"\n        public static void {upPacketName}(Packet _packet) {{";

                for (int x = 0; x < spcs[i].PacketVariables.Length; x++) {
                    phs += $"\n            {packetTypes[spcs[i].PacketVariables[x].VariableType]} {Lower(spcs[i].PacketVariables[x].VariableName)} = _packet.Read{packetReadTypes[spcs[i].PacketVariables[x].VariableType]}();";
                }

                phs += "\n";

                string packetParameters = "";
                packetParameters += "_packet.FromClient, ";
                for (int x = 0; x < spcs[i].PacketVariables.Length; x++) {
                    packetParameters += $"{Lower(spcs[i].PacketVariables[x].VariableName)}, ";
                }
                packetParameters = packetParameters.Substring(0, packetParameters.Length - 2);

                phs += $"\n            {upPacketName}Packet {loPacketName}Packet = new {upPacketName}Packet({packetParameters});";
                phs += $"\n            PacketManager.instance.PacketReceived(_packet, {loPacketName}Packet);";

                phs += "\n        }\n";
            }
            if (spcs.Length > 0) phs = phs.Substring(0, phs.Length - 1); // Remove last /n
            return phs;
        }

        private string GeneratePacketSends(ServerPacketConfig[] spcs, string usingStatement) {
            #region TcpUdpSendFunctionsString

            string TcpUdpSendFunctionsString = "        #region TCP & UDP Send Functions" +
                "\n    " +
                "\n        private static void SendTCPData(int _toClient, USNL.Package.Packet _packet) {" +
                "\n            _packet.WriteLength();" +
                "\n    " +
                "\n            Client client = null;" +
                "\n            if (_toClient >= 1000000) client = USNL.Package.Server.WaitingLobbyClients[_toClient % 1000000];" +
                "\n            else client = USNL.Package.Server.Clients[_toClient];" +
                "\n    " +
                "\n            client.Tcp.SendData(_packet);" +
                "\n            if (client.IsConnected) { NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length()); }" +
                "\n        }" +
                "\n    " +
                "\n        private static void SendTCPDataToAll(USNL.Package.Packet _packet) {" +
                "\n            _packet.WriteLength();" +
                "\n            for (int i = 0; i < USNL.Package.Server.MaxClients; i++) {" +
                "\n                USNL.Package.Server.Clients[i].Tcp.SendData(_packet);" +
                "\n                if (USNL.Package.Server.Clients[i].IsConnected) { NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length()); }" +
                "\n            }" +
                "\n        }" +
                "\n    " +
                "\n        private static void SendTCPDataToAll(int _excpetClient, USNL.Package.Packet _packet) {" +
                "\n            _packet.WriteLength();" +
                "\n            for (int i = 0; i < USNL.Package.Server.MaxClients; i++) {" +
                "\n                if (i != _excpetClient) {" +
                "\n                    USNL.Package.Server.Clients[i].Tcp.SendData(_packet);" +
                "\n                    if (USNL.Package.Server.Clients[i].IsConnected) { NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length()); }" +
                "\n                }" +
                "\n            }" +
                "\n        }" +
                "\n    " +
                "\n        private static void SendUDPData(int _toClient, USNL.Package.Packet _packet) {" +
                "\n            _packet.WriteLength();" +
                "\n    " +
                "\n            Client client = null;" +
                "\n            if (_toClient >= 1000000) client = USNL.Package.Server.WaitingLobbyClients[_toClient % 1000000];" +
                "\n            else client = USNL.Package.Server.Clients[_toClient];" +
                "\n    " +
                "\n            client.Udp.SendData(_packet);" +
                "\n            if (client.IsConnected) { NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length()); }" +
                "\n        }" +
                "\n    " +
                "\n        private static void SendUDPDataToAll(USNL.Package.Packet _packet) {" +
                "\n            _packet.WriteLength();" +
                "\n            for (int i = 0; i < USNL.Package.Server.MaxClients; i++) {" +
                "\n                USNL.Package.Server.Clients[i].Udp.SendData(_packet);" +
                "\n                if (USNL.Package.Server.Clients[i].IsConnected) { NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length()); }" +
                "\n            }" +
                "\n        }" +
                "\n    " +
                "\n        private static void SendUDPDataToAll(int _excpetClient, USNL.Package.Packet _packet) {" +
                "\n            _packet.WriteLength();" +
                "\n            for (int i = 0; i < USNL.Package.Server.MaxClients; i++) {" +
                "\n                if (i != _excpetClient) {" +
                "\n                    USNL.Package.Server.Clients[i].Udp.SendData(_packet);" +
                "\n                    if (USNL.Package.Server.Clients[i].IsConnected) { NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length()); }" +
                "\n                }" +
                "\n            }" +
                "\n        }" +
                "\n    " +
                "\n        #endregion" +
                "\n    ";
            #endregion

            if (spcs.Length == 0) return "";

            string pss = TcpUdpSendFunctionsString;

            for (int i = 0; i < spcs.Length; i++) {
                string pas = ""; // Packet arguments (parameters) string
                if (spcs[i].SendType == ServerPacketType.SendToClient) { pas = "int _toClient, "; }
                if (spcs[i].SendType == ServerPacketType.SendToAllClientsExcept) { pas = "int _exceptClient, "; }
                for (int x = 0; x < spcs[i].PacketVariables.Length; x++) {
                    pas += $"{packetTypes[spcs[i].PacketVariables[x].VariableType]} _{Lower(spcs[i].PacketVariables[x].VariableName)}, ";
                }
                pas = pas.Substring(0, pas.Length - 2);

                pss += $"\n        public static void {Upper(spcs[i].PacketName)}({pas}) {{";
                pss += $"\n            using (USNL.Package.Packet _packet = new USNL.Package.Packet((int)ServerPackets.{Upper(spcs[i].PacketName)})) {{";

                string pws = ""; // Packet writes
                for (int x = 0; x < spcs[i].PacketVariables.Length; x++) {
                    pws += $"\n                _packet.Write(_{Lower(spcs[i].PacketVariables[x].VariableName)});";
                }
                pss += pws;
                pss += "\n";

                string protocolString = "TCP";
                if (spcs[i].Protocol == Protocol.UDP) { protocolString = "UDP"; }
                if (spcs[i].SendType == ServerPacketType.SendToClient) {
                    pss += $"\n                Send{protocolString}Data(_toClient, _packet);";
                } else if (spcs[i].SendType == ServerPacketType.SendToAllClientsExcept) {
                    pss += $"\n                Send{protocolString}DataToAll(_exceptClient, _packet);";
                } else {
                    pss += $"\n                Send{protocolString}DataToAll(_packet);";
                }

                pss += "\n            }\n        }\n"; // Close functions and using
            }
            pss = pss.Substring(0, pss.Length - 1); // Remove last /n
            return pss;
        }

        private bool CheckUserPacketsValid() {
            for (int i = 0; i < clientPackets.Length; i++) {
                if (clientPackets[i].PacketName == "") {
                    Debug.LogError("Packet name cannot be empty!");
                    return false;
                }
                if (clientPackets[i].PacketVariables.Length <= 0) {
                    Debug.LogError("Packet variables cannot be empty!");
                    return false;
                }
                for (int x = 0; x < clientPackets[i].PacketVariables.Length; x++) {
                    if (clientPackets[i].PacketVariables[x].VariableName == "" || clientPackets[i].PacketVariables[x].VariableName == null) {
                        Debug.LogError("Variable name cannot be empty!");
                        return false;
                    }
                }
            }

            for (int i = 0; i < serverPackets.Length; i++) {
                if (serverPackets[i].PacketName == "") {
                    Debug.LogError("Packet name cannot be empty!");
                    return false;
                }
                if (ServerPackets[i].PacketVariables.Length <= 0) {
                    Debug.LogError("Packet variables cannot be empty!");
                    return false;
                }
                for (int x = 0; x < serverPackets[i].PacketVariables.Length; x++) {
                    if (serverPackets[i].PacketVariables[x].VariableName == "" || serverPackets[i].PacketVariables[x].VariableName == null) {
                        Debug.LogError("Variable name cannot be empty!");
                        return false;
                    }
                }
            }
            return true;
        }

        private string Upper(string _input) {
            string output = String.Concat(_input.Where(c => !Char.IsWhiteSpace(c))); // Remove whitespace
            return $"{Char.ToUpper(output[0])}{output.Substring(1)}";
        }

        private string Lower(string _input) {
            string output = String.Concat(_input.Where(c => !Char.IsWhiteSpace(c))); // Remove whitespace
            return $"{Char.ToLower(output[0])}{output.Substring(1)}";
        }

        #endregion
    }
}
