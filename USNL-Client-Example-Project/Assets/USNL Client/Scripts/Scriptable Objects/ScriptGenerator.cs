using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "ScriptGenerator", menuName = "USNL/Script Generator", order = 0)]
public class ScriptGenerator : ScriptableObject {
    #region Variables
    
    [Header("Packets")]
    [SerializeField] private ClientPacketConfig[] clientPackets;
    [SerializeField] private ServerPacketConfig[] serverPackets;

    private string usnlPath = "Assets/USNL Client/Scripts/";

    // Custom Callbacks for library functions
    private string[] libCallbacks = {
        "OnConnected",
        "OnDisconnected"
    };

    public ClientPacketConfig[] ClientPackets { get => clientPackets; set => clientPackets = value; }
    public ServerPacketConfig[] ServerPackets { get => serverPackets; set => serverPackets = value; }

    #region Packet Generation Variables

    /*** Library Packets (Not for user) ***/
    private ServerPacketConfig[] libServerPackets = {
        new ServerPacketConfig(
            "Welcome",
            new PacketVariable[] { 
                new PacketVariable("Welcome Message", PacketVarType.String), new PacketVariable("Server Name", PacketVarType.String), new PacketVariable("Client Id", PacketVarType.Int) },
            ServerPacketType.SendToClient,
            Protocol.TCP),
        new ServerPacketConfig(
            "Ping",
            new PacketVariable[] { new PacketVariable("Send Ping Back", PacketVarType.Bool) },
            ServerPacketType.SendToClient,
            Protocol.TCP),
        #region Synced Objects
        new ServerPacketConfig(
            "SyncedObjectInstantiate",
            new PacketVariable[] {new PacketVariable("Synced Object Prefeb Id", PacketVarType.Int), new PacketVariable("Synced Object UUID", PacketVarType.Int), new PacketVariable("Position", PacketVarType.Vector3), new PacketVariable("Rotation", PacketVarType.Quaternion), new PacketVariable("Scale", PacketVarType.Vector3) },
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
            "Welcome Received",
            new PacketVariable[] { new PacketVariable("Client Id Check", PacketVarType.Int) },
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
        List<ServerPacketConfig> _serverPackets = new List<ServerPacketConfig>();
        List<ClientPacketConfig> _clientPackets = new List<ClientPacketConfig>();

        _serverPackets.AddRange(libServerPackets);
        if (serverPackets.Length > 0) _serverPackets.AddRange(serverPackets);
        _clientPackets.AddRange(libClientPackets);
        if (clientPackets.Length > 0) _clientPackets.AddRange(clientPackets);

        #region Server & Client Packet Enums

        string serverPacketsString = "";
        for (int i = 0; i < _serverPackets.Count; i++) {
            serverPacketsString += $"\n    {Upper(_serverPackets[i].PacketName.ToString())},";
        }
        serverPacketsString = serverPacketsString.Substring(1, serverPacketsString.Length - 1); // Remove last ", " & first \n

        string clientPacketsString = "";
        for (int i = 0; i < _clientPackets.Count; i++) {
            clientPacketsString += $"\n    {Upper(_clientPackets[i].PacketName.ToString())},";
        }
        clientPacketsString = clientPacketsString.Substring(1, clientPacketsString.Length - 1); // Remove last ", " & first \n

        #endregion

        #region Packet Structs

        string psts = ""; // Packet Structs String
        for (int i = 0; i < _serverPackets.Count; i++) {
            psts += $"\npublic struct {Upper(_serverPackets[i].PacketName.ToString())}Packet {{";

            psts += "";
            for (int x = 0; x < _serverPackets[i].PacketVariables.Length; x++) {
                string varName = Lower(_serverPackets[i].PacketVariables[x].VariableName); // Lower case variable name (C# formatting)
                string varType = packetTypes[_serverPackets[i].PacketVariables[x].VariableType]; // Variable type string
                psts += $"\n    private {varType} {varName};";
            }


            // Constructor:
            psts += "\n";
            string constructorParameters = "";
            for (int x = 0; x < _serverPackets[i].PacketVariables.Length; x++) {
                constructorParameters += $"{packetTypes[_serverPackets[i].PacketVariables[x].VariableType]} _{Lower(_serverPackets[i].PacketVariables[x].VariableName)}, ";
            }
            constructorParameters = constructorParameters.Substring(0, constructorParameters.Length - 2);

            psts += $"\n    public {Upper(_serverPackets[i].PacketName)}Packet({constructorParameters}) {{";
            for (int x = 0; x < _serverPackets[i].PacketVariables.Length; x++) {
                psts += $"\n        {Lower(_serverPackets[i].PacketVariables[x].VariableName)} = _{Lower(_serverPackets[i].PacketVariables[x].VariableName)};";
            }
            psts += "\n    }";
            psts += "\n";


            for (int x = 0; x < _serverPackets[i].PacketVariables.Length; x++) {
                string varName = Lower(_serverPackets[i].PacketVariables[x].VariableName); // Lower case variable name (C# formatting)
                string varType = packetTypes[_serverPackets[i].PacketVariables[x].VariableType]; // Variable type string

                psts += $"\n    public {varType} {Upper(varName)} {{ get => {varName}; set => {varName} = value; }}";
            }
            psts += "\n}\n";
        }

        #endregion

        #region Packet Handlers

        string phs = ""; // Packet Handlers String

        phs += "\n    public delegate void PacketHandler(Packet _packet);";
        phs = phs.Substring(1, phs.Length - 1);
        phs += "\n    public static List<PacketHandler> packetHandlers = new List<PacketHandler>() {";
        string packetHandlerFunctionsString = "";
        for (int i = 0; i < _serverPackets.Count; i++) {
            phs += $"\n        {{ {Upper(_serverPackets[i].PacketName)} }},";
        }
        phs += packetHandlerFunctionsString;
        phs += "\n    };";

        phs += "\n";

        for (int i = 0; i < _serverPackets.Count; i++) {
            string upPacketName = Upper(_serverPackets[i].PacketName);
            string loPacketName = Lower(_serverPackets[i].PacketName);
            phs += $"\n    public static void {upPacketName}(Packet _packet) {{";

            for (int x = 0; x < _serverPackets[i].PacketVariables.Length; x++) {
                phs += $"\n        {packetTypes[_serverPackets[i].PacketVariables[x].VariableType]} {Lower(_serverPackets[i].PacketVariables[x].VariableName)} = _packet.Read{packetReadTypes[_serverPackets[i].PacketVariables[x].VariableType]}();";
            }

            phs += "\n";

            string packetParameters = "";
            for (int x = 0; x < _serverPackets[i].PacketVariables.Length; x++) {
                packetParameters += $"{Lower(_serverPackets[i].PacketVariables[x].VariableName)}, ";
            }
            packetParameters = packetParameters.Substring(0, packetParameters.Length - 2);

            phs += $"\n        {upPacketName}Packet {loPacketName}Packet = new {upPacketName}Packet({packetParameters});";
            phs += $"\n        PacketManager.instance.PacketReceived(_packet, {loPacketName}Packet);";

            phs += "\n    }";
            phs += "\n";
        }
        phs = phs.Substring(0, phs.Length - 1); // Remove last /n

        #endregion

        #region Packet Send

        string pss = ""; // Packet Send String

        #region TcpUdpSendFunctionsString
        string TcpUdpSendFunctionsString = "    private static void SendTCPData(Packet _packet) {" +
                "\n        _packet.WriteLength();" +
                "\n        if (Client.instance.IsConnected) {" +
                "\n            Client.instance.Tcp.SendData(_packet);" +
                "\n            NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length());" +
                "\n        }" +
                "\n    }" +
                "\n" +
                "\n    private static void SendUDPData(Packet _packet) {" +
                "\n        _packet.WriteLength();" +
                "\n        if (Client.instance.IsConnected) {" +
                "\n            Client.instance.Udp.SendData(_packet);" +
                "\n            NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length());" +
                "\n        }" +
                "\n    }" +
                "\n";
        #endregion
        pss += TcpUdpSendFunctionsString;

        for (int i = 0; i < _clientPackets.Count; i++) {
            string pas = ""; // Packet arguments (parameters) string
            for (int x = 0; x < _clientPackets[i].PacketVariables.Length; x++) {
                pas += $"{packetTypes[_clientPackets[i].PacketVariables[x].VariableType]} _{Lower(_clientPackets[i].PacketVariables[x].VariableName)}, ";
            }
            pas = pas.Substring(0, pas.Length - 2);

            pss += $"\n    public static void {Upper(_clientPackets[i].PacketName)}({pas}) {{";
            pss += $"\n        using (Packet _packet = new Packet((int)ClientPackets.{Upper(_clientPackets[i].PacketName)})) {{";

            string pws = ""; // Packet writes
            for (int x = 0; x < _clientPackets[i].PacketVariables.Length; x++) {
                pws += $"\n            _packet.Write(_{Lower(_clientPackets[i].PacketVariables[x].VariableName)});";
            }
            pss += pws;
            pss += "\n";

            if (_clientPackets[i].Protocol == Protocol.TCP) {
                pss += "\n            SendTCPData(_packet);";
            } else {
                pss += "\n            SendUDPData(_packet);";
            }

            pss += "\n        }\n    }\n"; // Close functions and using statements
        }
        pss = pss.Substring(0, pss.Length - 1); // Remove last /n

        #endregion

        return
            "\n#region Packet Enums" +
            "\n" +
            "\n// Sent from Server to Client" +
            "\npublic enum ServerPackets {" +
            $"\n{serverPacketsString}" +
            "\n}" +
            "\n" +
            "\n// Sent from Client to Server" +
            "\npublic enum ClientPackets {" +
            $"\n{clientPacketsString}" +
            "\n}" +
            "\n" +
            "\n#endregion" +
            "\n" +
            "\n#region Packet Structs" +
            $"\n{psts}" +
            "\n#endregion" +
            "\n" +
            "\n#region Packet Handlers" +
            "\n" +
            "\npublic static class PacketHandlers {" +
            $"\n{phs}" +
            "\n}" +
            "\n" +
            "\n#endregion" +
            "\n" +
            "\n#region Packet Send" +
            "\n" +
            "\npublic static class PacketSend {" +
            $"\n{pss}" +
            "\n}" +
            "\n" +
            $"\n#endregion";
    }

    private string GenerateUSNLCallbackEventsText() {
        ServerPacketConfig[] _serverPackets = new ServerPacketConfig[libServerPackets.Length + serverPackets.Length];

        libServerPackets.CopyTo(_serverPackets, 0);
        serverPackets.CopyTo(_serverPackets, libServerPackets.Length);

        // Function declaration
        string output = "#region Callbacks\n" +
            "\npublic static class USNLCallbackEvents {" +
            "\n    public delegate void USNLCallbackEvent(object _param);" +
            "\n";

        // Packet Callback Events for Packet Handling
        output += "\n    public static USNLCallbackEvent[] PacketCallbackEvents = {";
        for (int i = 0; i < _serverPackets.Length; i++) {
            output += $"\n        CallOn{Upper(_serverPackets[i].PacketName)}PacketCallbacks,";
        }
        output += "\n    };";
        output += "\n";

        // Standard Callback events
        for (int i = 0; i < libCallbacks.Length; i++) {
            output += $"\n    public static event USNLCallbackEvent {libCallbacks[i]};";
        }
        output += "\n";

        // Packet Callback events
        for (int i = 0; i < _serverPackets.Length; i++) {
            output += $"\n    public static event USNLCallbackEvent On{Upper(_serverPackets[i].PacketName)}Packet;";
        }

        output += "\n";

        // Standard Callback Functions
        for (int i = 0; i < libCallbacks.Length; i++) {
            output += $"\n    public static void Call{libCallbacks[i]}Callbacks(object _param) {{ if ({libCallbacks[i]} != null) {{ {libCallbacks[i]}(_param); }} }}";
        }
        output += "\n";

        // Packet Callback Functions
        for (int i = 0; i < _serverPackets.Length; i++) {
            output += $"\n    public static void CallOn{Upper(_serverPackets[i].PacketName)}PacketCallbacks(object _param) {{ if (On{Upper(_serverPackets[i].PacketName)}Packet != null) {{ On{Upper(_serverPackets[i].PacketName)}Packet(_param); }} }}";
        }

        output += "\n}";
        output += "\n";
        output += "\n#endregion";

        return output;
    }

    #endregion
    
    #region Utils

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
