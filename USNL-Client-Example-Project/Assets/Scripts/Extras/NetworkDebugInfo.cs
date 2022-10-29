using System;
using System.Collections;
using System.Net.NetworkInformation;
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
    [SerializeField] private int pingMs;
    [SerializeField] private long pingMsLong;
    [SerializeField] private int unityPingMs;
    [SerializeField] private int packetPingMs;
    [Space]
    [SerializeField] private bool trackPing = true;

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
            if (trackPing && Client.instance.IsConnected) {
                PingReply();
                StartCoroutine(UnityPing());
                SendPingPacket();
            }

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

    private void PingReply() {
        System.Net.NetworkInformation.Ping pingSender = new System.Net.NetworkInformation.Ping();
        PingReply pingReply = pingSender.Send(Client.instance.ip, 5000);

        Debug.Log("Before");

        if (pingReply.Status == IPStatus.Success) {
            pingMs = (int)pingReply.RoundtripTime;
            pingMsLong = pingReply.RoundtripTime;
        } else {
            pingMs = -1;
        }

        Debug.Log("After");
    }

    private IEnumerator UnityPing() {
        UnityEngine.Ping ping = new UnityEngine.Ping(Client.instance.ip);

        float timeout = 0;
        while (!ping.isDone) {
            yield return new WaitForEndOfFrame();

            timeout += Time.deltaTime;

            if (timeout > 5000) { yield break; }
        }

        if (ping.isDone) {
            unityPingMs = ping.time;
        } else {
            unityPingMs = -1;
        }
    }

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

        Debug.Log(Mathf.RoundToInt((Time.realtimeSinceStartup - packetPingSentTime) * 1000));
        packetPingMs = Mathf.RoundToInt((Time.realtimeSinceStartup - packetPingSentTime) * 1000); // Round to ms (*1000), instead of seconds
        packetPingSentTime = -1;
    }

    private void OnDisconnected(object _object) {
        packetPingSentTime = -1; // Reset Packet Ping Sent Time so SendPingPacket() works
    }

    #endregion
}
