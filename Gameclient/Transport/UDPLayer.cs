/*  __                  _   __     __ 
   / /   ____ _      __/ | / /__  / /_
  / /   / __ \ | /| / /  |/ / _ \/ __/
 / /___/ /_/ / |/ |/ / /|  /  __/ /_  
/_____/\____/|__/|__/_/ |_/\___/\__/  
Simple Unity3D Solution ©2022 by Kuxii
*/
using LowNet.Data;
using LowNet.Unity3D;
using System;
using System.Net;
using System.Net.Sockets;

namespace LowNet.Gameclient.Transport
{
    internal class UDPLayer
    {
        public UDPLayer(GameClient client)
        {
            this.client = client;
        }

        private GameClient client;
        private UdpClient socket;
        private IPEndPoint endPoint;

        public void SetServer(string IPAdress, int Port)
        {
            endPoint = new IPEndPoint(IPAddress.Parse(IPAdress), Port);
        }

        internal void Connect(int localPort = 0)
        {
            int port = ((IPEndPoint)client.TCP.socket.Client.LocalEndPoint).Port;
            if (localPort != 0)
                port = localPort;

            socket = new UdpClient(port);
            socket.Client.SendTimeout = client.Timeout;
            socket.Client.ReceiveTimeout = client.Timeout;
            socket.Connect(endPoint);
            socket.BeginReceive(ReceiveCallback, null);
            Store store = new Store();
            SendData(store);
        }

        internal void SendData(Store store)
        {
            try
            {
                if (socket != null)
                {
                    store.WriteLength();
                    store.InsertInt(client.GetConnectionId);
                    socket.BeginSend(store.ToArray, store.Length, null, null);
                    store.Dispose();
                }
            }
            catch (Exception ex)
            {
                client.Error("Error on sending data to Server. " + ex.Message, this);
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                byte[] data = socket.EndReceive(result, ref endPoint);
                socket.BeginReceive(ReceiveCallback, null);

                if (data.Length < 4)
                {
                    client.Disconnect();
                    return;
                }

                HandleData(data);
            }
            catch
            {
                client.Disconnect();
            }
        }

        private void HandleData(byte[] data)
        {
            using (Store store = new Store(data))
            {
                int packetLength = store.PopInt();
                data = store.PopBytes(packetLength);
            }

            ThreadManager.ExecuteOnMainThread(() =>
            {
                Store store = new Store(data);
                int packetId = store.PopInt();
                GameClient.ClientPackets[packetId](client, store);
            });
        }

        internal void Disconnect()
        {
            try
            {
                socket.Close();
            }
            catch { }
            endPoint = null;
            socket = null;
        }

        /// <summary>
        /// Stop Discovery Layer
        /// </summary>
        public void Shutdown()
        {
            try
            {
                socket.Close();
                client.Log("Stopped Discovery Service", this);
            }
            catch { }
        }
    }
}