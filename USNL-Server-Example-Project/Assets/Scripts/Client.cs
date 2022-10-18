using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Client {
    #region Variables & Core

    public static int dataBufferSize = 4096; // (TAG: OLDCODE) This should be set by the user
    private TCP tcp;
    private UDP udp;

    private int clientId;

    private bool isConnected = false;

    public TCP Tcp { get => tcp; set => tcp = value; }
    public UDP Udp { get => udp; set => udp = value; }
    public int ClientId { get => clientId; set => clientId = value; }
    public bool IsConnected { get => isConnected; set => isConnected = value; }

    public Client(int _clientID) {
        clientId = _clientID;
        tcp = new TCP(clientId, this);
        udp = new UDP(clientId);
    }

    #endregion

    #region TCP & UDP

    public class TCP {
        public TcpClient socket;

        private readonly int clientId;
        private NetworkStream stream;
        private Packet receivedData;
        private byte[] receiveBuffer;

        Client client;

        public TCP(int _id, Client _client) {
            clientId = _id;
            client = _client;
        }

        public void Connect(TcpClient _socket) {
            socket = _socket;
            socket.ReceiveBufferSize = dataBufferSize;
            socket.SendBufferSize = dataBufferSize;

            stream = socket.GetStream();

            receivedData = new Packet();
            receiveBuffer = new byte[dataBufferSize];

            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

            //ServerSend.Welcome(id, "Welcome to the server!"); UPDATE THIS WHEN PACKET CODE IS WRITTEN

            client.isConnected = true;
        }

        public void SendData(Packet _packet) {
            try {
                if (socket != null) {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
            } catch (Exception _ex) {
                Debug.Log($"Error sending data to client {clientId} via TCP: {_ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult _result) {
            try {
                int _byteLength = stream.EndRead(_result);
                if (_byteLength <= 0) {
                    Server.clients[clientId].Disconnect(clientId, true);
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(receiveBuffer, _data, _byteLength);

                receivedData.Reset(HandleData(_data));
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            } catch (Exception _ex) {
                Debug.Log($"Error recieving TCP data: {_ex}");
                Server.clients[clientId].Disconnect(clientId, true);
            }
        }


        private bool HandleData(byte[] _data) {
            int _packetLength = 0;

            receivedData.SetBytes(_data);

            if (receivedData.UnreadLength() >= 4) {
                _packetLength = receivedData.ReadInt();
                if (_packetLength <= 0) {
                    return true;
                }
            }

            while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength()) {
                byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                ThreadManager.ExecuteOnMainThread(() => {
                    using (Packet _packet = new Packet(_packetBytes)) {
                        _packet.PacketId = _packet.ReadInt();
                        _packet.FromClient = clientId;
                        PacketHandlers.packetHandlers[_packet.PacketId](_packet);
                    }
                });

                _packetLength = 0;
                if (receivedData.UnreadLength() >= 4) {
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0) {
                        return true;
                    }
                }
            }

            if (_packetLength <= 1) {
                return true;
            }

            return false;
        }

        public void Disconnect() {
            socket.Close();
            stream = null;
            receivedData = null;
            receiveBuffer = null;
            socket = null;
        }
    }

    public class UDP {
        public IPEndPoint endPoint;

        private int clientId;

        public UDP(int _id) {
            clientId = _id;
        }

        public void Connect(IPEndPoint _endPoint) {
            endPoint = _endPoint;
        }

        public void SendData(Packet _packet) {
            Server.SendUDPData(endPoint, _packet);
        }

        public void handleData(Packet _packetData) {
            int _packetLength = _packetData.ReadInt();
            byte[] _packetBytes = _packetData.ReadBytes(_packetLength);


            ThreadManager.ExecuteOnMainThread(() => {
                using (Packet _packet = new Packet(_packetBytes)) {
                    _packet.PacketId = _packet.ReadInt();
                    _packet.FromClient = clientId;
                    PacketHandlers.packetHandlers[_packet.PacketId](_packet);
                }
            });
        }

        public void Disconnect() {
            endPoint = null;
        }
    }

    #endregion

    #region Functions

    public void Disconnect(int _clientId, bool _forcablyDisconnected = false) {
        if (_forcablyDisconnected) {
            Debug.Log($"{tcp.socket.Client.RemoteEndPoint} has been forcably disconnected. - I think this is a bug right here");
        } else {
            Debug.Log($"{tcp.socket.Client.RemoteEndPoint} has disconnected.");
        }

        isConnected = false;

        ThreadManager.ExecuteOnMainThread(() => {
            //NetworkManager.instance.CallClientDisconnectedCallbacks(_clientId);
        });

        tcp.Disconnect();
        udp.Disconnect();
    }

    #endregion
}
