/*  __                  _   __     __ 
   / /   ____ _      __/ | / /__  / /_
  / /   / __ \ | /| / /  |/ / _ \/ __/
 / /___/ /_/ / |/ |/ / /|  /  __/ /_  
/_____/\____/|__/|__/_/ |_/\___/\__/  
Simple Unity3D Solution ©2022 by Kuxii
*/
using LowNet.Data;
using LowNet.Packets;
using LowNet.Unity3D;
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace LowNet.Server.Data
{
    /// <summary>
    /// LowNet Client.
    /// Contains the Import Vales, and Infos.
    /// Session is Attached when client is Connected.
    /// Contains Connecting Time, Last Packet, PlayerId and More
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Get ConnectionId
        /// </summary>
        public int ClientId { get; private set; }
        /// <summary>
        /// Get Client Session
        /// </summary>
        public Session Session { get; set; }
        private readonly LowNetServer Mainserver;
        private static readonly int dataBufferSize = 4096;
        internal TCP tcp;
        internal UDP udp;

        /// <summary>
        /// Create new Playerslot
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="server"></param>
        public Client(int clientId, LowNetServer server)
        {
            if (server == null)
                return;

            Mainserver = server;
            ClientId = clientId;
            tcp = new TCP(ClientId, this);
            udp = new UDP(ClientId, this);
            Mainserver.Debug("Create Playerslot " + clientId, this);
        }

        /// <summary>
        /// Client TCP Layer
        /// </summary>
        public class TCP
        {
            /// <summary>
            /// Client public Socket
            /// </summary>
            public TcpClient Socket;
            private readonly int ClientId;
            private readonly Client client;
            private NetworkStream stream;
            private Store receivedData;
            private byte[] receiveBuffer;

            /// <summary>
            /// Create new TCP Layer
            /// </summary>
            /// <param name="_id"></param>
            /// <param name="newclient"></param>
            public TCP(int _id, Client newclient)
            {
                client = newclient;
                ClientId = _id;
            }

            /// <summary>
            /// Connect to Client
            /// </summary>
            /// <param name="socket"></param>
            public void Connect(TcpClient socket)
            {
                try
                {
                    client.Session = new Session(ClientId, client.Mainserver)
                    {
                        Connected = DateTime.Now,
                        Lastpacket = DateTime.Now
                    };
                    Socket = socket;
                    socket.ReceiveBufferSize = dataBufferSize;
                    socket.SendBufferSize = dataBufferSize;

                    //2mins KeepAlaive
                    int size = Marshal.SizeOf((uint)0);
                    byte[] keepAlive = new byte[size * 3];
                    Buffer.BlockCopy(BitConverter.GetBytes((uint)1), 0, keepAlive, 0, size);
                    Buffer.BlockCopy(BitConverter.GetBytes((uint)120000), 0, keepAlive, size, size);
                    Buffer.BlockCopy(BitConverter.GetBytes((uint)120000), 0, keepAlive, size * 2, size);
                    socket.Client.IOControl(IOControlCode.KeepAliveValues, keepAlive, null);
                    socket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

                    stream = socket.GetStream();
                    receivedData = new Store();
                    receiveBuffer = new byte[dataBufferSize];
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                }
                catch (Exception ex)
                {
                    client.Mainserver.Error($"Failed, Connect to Client. {ex.Message}", this);
                }
                finally
                {
                    Sendresponse();
                    client.Mainserver.InvokePlayerconnect(client);
                }
            }

            /// <summary>
            /// Send Data to Socket
            /// </summary>
            /// <param name="store"></param>
            public void SendData(Store store)
            {
                try
                {
                    if (Socket != null)
                    {
                        store.WriteLength();
                        stream.BeginWrite(store.ToArray, 0, store.Length, null, null);
                        store.Dispose();
                    }
                }
                catch (Exception)
                {
                }
            }

            private void ReceiveCallback(IAsyncResult result)
            {
                try
                {
                    int byteLength = stream.EndRead(result);
                    if (byteLength <= 0)
                    {
                        client.Disconnect();
                        return;
                    }

                    byte[] data = new byte[byteLength];
                    Array.Copy(receiveBuffer, data, byteLength);

                    receivedData.Reset(HandleData(data));
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                }
                catch (Exception)
                {
                    client.Disconnect();
                }
            }

            private bool HandleData(byte[] data)
            {
                client.Session.Lastpacket = DateTime.Now;
                int packetLength = 0;

                receivedData.SetBytes(data);

                if (receivedData.UnreadLength >= 4)
                {
                    packetLength = receivedData.PopInt();
                    if (packetLength <= 0)
                    {
                        return true;
                    }
                }

                while (packetLength > 0 && packetLength <= receivedData.UnreadLength)
                {
                    byte[] packetBytes = receivedData.PopBytes(packetLength);
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        Store store = new Store(packetBytes);
                        int packetId = store.PopInt();
                        LowNetServer.Serverpackets[packetId](client, store);
                    });

                    packetLength = 0;
                    if (receivedData.UnreadLength >= 4)
                    {
                        packetLength = receivedData.PopInt();
                        if (packetLength <= 0)
                        {
                            return true;
                        }
                    }
                }

                if (packetLength <= 1)
                {
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Disconnect Socket
            /// </summary>
            public void Disconnect()
            {
                Socket.Close();
                stream = null;
                receivedData = null;
                receiveBuffer = null;
                Socket = null;
            }

            /// <summary>
            /// Send Connect Response
            /// </summary>
            public void Sendresponse()
            {
                var store = new Store(LowNetpacketOrder.LOWNET_CONNECT);
                store.PushAscii("LowNet");
                store.PushInt(ClientId);
                store.PushInt(client.Mainserver.GetPlayer);
                store.PushInt(client.Mainserver.GetMaxplayer);
                store.PushAscii(client.Mainserver.GetServerpassword);
                store.PushAscii(client.Mainserver.GetServername);
                store.PushAscii(client.Session.GetUUID);
                SendData(store);
            }
        }

        /// <summary>
        /// Client UDP Layer
        /// </summary>
        public class UDP
        {
            /// <summary>
            /// Client public Endpoint
            /// </summary>
            public IPEndPoint EndPoint;
            private readonly Client client;

            /// <summary>
            /// Create new UDP Layer
            /// </summary>
            /// <param name="id"></param>
            /// <param name="newclient"></param>
            public UDP(int id, Client newclient)
            {
                client = newclient;
                Console.WriteLine("UDp Client " + id);
            }

            /// <summary>
            /// Connect UDP Socket
            /// </summary>
            /// <param name="endPoint"></param>
            public void Connect(IPEndPoint endPoint)
            {
                EndPoint = endPoint;
            }

            /// <summary>
            /// Send Data
            /// </summary>
            /// <param name="store"></param>
            public void SendData(Store store)
            {
                store.WriteLength();
                client.Mainserver.UDPLayer.SendUDP(EndPoint, store);
                store.Dispose();
            }

            /// <summary>
            /// Handle Client Data
            /// </summary>
            /// <param name="packetData"></param>
            public void HandleData(Store packetData)
            {
                client.Session.Lastpacket = DateTime.Now;
                int packetLength = packetData.PopInt();
                byte[] packetBytes = packetData.PopBytes(packetLength);

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    Store store = new Store(packetBytes);
                    int packetId = store.PopInt();
                    LowNetServer.Serverpackets[packetId](client, store);
                });
            }

            /// <summary>
            /// Disconnect UDP Socket
            /// </summary>
            public void Disconnect()
            {
                EndPoint = null;
            }
        }

        /// <summary>
        /// Disconnect Client from Server
        /// </summary>
        public void Disconnect()
        {
            Mainserver.InvokePlayerDisconnect(this);
            Session = null;
            tcp.Disconnect();
            udp.Disconnect();
        }
    }
}