using System;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptGenerator", menuName = "USNL/Script Generator", order = 0)]
public class ScriptGenerator : ScriptableObject {
    [SerializeField] private PacketConfigurator packetConfigurator;

    private string generationPath = "Assets/";

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
        output += "\n    public static event USNLCallbackEvent OnServerStarted;";
        output += "\n    public static event USNLCallbackEvent OnServerStopped;";
        output += "\n    public static event USNLCallbackEvent OnClientConnected;";
        output += "\n    public static event USNLCallbackEvent OnClientDisconnected;";
        output += "\n";

        // Packet Callback events
        for (int i = 0; i < Enum.GetNames(typeof(ClientPackets)).Length; i++) {
            output += $"\n    public static event USNLCallbackEvent On{Enum.GetNames(typeof(ClientPackets))[i]}Packet;";
        }

        output += "\n";

        // Standard Callback Functions
        output += "\n    public static void CallOnServerStartedCallbacks(object _param) { if (OnServerStarted != null) { OnServerStarted(_param); } }";
        output += "\n    public static void CallOnServerStoppedCallbacks(object _param) { if (OnServerStopped != null) { OnServerStopped(_param); } }";
        output += "\n    public static void CallOnClientConnectedCallbacks(object _param) { if (OnClientConnected != null) { OnClientConnected(_param); } }";
        output += "\n    public static void CallOnClientDisconnectedCallbacks(object _param) { if (OnClientDisconnected != null) { OnClientDisconnected(_param); } }";
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
}
