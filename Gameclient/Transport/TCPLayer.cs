/*  __                  _   __     __ 
   / /   ____ _      __/ | / /__  / /_
  / /   / __ \ | /| / /  |/ / _ \/ __/
 / /___/ /_/ / |/ |/ / /|  /  __/ /_  
/_____/\____/|__/|__/_/ |_/\___/\__/  
Simple Unity3D Solution ©2020 by Kuxii
*/
using LowNet.Data;
using LowNet.Unity3D;
using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace LowNet.Gameclient.Transport
{
    /// <summary>
    /// Game Client TCP Layer
    /// </summary>
    internal class TCPLayer
    {
        /// <summary>
        /// Create new TCP Layer instance
        /// </summary>
        /// <param name="client"></param>
        public TCPLayer(GameClient client)
        {
            this.client = client;
        }

        private string IpAdress;
        private int port;
        private GameClient client;
        private static readonly int dataBufferSize = 4096;
        internal TcpClient socket;
        private NetworkStream stream;
        private Store receivedData;
        private byte[] receiveBuffer;

        internal void SetServer(string IPAdress, int Port)
        {
            IpAdress = IPAdress;
            port = Port;
        }

        internal void Connect()
        {
            socket = new TcpClient { ReceiveBufferSize = dataBufferSize, SendBufferSize = dataBufferSize };
            socket.ReceiveTimeout = client.Timeout;
            socket.SendTimeout = client.Timeout;

            //2mins KeepAlaive
            int size = Marshal.SizeOf((uint)0);
            byte[] keepAlive = new byte[size * 3];
            Buffer.BlockCopy(BitConverter.GetBytes((uint)1), 0, keepAlive, 0, size);
            Buffer.BlockCopy(BitConverter.GetBytes((uint)120000), 0, keepAlive, size, size);
            Buffer.BlockCopy(BitConverter.GetBytes((uint)120000), 0, keepAlive, size * 2, size);
            socket.Client.IOControl(IOControlCode.KeepAliveValues, keepAlive, null);
            socket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            receiveBuffer = new byte[dataBufferSize];
            socket.BeginConnect(IpAdress, port, ConnectCallback, socket);
        }

        private void ConnectCallback(IAsyncResult result)
        {
            try
            {
                socket.EndConnect(result);
            }
            catch (SocketException)
            {
                client.Error("Failed Connect to Server", this);
            }
            finally
            {
                client.isConnected = true;
                stream = socket.GetStream();
                receivedData = new Store();
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                client.Log("Successfully connected to server", this);
            }
        }

        internal void SendData(Store packet)
        {
            try
            {
                if (socket != null)
                {
                    packet.WriteLength();
                    stream.BeginWrite(packet.ToArray, 0, packet.Length, null, null);
                    packet.Dispose();
                }

            }
            catch (Exception ex)
            {
                client.Error("Failed send Data to Server. " + ex.Message, this);
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                int byteLength = stream.EndRead(result);
                if (byteLength <= 0)
                {
                    client.Error("Connection Lost.. Feels Bad :(", this);
                    client.Disconnect();
                    return;
                }

                byte[] data = new byte[byteLength];
                Array.Copy(receiveBuffer, data, byteLength);

                receivedData.Reset(HandleData(data));
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch
            {
                client.Disconnect();
            }
        }

        private bool HandleData(byte[] data)
        {
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
                    GameClient.ClientPackets[packetId](client, store);
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

        internal void Disconnect()
        {
            socket.Close();
            stream = null;
            receivedData = null;
            receiveBuffer = null;
            socket = null;
        }
    }
}