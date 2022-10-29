using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NetworkDebugInfo : MonoBehaviour {
    #region Variables

    public static NetworkDebugInfo instance;

    [Header("Bytes")]
    [SerializeField] private int totalBytesSent;
    [SerializeField] private int totalBytesReceived;
    [Space]
    [SerializeField] private int bytesSentPerSecond;
    [SerializeField] private int bytesReceivedPerSecond;

    [Header("Packets")]
    [SerializeField] private int totalPacketsSent;
    [SerializeField] private int totalPacketsReceived;
    [Space]
    [SerializeField] private int totalPacketsSentPerSecond;
    [SerializeField] private int totalPacketsReceivedPerSecond;

    [Header("Ping")]
    [SerializeField] private int packetRTT;
    [Tooltip("Average of last 5 pings")]
    [SerializeField] private int smoothPacketRTT;

    private List<int> packetRTTs = new List<int>();
    private float packetPingSentTime = -1; // Seconds since startup when ping packet was sent

    // Too much memory? - adding a clear function, nvm it's just some ints
    // Index is packet Id
    private int[] totalBytesSentByPacket = new int[Enum.GetNames(typeof(ClientPackets)).Length];
    private int[] totalBytesReceivedByPacket = new int[Enum.GetNames(typeof(ServerPackets)).Length];

    // These are the values that can be accessed by the user
    private int[] bytesSentByPacketPerSecond = new int[Enum.GetNames(typeof(ClientPackets)).Length];
    private int[] bytesReceivedByPacketPerSecond = new int[Enum.GetNames(typeof(ServerPackets)).Length];
    private int[] packetsSentPerSecond = new int[Enum.GetNames(typeof(ClientPackets)).Length];
    private int[] packetsReceivedPerSecond = new int[Enum.GetNames(typeof(ServerPackets)).Length];

    // Index is packet Id - Temp because this is added to over a second, then reset
    private int[] tempBytesSentByPacketPerSecond = new int[Enum.GetNames(typeof(ClientPackets)).Length];
    private int[] tempBytesReceivedByPacketPerSecond = new int[Enum.GetNames(typeof(ServerPackets)).Length];
    private int[] tempPacketsSentPerSecond = new int[Enum.GetNames(typeof(ClientPackets)).Length];
    private int[] tempPacketsReceivedPerSecond = new int[Enum.GetNames(typeof(ServerPackets)).Length];

    #endregion

    #region Core

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Debug.Log("Network Debug Info instance already exists, destroying object.");
            Destroy(this);
        }
    }

    private void Start() {
        StartCoroutine(BytesAndPacketsPerSecondCoroutine());
    }

    private void OnEnable() {
        USNLCallbackEvents.OnPingPacket += OnPingPacketReceived;
        USNLCallbackEvents.OnDisconnected += OnDisconnected;
    }

    private void OnDisable() {
        USNLCallbackEvents.OnPingPacket -= OnPingPacketReceived;
        USNLCallbackEvents.OnDisconnected -= OnDisconnected;
    }

    #endregion

    #region Bytes and Packets

    private IEnumerator BytesAndPacketsPerSecondCoroutine() {
        while (true) {
            if (Client.instance.IsConnected) { SendPingPacket(); }

            // Clear existing data
            bytesSentPerSecond = 0;
            bytesReceivedPerSecond = 0;

            totalPacketsSentPerSecond = 0;
            totalPacketsReceivedPerSecond = 0;

            bytesSentByPacketPerSecond = new int[Enum.GetNames(typeof(ClientPackets)).Length];
            bytesReceivedByPacketPerSecond = new int[Enum.GetNames(typeof(ServerPackets)).Length];

            packetsSentPerSecond = new int[Enum.GetNames(typeof(ClientPackets)).Length];
            packetsReceivedPerSecond = new int[Enum.GetNames(typeof(ServerPackets)).Length];


            // Loop through temp data
            bytesSentByPacketPerSecond = tempBytesSentByPacketPerSecond;
            packetsSentPerSecond = tempPacketsSentPerSecond;
            for (int i = 0; i < tempBytesSentByPacketPerSecond.Length; i++) {
                bytesSentPerSecond += tempBytesSentByPacketPerSecond[i];
            }

            bytesReceivedByPacketPerSecond = tempBytesReceivedByPacketPerSecond;
            packetsReceivedPerSecond = tempPacketsReceivedPerSecond;
            for (int i = 0; i < tempBytesReceivedByPacketPerSecond.Length; i++) {
                bytesReceivedPerSecond += tempBytesReceivedByPacketPerSecond[i];
            }

            for (int i = 0; i < tempPacketsSentPerSecond.Length; i++) {
                totalPacketsSentPerSecond += tempPacketsSentPerSecond[i];
            }

            for (int i = 0; i < tempPacketsReceivedPerSecond.Length; i++) {
                totalPacketsReceivedPerSecond += tempPacketsReceivedPerSecond[i];
            }

            // Reset temp variables
            tempBytesSentByPacketPerSecond = new int[Enum.GetNames(typeof(ClientPackets)).Length];
            tempBytesReceivedByPacketPerSecond = new int[Enum.GetNames(typeof(ServerPackets)).Length];
            tempPacketsSentPerSecond = new int[Enum.GetNames(typeof(ClientPackets)).Length];
            tempPacketsReceivedPerSecond = new int[Enum.GetNames(typeof(ServerPackets)).Length];

            yield return new WaitForSecondsRealtime(1f);
        }
    }

    public void PacketSent(int _packetId, int _bytesLength) {
        totalBytesSent += _bytesLength;

        totalBytesSentByPacket[_packetId] += _bytesLength;
        tempBytesSentByPacketPerSecond[_packetId] += _bytesLength;
        tempPacketsSentPerSecond[_packetId]++;
        totalPacketsSent++;
    }

    public void PacketReceived(int _packetId, int _bytesLength) {
        totalBytesReceived += _bytesLength;

        totalBytesReceivedByPacket[_packetId] += _bytesLength;
        tempBytesReceivedByPacketPerSecond[_packetId] += _bytesLength;
        tempPacketsReceivedPerSecond[_packetId]++;
        totalPacketsReceived++;
    }

    #endregion

    #region Ping

    private void SendPingPacket() {
        // If packet has been received packetPingSentTime will be -1
        if (packetPingSentTime < 0) {
            packetPingSentTime = Time.realtimeSinceStartup;
            PacketSend.Ping(true);
        } else {
            Debug.Log("Packet RTT/ping is greater than 1000ms");
        }
    }

    private void OnPingPacketReceived(object _packetObject) {
        PingPacket pingPacket = (PingPacket)_packetObject;

        // If this is to determine Server -> Client ping time, send packet back, don't get total ping time on Client
        if (pingPacket.SendPingBack) {
            PacketSend.Ping(false);
            return;
        }

        packetRTT = Mathf.RoundToInt((Time.realtimeSinceStartup - packetPingSentTime) * 1000); // Round to ms (*1000), instead of seconds

        // Set smoothPacketRTT
        if (packetRTTs.Count > 5) { packetRTTs.RemoveAt(0); }
        packetRTTs.Add(packetRTT);
        smoothPacketRTT = packetRTTs.Sum() / packetRTTs.Count;

        packetPingSentTime = -1; // Reset Packet Ping Sent Time so SendPingPacket() works
    }

    private void OnDisconnected(object _object) {
        packetPingSentTime = -1; // Reset Packet Ping Sent Time so SendPingPacket() works
    }

    #endregion
}
