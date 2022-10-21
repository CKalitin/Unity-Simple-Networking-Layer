using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class CallbackManager {
    private List<Callback> callbacks = new List<Callback>();

    private static List<Type> userTypes = new List<Type>();

    public CallbackManager(string[] callbackNames) {
        GenerateCallbacks(callbackNames);
    }

    public CallbackManager(string callbackNames) {
        GenerateCallbacks(new string[] { callbackNames });
    }

    private struct Callback {
        private MethodInfo methodInfo;
        private Type classType;

        public Callback(MethodInfo methodInfo, Type classType) {
            this.methodInfo = methodInfo;
            this.classType = classType;
        }

        public MethodInfo MethodInfo { get => methodInfo; set => methodInfo = value; }
        public Type ClassType { get => classType; set => classType = value; }
    }

    public void CallCallbacks(object[] _parameters) {
        // Loop through all callback methods
        for (int i = 0; i < callbacks.Count; i++) {
            // Get and Loop through all classes of type of the base call of method[i]
            List<MonoBehaviour> types = GetObjectsOfType(callbacks[i].ClassType);
            for (int x = 0; x < types.Count; x++) {
                try {
                    callbacks[i].MethodInfo.Invoke(types[x], _parameters);
                } catch (Exception _ex) {
                    Debug.LogError($"Could not run packet callback function: {callbacks[i].MethodInfo} in class {types[x].GetType()}\n{_ex}");
                }
            }
        }
    }

    public void CallCallbacks() {
        object[] _parameters = { };

        // Loop through all callback methods
        for (int i = 0; i < callbacks.Count; i++) {
            // Get and Loop through all classes of type of the base call of method[i]
            List<MonoBehaviour> types = GetObjectsOfType(callbacks[i].ClassType);
            for (int x = 0; x < types.Count; x++) {
                try {
                    callbacks[i].MethodInfo.Invoke(types[x], _parameters);
                } catch (Exception _ex) {
                    Debug.LogError($"Could not run packet callback function: ({callbacks[i].MethodInfo}) in class ({types[x].GetType()})\n{_ex}");
                }
            }
        }
    }

    private void GenerateCallbacks(string[] _callbackNames) {
        // Track how long this takes in a project with many scripts
        // https://stackoverflow.com/questions/540066/calling-a-function-from-a-string-in-c-sharp

        if (userTypes.Count <= 0) { userTypes = GetAllUserScriptTypes(); }

        // Iterate through every user script type
        for (int i = 0; i < userTypes.Count; i++) {
            // Iterate through every server packet type
            for (int x = 0; x < _callbackNames.Length; x++) {
                // Find method of name "On{packetName}Packet" / "OnWelcomeReceivedPacket()"
                MethodInfo method = userTypes[i].GetMethod($"{_callbackNames[x]}", BindingFlags.NonPublic | BindingFlags.Instance);
                if (method == null) { method = userTypes[i].GetMethod($"{_callbackNames[x]}"); }

                if (method != null) {
                    callbacks.Add(new Callback(method, userTypes[i]));
                }
            }
        }
    }

    private List<MonoBehaviour> GetObjectsOfType(Type _t) {
        // Pretty slow alternative to using FindObjectsOfType<>(), but that doesn't work with a variable type

        MonoBehaviour[] monoBehaviours = GameObject.FindObjectsOfType<MonoBehaviour>();
        List<MonoBehaviour> output = new List<MonoBehaviour>();

        for (int i = 0; i < monoBehaviours.Length; i++) {
            if (monoBehaviours[i].GetType() == _t) {
                output.Add(monoBehaviours[i]);
            }
        }

        return output;
    }

    // Gets all user types (class / monobehaviour names)
    private static List<Type> GetAllUserScriptTypes() {
        // https://forum.unity.com/threads/geting-a-array-or-list-of-all-unity-types.416976/
        List<Type> results = new List<Type>();

        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
            if (assembly.FullName.StartsWith("Assembly-CSharp")) {
                foreach (Type type in assembly.GetTypes()) { results.Add(type); }
                break;
            }
        }

        return results;
    }

}
