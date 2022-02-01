using LowNet.Utils;
using System;
using System.Net;
using System.Net.Sockets;

namespace LowNet.Server.Transport
{
    internal class UDPLayer
    {
        public UDPLayer(int Port)
        {
            Start(Port);
        }
        private static bool isRunning = false;

        internal static UdpClient Listener;

        internal static void Start(int ServerPort)
        {
            Server.Debug("Starting UDP-Layer on: " + ServerPort, Server.Instance);
            Listener = new UdpClient(ServerPort);
            isRunning = true;
            Listener.BeginReceive(ConnectCallBack, null);
        }

        private static void ConnectCallBack(IAsyncResult ar)
        {
            if (isRunning)
            {
                try
                {
                    IPEndPoint client = new IPEndPoint(IPAddress.Any, 0);
                    byte[] data = Listener.EndReceive(ar, ref client);
                    Listener.BeginReceive(ConnectCallBack, null);

                    if (data.Length < 4)
                        return;

                    using (Store store = new Store(data))
                    {
                        int Client = store.PopInt();
                        if (Client == 0)
                            return;
                        if (Server.Clients[Client].Udp.EndPoint == null)
                        {
                            Server.Clients[Client].Udp.Connect(client);
                            return;
                        }
                        if (Server.Clients[Client].Udp.EndPoint == client)
                        {
                            Server.Clients[Client].Udp.ReadPacket(store);
                        }
                    }
                }
                catch (SocketException ex)
                {
                    Server.Error("Failed read data from Socket", ex.Message, Server.Instance);
                }
            }
        }

        internal static void SendUDPData(IPEndPoint endPoint, Store store)
        {
            try
            {
                if (endPoint != null)
                    Listener.BeginSend(store.ToArray, store.Length, endPoint, null, null);
            }
            catch (SocketException ex)
            {
                Server.Error("Failed send data to Socket", ex.Message, Server.Instance);
            }
        }

        public static void Shutdown()
        {
            if (isRunning)
            {
                isRunning = false;
                Listener.Close();
            }
        }
    }
}