using LowNet.Utils;
using System;
using System.Net;
using System.Net.Sockets;

namespace LowNet.Server.Transport
{
    class UDPLayer
    {
        public UDPLayer(int Port)
        {
            Start(Port);
        }

        internal static UdpClient Listener;

        internal static void Start(int ServerPort)
        {
            Listener = new UdpClient(ServerPort);
            Listener.BeginReceive(ConnectCallBack, null);
        }

        private static void ConnectCallBack(IAsyncResult ar)
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
            catch
            {
                //Receive error
            }
        }

        internal static void SendUDPData(IPEndPoint endPoint, Store store)
        {
            try
            {
                if (endPoint != null)
                    Listener.BeginSend(store.ToArray, store.Length, endPoint, null, null);
            }
            catch
            {
                //Send Error
            }
        }
    }
}