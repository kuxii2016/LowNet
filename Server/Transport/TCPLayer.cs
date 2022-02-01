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

        private static bool isRunning = false;

        internal static TcpListener Listener { get; set; }

        internal static void Start(int ServerPort)
        {
            Server.Debug("Starting TCP-Layer on: " + ServerPort, Server.Instance);
            Listener = new TcpListener(IPAddress.Any, ServerPort);
            isRunning = true;
            Listener.Start();
            Listener.BeginAcceptTcpClient(ConnectCallback, null);
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            if (isRunning)
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

        public static void Shutdown()
        {
            if (isRunning)
            {
                for (int i = 1; i < Server.Clients.Count; i++)
                {
                    if (Server.Clients[i].Tcp.Socket != null)
                    {
                        Server.Clients[i].Disconnect();
                    }
                }
                isRunning = false;
                Listener.Stop();
            }
        }
    }
}