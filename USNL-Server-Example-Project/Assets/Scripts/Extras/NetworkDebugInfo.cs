using System;
using System.Collections;
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

    // Too much memory? - adding a clear function, nvm it's just some ints
    // Index is packet Id
    private int[] totalBytesSentByPacket = new int[Enum.GetNames(typeof(ServerPackets)).Length];
    private int[] totalBytesReceivedByPacket = new int[Enum.GetNames(typeof(ClientPackets)).Length];

    // These are the values that can be accessed by the user
    private int[] bytesSentByPacketPerSecond = new int[Enum.GetNames(typeof(ServerPackets)).Length];
    private int[] bytesReceivedByPacketPerSecond = new int[Enum.GetNames(typeof(ClientPackets)).Length];
    private int[] packetsSentPerSecond = new int[Enum.GetNames(typeof(ServerPackets)).Length];
    private int[] packetsReceivedPerSecond = new int[Enum.GetNames(typeof(ClientPackets)).Length];

    // Index is packet Id - Temp because this is added to over a second, then reset
    private int[] tempBytesSentByPacketPerSecond = new int[Enum.GetNames(typeof(ServerPackets)).Length];
    private int[] tempBytesReceivedByPacketPerSecond = new int[Enum.GetNames(typeof(ClientPackets)).Length];
    private int[] tempPacketsSentPerSecond = new int[Enum.GetNames(typeof(ServerPackets)).Length];
    private int[] tempPacketsReceivedPerSecond = new int[Enum.GetNames(typeof(ClientPackets)).Length];

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

    private void OnEnable() { USNLCallbackEvents.OnPingPacket += OnPingPacketReceived; }
    private void OnDisable() { USNLCallbackEvents.OnPingPacket -= OnPingPacketReceived; }

    #endregion

    #region Bytes and Packets

    private IEnumerator BytesAndPacketsPerSecondCoroutine() {
        while (true) {
            // Clear existing data
            bytesSentPerSecond = 0;
            bytesReceivedPerSecond = 0;

            totalPacketsSentPerSecond = 0;
            totalPacketsReceivedPerSecond = 0;

            bytesSentByPacketPerSecond = new int[Enum.GetNames(typeof(ServerPackets)).Length];
            bytesReceivedByPacketPerSecond = new int[Enum.GetNames(typeof(ClientPackets)).Length];

            packetsSentPerSecond = new int[Enum.GetNames(typeof(ServerPackets)).Length];
            packetsReceivedPerSecond = new int[Enum.GetNames(typeof(ClientPackets)).Length];


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
            tempBytesSentByPacketPerSecond = new int[Enum.GetNames(typeof(ServerPackets)).Length];
            tempBytesReceivedByPacketPerSecond = new int[Enum.GetNames(typeof(ClientPackets)).Length];
            tempPacketsSentPerSecond = new int[Enum.GetNames(typeof(ServerPackets)).Length];
            tempPacketsReceivedPerSecond = new int[Enum.GetNames(typeof(ClientPackets)).Length];

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

    private void OnPingPacketReceived(object _packetObject) {
        PingPacket _pingPacket = (PingPacket)_packetObject;

        // If this is to determine Client -> Server ping time, send packet back
        if (_pingPacket.SendPingBack) {
            PacketSend.Ping(_pingPacket.FromClient, false);
            return;
        }
    }

    #endregion
}
