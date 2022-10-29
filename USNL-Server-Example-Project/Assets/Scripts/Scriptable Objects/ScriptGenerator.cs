using System;
using System.IO;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptGenerator", menuName = "USNL/Script Generator", order = 0)]
public class ScriptGenerator : ScriptableObject {
    [SerializeField] private PacketConfigurator packetConfigurator;

    private string generationPath = "Assets/";

    // Custom Callbacks for library functions (Specified by me)
    private string[] libCallbacks = {
        "OnServerStarted",
        "OnServerStopped",
        "OnClientConnected",
        "OnClientDisconnected"
    };

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
            $"\n{packetConfigurator.GetScriptText()}" +
            "\n" +
            "\n#endregion Packets" +
            "\n";
        #endregion

        #region USNL Callback Events
        scriptText +=
            $"\n{GenerateUSNLCallbackEventsText()}" +
            "\n";
        #endregion

        StreamWriter sw = new StreamWriter($"{generationPath}Generated.cs");
        sw.Write(scriptText);
        sw.Flush();
        sw.Close();
    }

    private string GenerateUSNLCallbackEventsText() {
        // Function declaration
        string output = "#region Callbacks\n" +
            "\npublic static class USNLCallbackEvents {" +
            "\n    public delegate void USNLCallbackEvent(object _param);" +
            "\n";

        // Packet Callback Events for Packet Handling
        output += "\n    public static USNLCallbackEvent[] PacketCallbackEvents = {";
        for (int i = 0; i < Enum.GetNames(typeof(ClientPackets)).Length; i++) {
            output += $"\n        CallOn{Enum.GetNames(typeof(ClientPackets))[i]}PacketCallbacks,";
        }
        output += "\n    };";
        output += "\n";

        // Standard Callback events
        for (int i = 0; i < libCallbacks.Length; i++) {
            output += $"\n    public static event USNLCallbackEvent {libCallbacks[i]};";
        }
        output += "\n";

        // Packet Callback events
        for (int i = 0; i < Enum.GetNames(typeof(ClientPackets)).Length; i++) {
            output += $"\n    public static event USNLCallbackEvent On{Enum.GetNames(typeof(ClientPackets))[i]}Packet;";
        }

        output += "\n";

        // Standard Callback Functions
        for (int i = 0; i < libCallbacks.Length; i++) {
            output += $"\n    public static void Call{libCallbacks[i]}Callbacks(object _param) {{ if ({libCallbacks[i]} != null) {{ {libCallbacks[i]}(_param); }} }}";
        }
        output += "\n";

        // Packet Callback Functions
        for (int i = 0; i < Enum.GetNames(typeof(ClientPackets)).Length; i++) {
            output += $"\n    public static void CallOn{Enum.GetNames(typeof(ClientPackets))[i]}PacketCallbacks(object _param) {{ if (On{Enum.GetNames(typeof(ClientPackets))[i]}Packet != null) {{ On{Enum.GetNames(typeof(ClientPackets))[i]}Packet(_param); }} }}";
        }

        output += "\n}";
        output += "\n";
        output += "\n#endregion";

        return output;
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
