using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class USNLDebugDisplay : MonoBehaviour {
    [Header("Debug Menu")]
    [SerializeField] GameObject debugMenuParent;

    [Header("Client Manager Bool Values")]
    [SerializeField] private GameObject isConnected;
    [SerializeField] private GameObject isAttempingConnection;
    [SerializeField] private GameObject isHost;
    [SerializeField] private GameObject isMigratingHost;
    [SerializeField] private GameObject isBecomingHost;

    [Header("Network Info")]
    [SerializeField] private TextMeshProUGUI totalBytesSent;
    [SerializeField] private TextMeshProUGUI totalBytesReceived;
    [Space]
    [SerializeField] private TextMeshProUGUI bytesSentPerSecond;
    [SerializeField] private TextMeshProUGUI bytesReceivedPerSecond;
    [Space]
    [SerializeField] private TextMeshProUGUI totalPacketsSent;
    [SerializeField] private TextMeshProUGUI totalPacketsReceived;
    [Space]
    [SerializeField] private TextMeshProUGUI totalPacketsSentPerSecond;
    [SerializeField] private TextMeshProUGUI totalPacketsReceivedPerSecond;
    [Space]
    [SerializeField] private TextMeshProUGUI packetRTT;
    [Tooltip("Average of last 5 pings")]
    [SerializeField] private TextMeshProUGUI smoothPacketRTT;

    [Header("Connection Info")]
    [SerializeField] private InputField ip;
    [SerializeField] private InputField port;

    private void Update() {
        // Toggle debug menu visible
        if (Input.GetKeyDown(KeyCode.LeftControl) & Input.GetKeyDown(KeyCode.Tilde)) {
            debugMenuParent.SetActive(!debugMenuParent.activeSelf);
        }

        // If Debug Menu visible, update values
        if (debugMenuParent.activeSelf) {
            #region Client Mangaer Bool Values
            isConnected.SetActive(ClientManager.instance.IsConnected);
            isAttempingConnection.SetActive(ClientManager.instance.IsAttempingConnection);
            isHost.SetActive(ClientManager.instance.IsHost);
            isMigratingHost.SetActive(ClientManager.instance.IsMigratingHost);
            isBecomingHost.SetActive(ClientManager.instance.IsBecomingHost);
            #endregion

            #region Network Info
            totalBytesSent.text = NetworkDebugInfo.instance.TotalBytesSent.ToString();
            totalBytesReceived.text = NetworkDebugInfo.instance.TotalBytesReceived.ToString();
            bytesSentPerSecond.text = NetworkDebugInfo.instance.BytesSentPerSecond.ToString();
            bytesReceivedPerSecond.text = NetworkDebugInfo.instance.BytesReceivedPerSecond.ToString();

            totalPacketsSent.text = NetworkDebugInfo.instance.TotalPacketsSent.ToString();
            totalPacketsReceived.text = NetworkDebugInfo.instance.TotalPacketsReceived.ToString();

            totalPacketsSentPerSecond.text = NetworkDebugInfo.instance.TotalPacketsSentPerSecond.ToString();
            totalPacketsReceivedPerSecond.text = NetworkDebugInfo.instance.TotalPacketsReceivedPerSecond.ToString();

            packetRTT.text = NetworkDebugInfo.instance.PacketRTT.ToString();
            smoothPacketRTT.text = NetworkDebugInfo.instance.SmoothPacketRTT.ToString();
            #endregion
        }
    }

    public void ConnectButtonDown() {
        try {
            ClientManager.instance.ConnectToServer(ip.text, Int32.Parse(port.text));
        } catch (Exception _ex) {
            Debug.LogError($"Could not connect to server via Debug Menu, likely improper port.\n{_ex}");
        }
    }

    public void DisconnectButtonDown() {
        ClientManager.instance.DisconnectFromServer();
    }
}
