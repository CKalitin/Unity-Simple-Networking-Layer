using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PacketConfigurator : ScriptableObject {
    int numPackets;
    string packetName;
    string packetVariable = "private int clientIdCheck;";

    private string generationPath = "Scripts/";

    private void GenerateServerPacketManagement() {
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
    }
}

// What to specify in a new packet
// Name
// Variables
// Client or Server

// What Script will create
// Add script name enum
// Packet struct
// Packet Handler
// Packet Sender