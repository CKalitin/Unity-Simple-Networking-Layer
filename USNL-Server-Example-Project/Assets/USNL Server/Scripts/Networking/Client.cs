using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace USNL.Package {
    public class Client {
        #region Variables & Core

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
            tcp = new TCP(this);
            udp = new UDP(this);
        }

        #endregion

        #region TCP & UDP

        public class TCP {
            public TcpClient socket;
            
            private NetworkStream stream;
            private Packet receivedData;
            private byte[] receiveBuffer;

            private DateTime lastPacketTime; // Time when the last packet was received

            Client client;

            public DateTime LastPacketTime { get => lastPacketTime; set => lastPacketTime = value; }

            public TCP(Client _client) {
                client = _client;
                lastPacketTime = DateTime.Now;
            }

            public void Connect(TcpClient _socket) {
                socket = _socket;
                socket.ReceiveBufferSize = USNL.Package.Server.DataBufferSize;
                socket.SendBufferSize = USNL.Package.Server.DataBufferSize;

                stream = socket.GetStream();

                receivedData = new Packet();
                receiveBuffer = new byte[USNL.Package.Server.DataBufferSize];

                stream.BeginRead(receiveBuffer, 0, USNL.Package.Server.DataBufferSize, ReceiveCallback, null);

                lastPacketTime = DateTime.Now;

                USNL.Package.PacketSend.Welcome(client.clientId, client.clientId, USNL.ServerManager.instance.ServerConfig.WelcomeMessage);

                USNL.Package.PacketSend.ServerInfo(client.clientId, USNL.ServerManager.instance.ServerConfig.ServerName, USNL.ServerManager.GetConnectedClientsIds(), USNL.ServerManager.instance.ServerConfig.MaxClients, USNL.ServerManager.GetNumberOfConnectedClients() > USNL.ServerManager.instance.ServerConfig.MaxClients);

                client.isConnected = true;
            }

            public void SendData(Packet _packet) {
                try {
                    if (socket != null) {
                        stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                    }
                } catch (Exception _ex) {
                    Debug.Log($"Error sending data to client {client.clientId} via TCP: {_ex}");
                }
            }

            private void ReceiveCallback(IAsyncResult _result) {
                try {
                    int _byteLength = stream.EndRead(_result);
                    
                    if (_byteLength <= 0) {
                        if (client.clientId > 1000000) {
                            if (Server.WaitingLobbyClients.Contains(client)) Server.WaitingLobbyClients[client.clientId - 1000000].Disconnect();
                        } else {
                            if (Server.Clients.Contains(client)) Server.Clients[client.clientId].Disconnect();
                        }
                        return;
                    }

                    byte[] _data = new byte[_byteLength];
                    Array.Copy(receiveBuffer, _data, _byteLength);

                    receivedData.Reset(HandleData(_data));
                    stream.BeginRead(receiveBuffer, 0, USNL.Package.Server.DataBufferSize, ReceiveCallback, null);
                } catch (Exception _ex) {
                    Debug.Log($"Error recieving TCP data: {_ex}");
                    if (client.clientId > 1000000) Server.WaitingLobbyClients[client.clientId - 1000000].Disconnect();
                    else Server.Clients[client.clientId].Disconnect();
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
                    ThreadManager.ExecuteOnPacketHandleThread(() => {
                        using (Packet _packet = new Packet(_packetBytes)) {
                            lastPacketTime = DateTime.Now;

                            _packet.PacketId = _packet.ReadInt();
                            _packet.FromClient = client.clientId;
                            USNL.Package.PacketHandlers.packetHandlers[_packet.PacketId](_packet);
                            NetworkDebugInfo.instance.PacketReceived(_packet.PacketId, _packet.Length() + 4); // +4 for packet length
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
            
            Client client;

            public UDP(Client _client) {
                client = _client;
            }

            public void Connect(IPEndPoint _endPoint) {
                endPoint = _endPoint;
            }

            public void SendData(Packet _packet) {
                Server.SendUDPData(endPoint, _packet);
            }

            public void HandleData(Packet _packetData) {
                int _packetLength = _packetData.ReadInt();
                byte[] _packetBytes = _packetData.ReadBytes(_packetLength);


                ThreadManager.ExecuteOnPacketHandleThread(() => {
                    using (Packet _packet = new Packet(_packetBytes)) {
                        _packet.PacketId = _packet.ReadInt();
                        _packet.FromClient = client.clientId;
                        USNL.Package.PacketHandlers.packetHandlers[_packet.PacketId](_packet);
                        NetworkDebugInfo.instance.PacketReceived(_packet.PacketId, _packet.Length() + 4); // +4 for packet length
                    }
                });
            }

            public void Disconnect() {
                endPoint = null;
            }
        }

        #endregion

        #region Functions

        public void Disconnect() {
            tcp.Disconnect();
            udp.Disconnect();

            isConnected = false;

            if (USNL.Package.Server.Clients.Contains(this)) {
                Debug.Log("Not in server waiting lobby");

                ThreadManager.ExecuteOnMainThread(() => {
                    ServerManager.instance.ClientDisconnected(clientId);
                });

                Debug.Log($"{tcp.socket.Client.RemoteEndPoint} has disconnected.");

                USNL.Package.Server.Clients[clientId] = new Client(clientId);
            } else {
                Debug.Log($"{tcp.socket.Client.RemoteEndPoint} has disconnected from Waiting Lobby.");
            }
        }

        #endregion
    }
}
