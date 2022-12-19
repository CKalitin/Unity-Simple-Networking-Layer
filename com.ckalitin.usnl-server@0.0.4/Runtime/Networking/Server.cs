using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using UnityEngine;

public class Server {
    #region Variables

    public static int MaxClients { get; private set; }

    public static int Port { get; private set; }

    public static List<Client> Clients = new List<Client>();

    public static bool ServerActive = false;

    private static TcpListener tcpListener;
    private static UdpClient udpListener;

    #endregion

    #region Core

    public static void Start(int _maxClients, int _port) {
        MaxClients = _maxClients;
        Port = _port;

        Debug.Log("Starting server...");
        InitializeServerData();

        ThreadManager.StartPacketHandleThread();

        tcpListener = new TcpListener(IPAddress.Any, Port);
        tcpListener.Start();
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

        udpListener = new UdpClient(Port);
        udpListener.BeginReceive(UDPReceiveCallback, null);

        InputManager.instance.Initialize();

        ServerActive = true;

        USNLCallbackEvents.CallOnServerStartedCallbacks(0);

        Debug.Log($"Server started on port {Port}.");
    }

    private static void InitializeServerData() {
        for (int i = 0; i <= MaxClients; i++) {
            Clients.Add(new Client(i));
        }
    }

    public static void Stop() {
        tcpListener.Stop();
        udpListener.Close();
        
        ThreadManager.StopPacketHandleThread();

        ServerActive = false;

        USNLCallbackEvents.CallOnServerStoppedCallbacks(0);

        Debug.Log("Server stopped.");
    }

    public static void DisconnectAllClients() {
        for (int i = 0; i < Clients.Count; i++) {
            if (Clients[i].IsConnected) Clients[i].Disconnect();
        }
    }

    public static int GetConnectedClients() {
        int result = 0;
        for (int i = 0; i < Clients.Count; i++) {
            if (Clients[i].IsConnected) {
                result++;
            }
        }
        return result;
    }

    #endregion

    #region TCP & UDP

    private static void TCPConnectCallback(IAsyncResult _result) {
        TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);
        Debug.Log($"Incoming connection from {_client.Client.RemoteEndPoint}...");

        for (int i = 0; i <= MaxClients; i++) {
            if (Clients[i].Tcp.socket == null) {
                Clients[i].Tcp.Connect(_client);
                return;
            }
        }

        Debug.Log($"{_client.Client.RemoteEndPoint} failed to connect: Server full.");
    }

    private static void UDPReceiveCallback(IAsyncResult _result) {
        try {
            IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] _data = udpListener.EndReceive(_result, ref _clientEndPoint);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            if (_data.Length < 4) {
                return;
            }

            using (Packet _packet = new Packet(_data)) {
                int _clientId = _packet.ReadInt();

                if (Clients[_clientId].Udp.endPoint == null) {
                    Clients[_clientId].Udp.Connect(_clientEndPoint);
                    return;
                }

                if (Clients[_clientId].Udp.endPoint.ToString() == _clientEndPoint.ToString()) {
                    Clients[_clientId].Udp.HandleData(_packet);
                }
            }
        } catch (Exception _ex) {
            Debug.Log($"Error receoving UDP data: {_ex}");
        }
    }

    public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet) {
        try {
            if (_clientEndPoint != null) {
                udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
            }
        } catch (Exception _ex) {
            Debug.Log($"Error sending data tp {_clientEndPoint} via UDP {_ex}");
        }
    }

    #endregion
}
