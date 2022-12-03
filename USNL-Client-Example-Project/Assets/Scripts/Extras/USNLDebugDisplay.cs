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
    [Space]
    [SerializeField] private TextMeshProUGUI timeConnected;

    [Header("Connection Info")]
    [SerializeField] private TMP_InputField ip;
    [SerializeField] private TMP_InputField port;

    private void Update() {
        // Toggle debug menu visible
        if (Input.GetKey(KeyCode.LeftControl) & Input.GetKeyDown(KeyCode.BackQuote)) {
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
            totalBytesSent.text = RoundBytesToString(NetworkDebugInfo.instance.TotalBytesSent);
            totalBytesReceived.text = RoundBytesToString(NetworkDebugInfo.instance.TotalBytesReceived);
            bytesSentPerSecond.text = RoundBytesToString(NetworkDebugInfo.instance.BytesSentPerSecond);
            bytesReceivedPerSecond.text = RoundBytesToString(NetworkDebugInfo.instance.BytesReceivedPerSecond);

            totalPacketsSent.text = String.Format("{0:n0}", NetworkDebugInfo.instance.TotalPacketsSent);
            totalPacketsReceived.text = String.Format("{0:n0}", NetworkDebugInfo.instance.TotalPacketsReceived);

            totalPacketsSentPerSecond.text = String.Format("{0:n0}", NetworkDebugInfo.instance.TotalPacketsSentPerSecond);
            totalPacketsReceivedPerSecond.text = String.Format("{0:n0}", NetworkDebugInfo.instance.TotalPacketsReceivedPerSecond);

            packetRTT.text = String.Format("{0:n0}", NetworkDebugInfo.instance.PacketRTT) + "ms";
            smoothPacketRTT.text = String.Format("{0:n0}", NetworkDebugInfo.instance.SmoothPacketRTT) + "ms";

            if (ClientManager.instance.IsConnected)
                timeConnected.text = NetworkDebugInfo.instance.TimeConnected.ToString(@"hh\:mm\:ss");
            else
                timeConnected.text = "00:00:00";
            #endregion

            // Read text from json file ???? what
        }
    }

    private string RoundBytesToString(int _bytes) {
        string output = "";

        if (_bytes > 1000000000) {
            output = String.Format("{0:n}", _bytes / 1000000000f) + "GB";
        } else if (_bytes > 1000000) {
            output = String.Format("{0:n}", _bytes / 1000000f) + "MB";
        } else if (_bytes > 1000) {
            output = String.Format("{0:n}", _bytes / 1000f) + "KB";
        } else {
            output = String.Format("{0:n0}", _bytes) + "B";
        }

        return output;
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

    public void OnCloseButton() {
        debugMenuParent.SetActive(false);
    }

    public void OnOpenButton() {
        debugMenuParent.SetActive(true);
    }
}
