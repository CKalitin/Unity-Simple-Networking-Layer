using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class USNLDebugDisplay : MonoBehaviour {
    [Header("Debug Menu")]
    [SerializeField] GameObject debugMenuParent;

    [Header("Server Manager Bool Values")]
    [SerializeField] private GameObject isRunning;
    [SerializeField] private GameObject isMigratingHost;

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
    
    [Header("Connection")]
    [SerializeField] private TextMeshProUGUI uptime;
    [SerializeField] private TextMeshProUGUI clientsConnected;

    private void Update() {
        // Toggle debug menu visible
        if (Input.GetKey(KeyCode.LeftControl) & Input.GetKeyDown(KeyCode.BackQuote)) {
            debugMenuParent.SetActive(!debugMenuParent.activeSelf);
        }

        // If Debug Menu visible, update values
        if (debugMenuParent.activeSelf) {
            #region Client Mangaer Bool Values
            isRunning.SetActive(Server.ServerActive);
            isMigratingHost.SetActive(ServerManager.instance.IsMigratingHost);
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

            uptime.text = NetworkDebugInfo.instance.Uptime.ToString(@"hh\:mm\:ss");
            clientsConnected.text = $"{Server.GetConnectedClients()}/{ServerManager.instance.MaxPlayers}";
            #endregion

            // Read text from json file
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
    
    public void OnCloseButton() {
        debugMenuParent.SetActive(false);
    }

    public void OnOpenButton() {
        debugMenuParent.SetActive(true);
    }
}