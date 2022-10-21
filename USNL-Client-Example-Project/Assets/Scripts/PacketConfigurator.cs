using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "PacketConfigurator", menuName = "USNL/Packet Configurator", order = 0)]
public class PacketConfigurator : ScriptableObject {
    #region Variables

    /*** Library Packets (Not for user) ***/
    private ServerPacketConfig[] libServerPackets = {
        new ServerPacketConfig(
            "Welcome",
            new PacketVariable[] { new PacketVariable("Welcome Message", PacketVarTypes.String), new PacketVariable("Client Id", PacketVarTypes.Int) },
            ServerPacketTypes.SendToClient,
            Protocol.TCP)
    };
    private ClientPacketConfig[] libClientPackets = {
        new ClientPacketConfig(
            "Welcome Received",
            new PacketVariable[] { new PacketVariable("Client Id Check", PacketVarTypes.Int) },
            Protocol.TCP)
    };

    [SerializeField] private string generationPath = "Assets/";
    [Space]
    [SerializeField] private ServerPacketConfig[] serverPackets;
    [SerializeField] private ClientPacketConfig[] clientPackets;

    Dictionary<PacketVarTypes, string> packetTypes = new Dictionary<PacketVarTypes, string>()
    { { PacketVarTypes.Byte, "byte"},
    { PacketVarTypes.ByteArray, "byte[]"},
    { PacketVarTypes.Short, "short"},
    { PacketVarTypes.Int, "int"},
    { PacketVarTypes.Long, "long"},
    { PacketVarTypes.Float, "float"},
    { PacketVarTypes.Bool, "bool"},
    { PacketVarTypes.String, "string"},
    { PacketVarTypes.Vector2, "Vector2"},
    { PacketVarTypes.Vector3, "Vector3"},
    { PacketVarTypes.Quaternion, "Quaternion"}
    };
    Dictionary<PacketVarTypes, string> packetReadTypes = new Dictionary<PacketVarTypes, string>()
    { { PacketVarTypes.Byte, "Byte"},
    { PacketVarTypes.ByteArray, "Bytes"},
    { PacketVarTypes.Short, "Short"},
    { PacketVarTypes.Int, "Int"},
    { PacketVarTypes.Long, "Long"},
    { PacketVarTypes.Float, "Float"},
    { PacketVarTypes.Bool, "Bool"},
    { PacketVarTypes.String, "String"},
    { PacketVarTypes.Vector2, "Vector2"},
    { PacketVarTypes.Vector3, "Vector3"},
    { PacketVarTypes.Quaternion, "Quaternion"}
    };

    private enum PacketVarTypes {
        Byte,
        ByteArray,
        Short,
        Int,
        Long,
        Float,
        Bool,
        String,
        Vector2,
        Vector3,
        Quaternion
    }
    private enum ServerPacketTypes {
        SendToClient,
        SendToAllClients,
        SendToAllClientsExcept
    }
    private enum Protocol {
        TCP,
        UDP
    }

    [Serializable]
    private struct PacketVariable {
        [SerializeField] private string packetName;
        [SerializeField] private PacketVarTypes packetType;

        public PacketVariable(string packetName, PacketVarTypes packetType) {
            this.packetName = packetName;
            this.packetType = packetType;
        }

        public string PacketName { get => packetName; set => packetName = value; }
        public PacketVarTypes PacketType { get => packetType; set => packetType = value; }
    }

    [Serializable]
    private struct ServerPacketConfig {
        [Tooltip("Can be done in any formatting (eg. 'exampleName' & 'Example Name' both work)")]
        [SerializeField] private string packetName;
        [SerializeField] private PacketVariable[] packetVariables;
        [Tooltip("SendToClient: Send Packet to a single client specified by you.\n" +
            "SendToAllClients: Send Packet to all clients.\n" +
            "SendToAllClientsExcept: Send Packet to all clients execpt one specified by you.")]
        [SerializeField] private ServerPacketTypes sendType;
        [SerializeField] private Protocol protocol;
        [Space]
        [Tooltip("This is only for the user.")]
        [SerializeField] private string notes;

        public ServerPacketConfig(string packetName, PacketVariable[] packetVariables, ServerPacketTypes sendType, Protocol protocol) : this() {
            this.packetName = packetName;
            this.packetVariables = packetVariables;
            this.sendType = sendType;
            this.protocol = protocol;
        }

        public string PacketName { get => packetName; set => packetName = value; }
        public PacketVariable[] PacketVariables { get => packetVariables; set => packetVariables = value; }
        public ServerPacketTypes SendType { get => sendType; set => sendType = value; }
        public Protocol Protocol { get => protocol; set => protocol = value; }
    }

    [Serializable]
    private struct ClientPacketConfig {
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
    public void GenerateClientPacketManagement() {
        string filePathAndName = generationPath + "GeneratedClientPacketManagement" + ".cs"; // The folder where the script is expected to exist

        List<ServerPacketConfig> _serverPackets = new List<ServerPacketConfig>();
        List<ClientPacketConfig> _clientPackets = new List<ClientPacketConfig>();

        _serverPackets.AddRange(serverPackets);
        _serverPackets.AddRange(libServerPackets);
        _clientPackets.AddRange(clientPackets);
        _clientPackets.AddRange(libClientPackets);

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
                string varName = Lower(_serverPackets[i].PacketVariables[x].PacketName); // Lower case variable name (C# formatting)
                string varType = packetTypes[_serverPackets[i].PacketVariables[x].PacketType]; // Variable type string
                psts += $"\n    private {varType} {varName};";
            }


            // Constructor:
            psts += "\n";
            string constructorParameters = "";
            for (int x = 0; x < _serverPackets[i].PacketVariables.Length; x++) {
                constructorParameters += $"{packetTypes[_serverPackets[i].PacketVariables[x].PacketType]} _{Lower(_serverPackets[i].PacketVariables[x].PacketName)}, ";
            }
            constructorParameters = constructorParameters.Substring(0, constructorParameters.Length - 2);

            psts += $"\n    public {Upper(_serverPackets[i].PacketName)}Packet({constructorParameters}) {{";
            for (int x = 0; x < _serverPackets[i].PacketVariables.Length; x++) {
                psts += $"\n        {Lower(_serverPackets[i].PacketVariables[x].PacketName)} = _{Lower(_serverPackets[i].PacketVariables[x].PacketName)};";
            }
            psts += "\n    }";
            psts += "\n";


            for (int x = 0; x < _serverPackets[i].PacketVariables.Length; x++) {
                string varName = Lower(_serverPackets[i].PacketVariables[x].PacketName); // Lower case variable name (C# formatting)
                string varType = packetTypes[_serverPackets[i].PacketVariables[x].PacketType]; // Variable type string

                psts += $"\n    public {varType} {Upper(varName)} {{ get => {varName}; set => {varName} = value; }}";
            }
            psts += "\n}";
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

            phs += "\n_packet.ReadInt(); // This needs to be here for packet reading, idk why it just works.";
            string packetParameters = "";
            for (int x = 0; x < _serverPackets[i].PacketVariables.Length; x++) {
                packetParameters += $"_packet.Read{packetReadTypes[_serverPackets[i].PacketVariables[x].PacketType]}(), ";
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
                "\n        }" +
                "\n    }" +
                "\n" +
                "\n    private static void SendUDPData(Packet _packet) {" +
                "\n        _packet.WriteLength();" +
                "\n        if (Client.instance.IsConnected) {" +
                "\n            Client.instance.Udp.SendData(_packet);" +
                "\n        }" +
                "\n    }" +
                "\n";
        #endregion
        pss += TcpUdpSendFunctionsString;

        for (int i = 0; i < _clientPackets.Count; i++) {
            string pas = ""; // Packet arguments (parameters) string
            for (int x = 0; x < _clientPackets[i].PacketVariables.Length; x++) {
                pas += $"{packetTypes[_clientPackets[i].PacketVariables[x].PacketType]} _{Lower(_clientPackets[i].PacketVariables[x].PacketName)}, ";
            }
            pas = pas.Substring(0, pas.Length - 2);

            pss += $"\n    public static void {Upper(_clientPackets[i].PacketName)}({pas}) {{";
            pss += $"\n        using (Packet _packet = new Packet((int)ClientPackets.{Upper(_clientPackets[i].PacketName)})) {{";

            string pws = ""; // Packet writes
            for (int x = 0; x < _clientPackets[i].PacketVariables.Length; x++) {
                pws += $"\n            _packet.Write(_{Lower(_clientPackets[i].PacketVariables[x].PacketName)});";
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

        try {
            using (StreamWriter streamWriter = new StreamWriter(filePathAndName)) {

                streamWriter.Write(
                    "using System.Collections.Generic;" +
                    "using UnityEngine;" +
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
                    "\n/*** Packet Structs ***/" +
                    $"\n{psts}" +
                    "\n" +
                    "\npublic static class PacketHandlers {" +
                    $"\n{phs}" +
                    "\n}" +
                    "\n" +
                    "\npublic static class PacketSend {" +
                    $"\n{pss}" +
                    "\n}" +
                    "\n");
            }
        } catch (Exception _ex) {
            Debug.LogError($"Could not save Packet Configuration with error: {_ex}");
        }
    }

    private string Upper(string _input) {
        string output = String.Concat(_input.Where(c => !Char.IsWhiteSpace(c))); // Remove whitespace
        return $"{Char.ToUpper(output[0])}{output.Substring(1)}";
    }

    private string Lower(string _input) {
        string output = String.Concat(_input.Where(c => !Char.IsWhiteSpace(c))); // Remove whitespace
        return $"{Char.ToLower(output[0])}{output.Substring(1)}";
    }
}
