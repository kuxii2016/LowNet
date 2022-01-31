using System;
using System.Net;
using System.Net.Sockets;

namespace LowNet.Server.Transport
{
    internal class TCPLayer
    {
        public TCPLayer(int Port)
        {
            Start(Port);
        }
        internal static TcpListener Listener { get; set; }

        internal static void Start(int ServerPort)
        {
            Listener = new TcpListener(IPAddress.Any, ServerPort);
            Listener.Start();
            Listener.BeginAcceptTcpClient(ConnectCallback, null);
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            TcpClient client = Listener.EndAcceptTcpClient(ar);
            Listener.BeginAcceptTcpClient(ConnectCallback, null);

            for (int i = 1; i < Server.Clients.Count; i++)
            {
                if (Server.Clients[i].Tcp.Socket == null)
                {
                    Server.Clients[i].Tcp.Connect(client);
                    return;
                }
            }
        }
    }
}