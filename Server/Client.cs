using LowNet.Server.Packets;
using LowNet.Server.Transport;
using LowNet.Unity3D;
using LowNet.Utils;
using System;
using System.Net;
using System.Net.Sockets;

namespace LowNet.Server
{
    /// <summary>
    /// LowNet Connection Client
    /// </summary>
    public class Client
    {
        #region Public
        /// <summary>
        /// Playersession with most Infos about Player
        /// </summary>
        public Session Session;
        /// <summary>
        /// Last Timestamp from Last Packet
        /// </summary>
        public static DateTime LastPacket { get; private set; }

        /// <summary>
        /// Connection GUID
        /// </summary>
        public string ConnectionGuid { get; set; }

        /// <summary>
        /// Playername
        /// </summary>
        public string PlayerName { get; set; }

        #endregion Public

        #region Private

        private const int BufferSize = 4069;

        /// <summary>
        /// Get Client Id
        /// </summary>
        public int Connectionid { get; private set; }

        internal TCP Tcp;
        internal UDP Udp;

        #endregion Private

        /// <summary>
        /// Create new Client
        /// </summary>
        /// <param name="clientid"></param>
        public Client(int clientid)
        {
            Connectionid = clientid;
            Tcp = new TCP(Connectionid);
            Udp = new UDP(Connectionid);
        }

        internal class TCP
        {
            public TcpClient Socket;
            private NetworkStream Stream;
            private byte[] ReceivedBytes;
            private Store Received;
            private readonly int ClientId;

            public TCP(int Client)
            {
                ClientId = Client;
            }

            public void Connect(TcpClient client)
            {
                Socket = client;
                Socket.ReceiveBufferSize = BufferSize;
                Socket.SendBufferSize = BufferSize;
                Stream = Socket.GetStream();
                Received = new Store();
                ReceivedBytes = new byte[BufferSize];
                Stream.BeginRead(ReceivedBytes, 0, BufferSize, EndRead, null);
                LOWNET_CONNECT.SendPacket(Server.Clients[ClientId]);
                Server.Log("Player: " + ClientId + " Joint the Game", Server.Instance);
            }

            public void SendData(Store store)
            {
                try
                {
                    if (Socket != null)
                    {
                        Stream.BeginWrite(store.ToArray, 0, store.Length, null, null);
                    }
                }
                catch (Exception ex)
                {
                    Server.Error("Failed send data via TCP-Layer", ex.Message, this);
                }
            }

            private void EndRead(IAsyncResult ar)
            {
                LastPacket = DateTime.Now;
                try
                {
                    int bytes = Stream.EndRead(ar);
                    if (bytes <= 0)
                    {
                        Server.Error("Got Empty Packet", "Empty Packets not Allowed", this);
                        Server.Clients[ClientId].Disconnect();
                        return;
                    }

                    byte[] data = new byte[bytes];
                    Array.Copy(ReceivedBytes, data, bytes);
                    Received.Reset(Readpacket(data));
                    Stream.BeginRead(ReceivedBytes, 0, BufferSize, EndRead, null);
                }
                catch (Exception)
                {
                    Server.Error("Failed read data from Client: " + ClientId, "", this);
                    Server.Clients[ClientId].Disconnect();
                }
            }

            private bool Readpacket(byte[] data)
            {
                int Lenght = 0;
                Received.SetBytes(data);
                if (Received.UnreadLength >= 4)
                {
                    Lenght = Received.PopInt();
                    if (Lenght <= 0)
                    {
                        return true;
                    }
                }

                while (Lenght > 0 && Lenght <= Received.UnreadLength)
                {
                    byte[] packetBytes = Received.PopBytes(Lenght);
                    ServerNetworkmanager.ExecuteOnMainThread(() =>
                    {
                        using (Store store = new Store(packetBytes))
                        {
                            int PacketId = store.PopInt();
                            Server.Packets[PacketId](Server.Clients[ClientId], store);
                        }
                    });
                    Lenght = 0;
                    if (Received.UnreadLength >= 4)
                    {
                        Lenght = Received.PopInt();
                        if (Lenght <= 0)
                        {
                            return true;
                        }
                    }
                }

                if (Lenght <= 1)
                {
                    return true;
                }

                return false;
            }

            public void Disconnect()
            {
                Socket.Close();
                Stream = null;
                ReceivedBytes = null;
                Socket = null;
            }
        }

        internal class UDP
        {
            public IPEndPoint EndPoint;
            private readonly int ClientId;

            public UDP(int Client)
            {
                ClientId = Client;
            }

            public void Connect(IPEndPoint endPoint)
            {
                EndPoint = endPoint;
            }

            public void Send(Store store)
            {
                UDPLayer.SendUDPData(EndPoint, store);
            }

            public void ReadPacket(Store store)
            {
                LastPacket = DateTime.Now;
                int Lenght = store.PopInt();
                byte[] data = store.PopBytes(Lenght);

                ServerNetworkmanager.ExecuteOnMainThread(() =>
                {
                    using (Store store1 = new Store(data))
                    {
                        int PacketId = store1.PopInt();
                        Server.Packets[PacketId](Server.Clients[ClientId], store1);
                    }
                });
            }

            public void Disconnect()
            {
                EndPoint = null;
            }
        }

        /// <summary>
        /// Disconnect this Client from Server
        /// </summary>
        public void Disconnect()
        {
            Server.Log("Player: " + Connectionid + " Left the Game", Server.Instance);
            ServerNetworkmanager.ExecuteOnMainThread(() =>
            {
                Server.OnPlayerDisconnect(this);
                //TODO: Call Unity3D Event
                Tcp.Disconnect();
                Udp.Disconnect();
                Session = null;
            });
        }

        /// <summary>
        /// This will Send Own Player, Other Player to old and the new Player to the Old
        /// </summary>
        public void SendPlayers()
        {
            Server.OnPlayerconnect(this);

            Server.Debug("Spawn All Player for new Connection.", Server.Instance);
            foreach (var player in Server.Clients.Values)
            {
                if(player.Session != null)
                {
                    if(player.Connectionid != Connectionid)
                        LOWNET_PLAYER.SendPacket(this, player, true);
                }
            }

            Server.Debug("Spawn new Player to Old Players", Server.Instance);
            foreach (var player in Server.Clients.Values)
            {
                if (player.Session != null)
                {
                    LOWNET_PLAYER.SendPacket(player, this, true);
                }
            }
        }

        /// <summary>
        /// This will Send all Current Objeckt
        /// </summary>
        public void SendObjeckt()
        {
        }
    }
}