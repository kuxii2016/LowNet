/*  __                  _   __     __ 
   / /   ____ _      __/ | / /__  / /_
  / /   / __ \ | /| / /  |/ / _ \/ __/
 / /___/ /_/ / |/ |/ / /|  /  __/ /_  
/_____/\____/|__/|__/_/ |_/\___/\__/  
Simple Unity3D Solution ©2022 by Kuxii
*/
using LowNet.Data;
using LowNet.Server;
using LowNet.Server.Data;
using LowNet.Serverstore;
using System;
using System.Net;
using System.Net.Sockets;

namespace LowNet.Transport
{
    /// <summary>
    /// TCP Transport Layer
    /// </summary>
    public class TCPLayer
    {
        /// <summary>
        /// Create new TCP Layer
        /// </summary>
        /// <param name="port"></param>
        /// <param name="server"></param>
        public TCPLayer(int port, LowNetServer server)
        {
            this.Port = port;
            this.Mainserver = server;
            Mainserver.Debug("Init new TCP-Transportlayer.", this);
        }

        /// <summary>
        /// This Listener
        /// </summary>
        private static TcpListener Listener;
        /// <summary>
        /// Mainserver Instance for Logging
        /// </summary>
        private LowNetServer Mainserver { get; set; }
        /// <summary>
        /// Is TCP Layer Running
        /// </summary>
        public bool IsRunning { get; private set; } = false;
        /// <summary>
        /// Get Serverport
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// Send data only to Client
        /// </summary>
        /// <param name="client"></param>
        /// <param name="store"></param>
        public void Send(Client client, Store store)
        {
            client.tcp.SendData(store);
        }

        /// <summary>
        /// Send data to All Clients
        /// </summary>
        /// <param name="store"></param>
        public void SendAll(Store store)
        {
            for (int i = 0; i < Playerstore.Instance.Clients.Count; i++)
            {
                Playerstore.Instance.Clients[i].tcp.SendData(store);
            }
        }

        /// <summary>
        /// Send data to all Clients only not this Client
        /// </summary>
        /// <param name="client"></param>
        /// <param name="store"></param>
        public void SendAll(Client client, Store store)
        {
            for (int i = 0; i < Playerstore.Instance.Clients.Count; i++)
            {
                if (Playerstore.Instance.Clients[i] != client)
                    Playerstore.Instance.Clients[i].tcp.SendData(store);
            }
        }

        /// <summary>
        /// Send data only to Client
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="store"></param>
        public void Send(int clientId, Store store)
        {
            Playerstore.Instance.Clients[clientId].tcp.SendData(store);
        }

        /// <summary>
        /// Send data to all Clients only not this Client
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="store"></param>
        public void SendAll(int clientId, Store store)
        {
            for (int i = 0; i < Playerstore.Instance.Clients.Count; i++)
            {
                if (Playerstore.Instance.Clients[i].ClientId != i)
                    Playerstore.Instance.Clients[i].tcp.SendData(store);
            }
        }

        /// <summary>
        /// Stop TCP Layer
        /// </summary>
        public void Shutdown()
        {
            if (IsRunning)
            {
                Listener.Stop();
                Mainserver.Log("Stopped TCP-Layer", this);
            }
        }

        /// <summary>
        /// Start TCP Layer
        /// </summary>
        public void Start()
        {
            try
            {

            }
            catch (SocketException so)
            {
                Mainserver.Error("Failed Starting TCP-Layer: " + so.Message, this);
                return;
            }
            catch (Exception ex)
            {
                Mainserver.Error("Failed Starting TCP-Layer: " + ex.Message, this);
                return;
            }
            finally
            {
                IsRunning = true;
                Listener = new TcpListener(IPAddress.Any, Port);
                Listener.Start();
                Listener.BeginAcceptTcpClient(new AsyncCallback(ConnectCallBack), null);
                Mainserver.Log("Started TCP-Layer on Port: " + Port, this);
            }
        }

        /// <summary>
        /// Tcp Callback
        /// </summary>
        /// <param name="ar"></param>
        private void ConnectCallBack(IAsyncResult ar)
        {
            TcpClient client = Listener.EndAcceptTcpClient(ar);
            Listener.BeginAcceptTcpClient(new AsyncCallback(ConnectCallBack), null);

            for (int i = 1; i < Playerstore.Instance.Clients.Count; i++)
            {
                if (Playerstore.Instance.Clients[i].tcp.Socket == null)
                {
                    Playerstore.Instance.Clients[i].tcp.Connect(client);
                    return;
                }
            }
            Mainserver.Warning("Client tryed Connect to Fullserver!", this);
        }
    }
}