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

public class PacketConfigurator : ScriptableObject {
    #region Variables

    [SerializeField] private string generationPath = "Scripts/";
    [Space]
    [SerializeField] private PacketConfig[] serverPackets;
    [SerializeField] private PacketConfig[] clientPackets;

    enum PacketVariables {
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
    Dictionary<PacketVariables, string> packetTypes = new Dictionary<PacketVariables, string>() 
    { { PacketVariables.Byte, "byte"},
    { PacketVariables.ByteArray, "byte[]"},
    { PacketVariables.Short, "short"},
    { PacketVariables.Int, "int"},
    { PacketVariables.Long, "long"},
    { PacketVariables.Float, "float"},
    { PacketVariables.Bool, "bool"},
    { PacketVariables.String, "string"},
    { PacketVariables.Vector2, "vector2"},
    { PacketVariables.Vector3, "vector3"},
    { PacketVariables.Quaternion, "quaternion"}
    };
    Dictionary<PacketVariables, string> packetReadTypes = new Dictionary<PacketVariables, string>()
    { { PacketVariables.Byte, "Byte"},
    { PacketVariables.ByteArray, "Bytes"},
    { PacketVariables.Short, "Short"},
    { PacketVariables.Int, "Int"},
    { PacketVariables.Long, "Long"},
    { PacketVariables.Float, "Float"},
    { PacketVariables.Bool, "Bool"},
    { PacketVariables.String, "String"},
    { PacketVariables.Vector2, "Vector2"},
    { PacketVariables.Vector3, "Vector3"},
    { PacketVariables.Quaternion, "Quaternion"}
    };

    private struct PacketConfig {
        private string packetName; // In C# formatting: eg. "examplePacket"
        private PacketVariables[] packetVariables;

        public string PacketName { get => packetName; set => packetName = value; }
        public PacketVariables[] PacketVariables { get => packetVariables; set => packetVariables = value; }
    }
    
    #endregion

    private void generateServerPacketManagement() {
        string filePathAndName = generationPath + "GeneratedServerPacketManagement" + ".cs"; // The folder where the script is expected to exist

        // Packet Handler
        // Packet name (upper) 
        // variable names & types

        // Packet Send
        // Packet Name (upper), parameter (variables - names & types)

        #region Server & Client Packet Enums

        string serverPacketsString = "";
        for (int i = 0; i < serverPackets.Length; i++) {
            serverPacketsString += $"\n{Lower(serverPackets[i].ToString())} = {i + 1}, ";
        }
        serverPacketsString = serverPacketsString.Substring(0, serverPacketsString.Length - 2); // Remove last ", "

        string clientPacketsString = "";
        for (int i = 0; i < clientPackets.Length; i++) {
            clientPacketsString += $"\n{Lower(clientPackets[i].ToString())} = {i + 1}, ";
        }
        clientPacketsString = clientPacketsString.Substring(0, clientPacketsString.Length - 2); // Remove last ", "

        #endregion

        #region Packet Structs

        string packetStructsString = "";
        for (int i = 0; i < clientPackets.Length; i++) {
            packetStructsString += $"\npublic struct {Upper(clientPackets[i].ToString())}Packet {{";

            packetStructsString += $"\nprivate int clientId;";
            packetStructsString += "\n";
            for (int x = 0; x < clientPackets[i].PacketVariables.Length; x++) {
                string varLower = Lower(packetTypes[clientPackets[i].PacketVariables[x]].ToString()); // Lower case variable name (C# formatting)
                string varType = packetTypes[clientPackets[i].PacketVariables[x]]; // Variable type string
                packetStructsString += $"\nprivate {varType} {varLower};";
            }


            // Constructor:
            packetStructsString += "\n";
            string constructorParameters = "";
            for (int x = 0; x < clientPackets[i].PacketVariables.Length; x++) {
                constructorParameters += $"{packetTypes[clientPackets[i].PacketVariables[x]]} _{Lower(packetTypes[clientPackets[i].PacketVariables[x]].ToString())}, ";
            }
            constructorParameters = constructorParameters.Substring(0, constructorParameters.Length - 2);

            packetStructsString += $"\npublic {Upper(clientPackets[i].ToString())}Packet({constructorParameters}) {{";
            for (int x = 0; x < clientPackets[i].PacketVariables.Length; x++) {
                packetStructsString += $"\n{Upper(packetTypes[clientPackets[i].PacketVariables[x]].ToString())} = _{Lower(packetTypes[clientPackets[i].PacketVariables[x]].ToString())}";
            }
            packetStructsString += "\n}";
            packetStructsString += "\n";


            packetStructsString += "\npublic int PacketId { get => packetId; set => packetId = value; }";
            for (int x = 0; x < clientPackets[i].PacketVariables.Length; x++) {
                string varLower = Lower(packetTypes[clientPackets[i].PacketVariables[x]].ToString()); // Lower case variable name (C# formatting)
                string varType = packetTypes[clientPackets[i].PacketVariables[x]]; // Variable type string

                packetStructsString += $"\npublic {varType} {Upper(varLower)} {{ get => {varLower}; set => {varLower} = value; }}";
            }
            packetStructsString += "\n}";
        }
        packetStructsString = packetStructsString.Substring(0, packetStructsString.Length - 3); // This may or may not remove the "\n" we'll see

        #endregion

        #region Packet Handlers

        string packetHandlersString = "";

        packetHandlersString += "\npublic delegate void PacketHandler(Packet _packet);";
        packetHandlersString += "\npublic static List<PacketHandler> packetHandlers = new List<PacketHandler>() {";
        string packetHandlerFunctionsString = "";
        for (int i = 0; i < clientPackets.Length; i++) {
            packetHandlersString += $"\n{{ {Upper(clientPackets[i].ToString())} }},";
        }
        packetHandlersString += packetHandlerFunctionsString;
        packetHandlersString += "\n};";

        packetHandlersString += "\n";

        for (int i = 0; i < clientPackets.Length; i++) {
            string upPacketName = Upper(clientPackets[i].ToString());
            string loPacketName = Lower(clientPackets[i].ToString());
            packetHandlersString += $"\npublic static void {upPacketName}(Packet _packet) {{";

            string packetParameters = "";
            packetParameters += "_packet.PacketId, ";
            for (int x = 0; x < clientPackets[i].PacketVariables.Length; x++) {
                packetParameters += $"_packet.Read{packetReadTypes[clientPackets[i].PacketVariables[x]]}(), ";
            }
            packetParameters = packetParameters.Substring(0, packetParameters.Length - 2);

            packetHandlersString += $"\n{upPacketName}Packet {loPacketName}Packet = new {upPacketName}({packetParameters});";
            packetHandlersString += $"\nPacketManager.instance.PacketReceived(_packet, {loPacketName});";

            packetHandlersString += "\n}";
            packetHandlersString += "\n";
        }

        #endregion

        #region PacketSend

        string pss = ""; // Packet Send String

        #region TcpUdpSendFunctionsString
        string TcpUdpSendFunctionsString = "\n    #region TCP & UDP Send Functions" +
            "\n" +
            "\n    private static void SendTCPData(int _toClient, Packet _packet) {" +
            "\n        _packet.WriteLength();" +
            "\n        Server.clients[_toClient].Tcp.SendData(_packet);" +
            "\n    }" +
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
            "\n    #endregion";
        #endregion
        pss += TcpUdpSendFunctionsString;

        #endregion

        using (StreamWriter streamWriter = new StreamWriter(filePathAndName)) {

            streamWriter.Write(
                "using System.Collections.Generic;" +
                "\nusing System.Collections.Generic;" +
                "\nusing UnityEngine;" +
                "\n" +
                "\n// Sent from Server to Client" +
                "\n public enum ServerPackets {" +
                $"\n{serverPacketsString}" +
                "\n}" +
                "\n" +
                "\n// Sent from Client to Server" +
                "\npublic enum ClientPackets {" +
                $"\n{clientPacketsString}" +
                "\n}" +
                "\n" +
                "\n/*** Packet Structs ***/" +
                "\n" +
                $"\n{packetStructsString}" +
                "\npublic static class PacketHandlers {" +
                $"\n{packetHandlersString}" +
                "\n}" +
                "\npublic static class PacketSend {" +
                $"\n{pss}" +
                "\n}" +
                "\n");
        }
    }

    private string Upper(string _input) {
        string output = String.Concat(_input.Where(c => !Char.IsWhiteSpace(c))); // Remove whitespace
        return $"{Char.ToUpper(output[0])}{output.Substring(1)}";
    }

    private string Lower(string _input) {
        string output = String.Concat(_input.Where(c => !Char.IsWhiteSpace(c))); // Remove whitespace
        return $"{Char.ToUpper(output[0])}{output.Substring(1)}";
    }

    /*private void GenerateServerPacketManagementOld() {
        string filePathAndName = generationPath + "GeneratedServerPacketManagement" + ".cs"; //The folder where the enum script is expected to exist

        using (StreamWriter streamWriter = new StreamWriter(filePathAndName)) {
            // Pcaket Structs
            // Packet Handler
            // Packet Send

            // Write Using statements
            streamWriter.WriteLine("using System.Collections;\r\nusing System.Collections.Generic;\r\nusing UnityEngine;");

            streamWriter.WriteLine(""); // Line break

            // Client Packet enums
            streamWriter.WriteLine("// Sent from Server to Client.");
            streamWriter.WriteLine("public enum ClientPackets {");
            for (int i = 0; i < numPackets; i++) {
                streamWriter.WriteLine($"{packetName} = {i + 1}");
            }
            streamWriter.WriteLine("}");

            streamWriter.WriteLine(""); // Line break

            // Server Packet enums
            streamWriter.WriteLine("// Sent from Client to Server.");
            streamWriter.WriteLine("public enum ServerPackets {");
            for (int i = 0; i < numPackets; i++) {
                streamWriter.WriteLine($"   {packetName} = {i + 1}");
            }
            streamWriter.WriteLine("}");

            streamWriter.WriteLine(""); // Line break

            // Packet Structs
            for (int i = 0; i < numPackets; i++) {
                streamWriter.WriteLine($"public struct {packetName} {{"); // Might be a bug here
                streamWriter.WriteLine($"{packetVariable}");
                // Get Set
                streamWriter.WriteLine(""); // Line break
            }

            streamWriter.WriteLine("public static class PacketHandler {");
            for (int i = 0; i < numPackets; i++) {
                streamWriter.WriteLine($"    public static void {packetName}(int _fromClient, Packet _packet) {{"); // Might be a bug here
                // TODO
            }
        }
    }*/
}
