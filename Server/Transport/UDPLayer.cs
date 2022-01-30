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
    /// UDP Transport Layer
    /// </summary>
    public class UDPLayer
    {
        /// <summary>
        /// Create new UDP Layer
        /// </summary>
        /// <param name="port"></param>
        /// <param name="server"></param>
        public UDPLayer(int port, LowNetServer server)
        {
            this.Port = port;
            this.Mainserver = server;
            Mainserver.Debug("Init new UDP-Transportlayer.", this);
        }

        /// <summary>
        /// This Listener
        /// </summary>
        private UdpClient Listener;

        /// <summary>
        /// Mainserver Instance for Logging
        /// </summary>
        private LowNetServer Mainserver { get; set; }

        /// <summary>
        /// Is UDP Layer Running
        /// </summary>
        public bool IsRunning { get; private set; } = false;

        /// <summary>
        /// Get Serverport
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// Send UDP Data to Player
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="store"></param>
        public void SendUDP(IPEndPoint endPoint, Store store)
        {
            try
            {
                if (!IsRunning)
                    Mainserver.Warning("UDP-Layer is not Running to send data.", this);
                if (endPoint != null)
                    Listener.BeginSend(store.ToArray, store.Length, endPoint, null, null);
            }
            catch (SocketException ex)
            {
                Mainserver.Error("Error sending Data: " + ex.Message, this);
            }
        }

        /// <summary>
        /// Stop UDP Layer
        /// </summary>
        public void Shutdown()
        {
            if (IsRunning)
            {
                Listener.Close();
                Mainserver.Log("Stopped UDP-Layer", this);
            }
        }

        /// <summary>
        /// Start UDP Layer
        /// </summary>
        public void Start()
        {
            try
            {
                Listener = new UdpClient(Port);
            }
            catch (SocketException so)
            {
                Mainserver.Error("Failed Starting UDP-Layer: " + so.Message, this);
                return;
            }
            catch (Exception ex)
            {
                Mainserver.Error("Failed Starting UDP-Layer: " + ex.Message, this);
                return;
            }
            finally
            {
                IsRunning = true;
                Listener.BeginReceive(ReceiveCallback, null);
                Mainserver.Log("Started UDP-Layer on Port: " + Port, this);
            }
        }

        /// <summary>
        /// UDP Callback
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                if (IsRunning)
                {
                    IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] data = Listener.EndReceive(ar, ref clientEndPoint);
                    Listener.BeginReceive(ReceiveCallback, null);

                    if (data.Length < 4)
                    {
                        return;
                    }

                    using (Store store = new Store(data))
                    {
                        int ClientId = store.PopInt();
                        if (ClientId == 0)
                        {
                            return;
                        }

                        foreach (var client in Playerstore.Instance.Clients.Values)
                        {
                            if (client.ClientId == ClientId)
                            {
                                if (client.udp.EndPoint == null)
                                    client.udp.Connect(clientEndPoint);
                                else
                                    client.udp.HandleData(store);
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mainserver.Error("Failed Handle Client Data: " + ex.Message, this);
            }
        }

        /// <summary>
        /// Send UDP Data to Player
        /// </summary>
        /// <param name="client"></param>
        /// <param name="store"></param>
        public void SendUDP(Client client, Store store)
        {
            try
            {
                if (!IsRunning)
                    Mainserver.Warning("UDP-Layer is not Running to send data.", this);
                if (client.udp.EndPoint != null)
                    Listener.BeginSend(store.ToArray, store.Length, client.udp.EndPoint, null, null);
            }
            catch (SocketException ex)
            {
                Mainserver.Error("Error sending Data: " + ex.Message, this);
            }
        }

        /// <summary>
        /// Send UDP to All Clients
        /// </summary>
        /// <param name="store"></param>
        public void SendAll(Store store)
        {
            for (int i = 0; i < Playerstore.Instance.Clients.Count; i++)
            {
                Playerstore.Instance.Clients[i].udp.SendData(store);
            }
        }

        /// <summary>
        /// Send UDP to All Clients ecxept this Client
        /// </summary>
        /// <param name="client"></param>
        /// <param name="store"></param>
        public void SendAll(Client client, Store store)
        {
            for (int i = 0; i < Playerstore.Instance.Clients.Count; i++)
            {
                if (Playerstore.Instance.Clients[i] != client)
                    Playerstore.Instance.Clients[i].udp.SendData(store);
            }
        }

        /// <summary>
        /// Send UDP data to This Client 
        /// </summary>
        /// <param name="ClientId"></param>
        /// <param name="store"></param>
        public void Send(int ClientId, Store store)
        {
            Playerstore.Instance.Clients[ClientId].udp.SendData(store);
        }

        /// <summary>
        /// Send UDP Data to all only not on this ClientId
        /// </summary>
        /// <param name="client"></param>
        /// <param name="store"></param>
        public void SendAll(int client, Store store)
        {
            for (int i = 0; i < Playerstore.Instance.Clients.Count; i++)
            {
                if (Playerstore.Instance.Clients[i].ClientId != client)
                    Playerstore.Instance.Clients[i].udp.SendData(store);
            }
        }
    }
}