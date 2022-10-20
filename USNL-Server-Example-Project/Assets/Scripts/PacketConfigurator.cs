using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

// What to specify in a new packet
// Name
// Variables
// Client or Server

// What Script will create
// Add script name enum
// Packet struct
// Packet Handler
// Packet Sender

[CreateAssetMenu(fileName ="PacketConfigurator", menuName = "USNL/Packet Configurator", order =0)]
public class PacketConfigurator : ScriptableObject {
    #region Variables

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
    private enum TransmitionProtocol {
        TCP,
        UDP
    }

    [Serializable]
    private struct PacketVariable {
        [SerializeField] private string packetName;
        [SerializeField] private PacketVarTypes packetType;

        public string PacketName { get => packetName; set => packetName = value; }
        public PacketVarTypes PacketType { get => packetType; set => packetType = value; }
    }

    [Serializable]
    private struct ClientPacketConfig {
        [Tooltip("Can be done in any formatting (eg. 'exampleName' & 'Example Name' both work)")]
        [SerializeField] private string packetName; // In C# formatting: eg. "examplePacket"
        [SerializeField] private PacketVariable[] packetVariables;
        [SerializeField] private TransmitionProtocol protocol;
        [Space]
        [Tooltip("This is only for the user.")]
        [SerializeField] private string notes;

        public string PacketName { get => packetName; set => packetName = value; }
        public PacketVariable[] PacketVariables { get => packetVariables; set => packetVariables = value; }
        public TransmitionProtocol Protocol { get => protocol; set => protocol = value; }
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
        [SerializeField] private TransmitionProtocol protocol;
        [Space]
        [Tooltip("This is only for the user.")]
        [SerializeField] private string notes;

        public string PacketName { get => packetName; set => packetName = value; }
        public PacketVariable[] PacketVariables { get => packetVariables; set => packetVariables = value; }
        public ServerPacketTypes SendType { get => sendType; set => sendType = value; }
        public TransmitionProtocol Protocol { get => protocol; set => protocol = value; }
    }

    #endregion
    // UDP OR TCP SERVER PACKETS

    public void GenerateServerPacketManagement() {
        string filePathAndName = generationPath + "GeneratedServerPacketManagement" + ".cs"; // The folder where the script is expected to exist

        #region Server & Client Packet Enums

        string serverPacketsString = "";
        for (int i = 0; i < serverPackets.Length; i++) {
            serverPacketsString += $"\n    {Upper(serverPackets[i].PacketName.ToString())},";
        }
        serverPacketsString = serverPacketsString.Substring(1, serverPacketsString.Length - 1); // Remove last ", " & first \n

        string clientPacketsString = "";
        for (int i = 0; i < clientPackets.Length; i++) {
            clientPacketsString += $"\n    {Upper(clientPackets[i].PacketName.ToString())},";
        }
        clientPacketsString = clientPacketsString.Substring(1, clientPacketsString.Length - 1); // Remove last ", " & first \n

        #endregion

        #region Packet Structs

        string psts = ""; // Packet Structs String
        for (int i = 0; i < clientPackets.Length; i++) {
            psts += $"\npublic struct {Upper(clientPackets[i].PacketName.ToString())}Packet {{";

            psts += $"\n    private int fromClient;";
            psts += "\n";
            for (int x = 0; x < clientPackets[i].PacketVariables.Length; x++) {
                string varName = Lower(clientPackets[i].PacketVariables[x].PacketName); // Lower case variable name (C# formatting)
                string varType = packetTypes[clientPackets[i].PacketVariables[x].PacketType]; // Variable type string
                psts += $"\n    private {varType} {varName};";
            }


            // Constructor:
            psts += "\n";
            string constructorParameters = "int _fromClient, ";
            for (int x = 0; x < clientPackets[i].PacketVariables.Length; x++) {
                constructorParameters += $"{packetTypes[clientPackets[i].PacketVariables[x].PacketType]} _{Lower(clientPackets[i].PacketVariables[x].PacketName)}, ";
            }
            constructorParameters = constructorParameters.Substring(0, constructorParameters.Length - 2);

            psts += $"\n    public {Upper(clientPackets[i].PacketName)}Packet({constructorParameters}) {{";
            psts += "\n        fromClient = _fromClient;";
            for (int x = 0; x < clientPackets[i].PacketVariables.Length; x++) {
                psts += $"\n        {Lower(clientPackets[i].PacketVariables[x].PacketName)} = _{Lower(clientPackets[i].PacketVariables[x].PacketName)};";
            }
            psts += "\n    }";
            psts += "\n";


            psts += "\n    public int FromClient { get => fromClient; set => fromClient = value; }";
            for (int x = 0; x < clientPackets[i].PacketVariables.Length; x++) {
                string varName = Lower(clientPackets[i].PacketVariables[x].PacketName); // Lower case variable name (C# formatting)
                string varType = packetTypes[clientPackets[i].PacketVariables[x].PacketType]; // Variable type string

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
        for (int i = 0; i < clientPackets.Length; i++) {
            phs += $"\n        {{ {Upper(clientPackets[i].PacketName)} }},";
        }
        phs += packetHandlerFunctionsString;
        phs += "\n    };";

        phs += "\n";

        for (int i = 0; i < clientPackets.Length; i++) {
            string upPacketName = Upper(clientPackets[i].PacketName);
            string loPacketName = Lower(clientPackets[i].PacketName);
            phs += $"\n    public static void {upPacketName}(Packet _packet) {{";

            string packetParameters = "";
            packetParameters += "_packet.PacketId, ";
            for (int x = 0; x < clientPackets[i].PacketVariables.Length; x++) {
                packetParameters += $"_packet.Read{packetReadTypes[clientPackets[i].PacketVariables[x].PacketType]}(), ";
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
        string TcpUdpSendFunctionsString = "    #region TCP & UDP Send Functions" +
            "\n" +
            "\n    private static void SendTCPData(int _toClient, Packet _packet) {" +
            "\n        _packet.WriteLength();" +
            "\n        Server.clients[_toClient].Tcp.SendData(_packet);" +
            "\n    }" +
            "\n" +
            "\n    private static void SendTCPDataToAll(Packet _packet) {" +
            "\n        _packet.WriteLength();" +
            "\n        for (int i = 1; i < Server.MaxClients; i++) {" +
            "\n            Server.clients[i].Tcp.SendData(_packet);" +
            "\n        }" +
            "\n    }" +
            "\n" +
            "\n    private static void SendTCPDataToAll(int _excpetClient, Packet _packet) {" +
            "\n        _packet.WriteLength();" +
            "\n        for (int i = 1; i < Server.MaxClients; i++) {" +
            "\n            if (i != _excpetClient) {" +
            "\n                Server.clients[i].Tcp.SendData(_packet);" +
            "\n            }" +
            "\n        }" +
            "\n    }" +
            "\n" +
            "\n    private static void SendUDPData(int _toClient, Packet _packet) {" +
            "\n        _packet.WriteLength();" +
            "\n        Server.clients[_toClient].Udp.SendData(_packet);" +
            "\n    }" +
            "\n" +
            "\n    private static void SendUDPDataToAll(Packet _packet) {" +
            "\n        _packet.WriteLength();" +
            "\n        for (int i = 1; i < Server.MaxClients; i++) {" +
            "\n            Server.clients[i].Udp.SendData(_packet);" +
            "\n        }" +
            "\n    }" +
            "\n" +
            "\n    private static void SendUDPDataToAll(int _excpetClient, Packet _packet) {" +
            "\n        _packet.WriteLength();" +
            "\n        for (int i = 1; i < Server.MaxClients; i++) {" +
            "\n            if (i != _excpetClient) {" +
            "\n                Server.clients[i].Udp.SendData(_packet);" +
            "\n            }" +
            "\n        }" +
            "\n    }" +
            "\n" +
            "\n    #endregion" + 
            "\n";
        #endregion
        pss += TcpUdpSendFunctionsString;

        for (int i = 0; i < serverPackets.Length; i++) {
            string pas = ""; // Packet arguments (parameters) string
            if (serverPackets[i].SendType == ServerPacketTypes.SendToClient) { pas = "int _toClient, "; }
            if (serverPackets[i].SendType == ServerPacketTypes.SendToAllClientsExcept) { pas = "int _exceptClient, "; }
            for (int x = 0; x < serverPackets[i].PacketVariables.Length; x++) {
                pas += $"{packetTypes[serverPackets[i].PacketVariables[x].PacketType]} _{Lower(serverPackets[i].PacketVariables[x].PacketName)}, ";
            }
            pas = pas.Substring(0, pas.Length - 2);

            pss += $"\n    public static void {Upper(serverPackets[i].PacketName)}({pas}) {{";
            pss += $"\n        using (Packet _packet = new Packet((int)ServerPackets.{Upper(serverPackets[i].PacketName)})) {{";

            string pws = ""; // Packet writes
            if (serverPackets[i].SendType == ServerPacketTypes.SendToClient) { pws = "\n            _packet.Write(_toClient);"; }
            if (serverPackets[i].SendType == ServerPacketTypes.SendToAllClientsExcept) { pas = "\n        _packet.Write(_exceptClient);"; }
            for (int x = 0; x < serverPackets[i].PacketVariables.Length; x++) {
                pws += $"\n            _packet.Write(_{Lower(serverPackets[i].PacketVariables[x].PacketName)});";
            }
            pss += pws;
            pss += "\n";

            string protocolString = "TCP";
            if (serverPackets[i].Protocol == TransmitionProtocol.UDP) { protocolString = "UDP"; }
            if (serverPackets[i].SendType == ServerPacketTypes.SendToClient) {
                pss += $"\n            Send{protocolString}Data(_toClient, _packet);";
            } else if (serverPackets[i].SendType == ServerPacketTypes.SendToClient) {
                pss += $"\n            Send{protocolString}DataToAll(_exceptClient, _packet);";
            } else {
                pss += $"\n            Send{protocolString}DataToAll(_packet);";
            }
            pss += "\n        }\n    }\n"; // Close functions and using
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
