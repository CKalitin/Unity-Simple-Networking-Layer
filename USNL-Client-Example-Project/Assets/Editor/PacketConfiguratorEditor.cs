using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PacketConfigurator))]
[CanEditMultipleObjects]
public class PacketConfiguratorEditor : Editor {
    PacketConfigurator packetConfigurator;

    float verticalBreakSize;

    GUIStyle tooltipStyle;
    GUIStyle helpBoxStyle;

    private void OnEnable() {
        packetConfigurator = (PacketConfigurator)target;
        verticalBreakSize = 5;
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        tooltipStyle = new GUIStyle(GUI.skin.label) { fontSize = 13 };
        helpBoxStyle = new GUIStyle(GUI.skin.GetStyle("HelpBox")) { fontSize = 13 };

        VerticalBreak();
        // This button is used for updating resourceIds at runtime
        if (GUILayout.Button("Generate Server Packet Management")) {
            packetConfigurator.GenerateClientPacketManagement();

            // This is used to refresh the assets which adds the newly generated enums
            AssetDatabase.Refresh();
        }

        // A tooltip to inform the user about the use of the button above
        GUILayout.TextArea("Press this button every time you update the Packets.", tooltipStyle);

        VerticalBreak();

        HelpBox();

        VerticalBreak();

        // This is so the Scriptable Objects save when Unity closes
        EditorUtility.SetDirty(target);
    }

    private void VerticalBreak() {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("", GUILayout.Height(verticalBreakSize)); // This is a break
        EditorGUILayout.EndHorizontal();
    }

    private void HelpBox() {
        string helpMessage =
            "Server Packets are sent from the Server to the Client. Server -> Client\n" +
            "Client Packets are sent from the Client to the Server. Client -> Server\n" +
            "\n" +
            "Packet & Variable names can be written any way you like. The script removes spaces and formats it to c# formatting: eg. exampleVariable." +
            "" +
            "";

        GUILayout.TextArea(helpMessage, helpBoxStyle);
    }
}
