using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ScriptGenerator))]
[CanEditMultipleObjects]
public class ScriptGeneratorEditor : Editor {
    ScriptGenerator scriptGenerator;

    float verticalBreakSize;

    GUIStyle tooltipStyle;
    GUIStyle helpBoxStyle;

    private void OnEnable() {
        scriptGenerator = (ScriptGenerator)target;
        verticalBreakSize = 5;
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        tooltipStyle = new GUIStyle(GUI.skin.label) { fontSize = 13 };
        helpBoxStyle = new GUIStyle(GUI.skin.GetStyle("HelpBox")) { fontSize = 13 };

        VerticalBreak();
        // This button is used for updating packets at runtime
        if (GUILayout.Button("Generate")) {
            scriptGenerator.GenerateScript();

            // This is used to refresh the assets which adds the newly generated enums
            AssetDatabase.Refresh();
        }
        
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
            "Server Packets: Server -> Client" +
            "\nClient Packets: Client -> Server" +
            "\n" +
            "\nType variable names should be in C# format: exampleVariable." +
            "\n" +
            "\nMaybe it's a good idea to back this up? Who knows... Just a suggestion.";

        GUILayout.TextArea(helpMessage, helpBoxStyle);
    }
}
