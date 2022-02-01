using LowNet.Enums;
using LowNet.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Collections;
using System.Text;
using System.Net.NetworkInformation;

namespace LowNet.Unity3D
{
    class ClientNetworkmanager : MonoBehaviour
    {
        public static ClientNetworkmanager Instance { get; private set; }
        [Header("Server IPAdresse")]
        public string ServerIP = "127.0.0.1";
        [Header("Server Listenport")]
        public int ServerPort = 4900;
        [Header("Server Password")]
        public string ServerPassword = "";
        [Header("Network Update Rate")]
        public NetworkUpdate NetworkSpeed = NetworkUpdate.Update;
        [Header("Server Log Mode")]
        public LogMode ServerLogging = LogMode.LogNormal;
        public bool AutoConnect = false;
        [HideInInspector] public TCP tcp;
        [HideInInspector] public UDP udp;
        public long RTT;

        /// <summary>
        /// On Incomming Packet
        /// </summary>
        /// <param name="store"></param>
        public delegate void PacketHandler(Store store);
        /// <summary>
        /// Regestrierte Packets
        /// </summary>
        public static Dictionary<int, PacketHandler> Packets;

        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Instance = this;
        }

        void Start()
        {
            if (AutoConnect)
            {
                ConnectToServer();
                StartCoroutine(GetPing());
            }
        }

        private void FixedUpdate()
        {
            if (NetworkSpeed == NetworkUpdate.FixedUpdate)
                UpdateMain();
        }

        private void Update()
        {
            if (NetworkSpeed == NetworkUpdate.Update)
                UpdateMain();
        }

        #region Threadmanager
        private static readonly List<Action> executeOnMainThread = new List<Action>();
        private static readonly List<Action> executeCopiedOnMainThread = new List<Action>();
        private static bool actionToExecuteOnMainThread = false;

        public static void ExecuteOnMainThread(Action _action)
        {
            if (_action == null)
            {
                Console.WriteLine("No action to execute on main thread!");
                return;
            }

            lock (executeOnMainThread)
            {
                executeOnMainThread.Add(_action);
                actionToExecuteOnMainThread = true;
            }
        }

        public static void UpdateMain()
        {
            if (actionToExecuteOnMainThread)
            {
                executeCopiedOnMainThread.Clear();
                lock (executeOnMainThread)
                {
                    executeCopiedOnMainThread.AddRange(executeOnMainThread);
                    executeOnMainThread.Clear();
                    actionToExecuteOnMainThread = false;
                }

                for (int i = 0; i < executeCopiedOnMainThread.Count; i++)
                {
                    executeCopiedOnMainThread[i]();
                }
            }
        }
        #endregion

        #region Networking
        public const int dataBufferSize = 4096;
        [Header("My Player Connection Id")]
        public int ConnectionId = 0;
        private bool isConnected = false;

        public class TCP
        {
            public TcpClient socket;

            private NetworkStream stream;
            private Store receivedData;
            private byte[] receiveBuffer;

            public void Connect()
            {
                socket = new TcpClient
                {
                    ReceiveBufferSize = dataBufferSize,
                    SendBufferSize = dataBufferSize
                };

                receiveBuffer = new byte[dataBufferSize];
                socket.BeginConnect(Instance.ServerIP, Instance.ServerPort, ConnectCallback, socket);
            }

            private void ConnectCallback(IAsyncResult result)
            {
                socket.EndConnect(result);
                if (!socket.Connected)
                {
                    return;
                }

                stream = socket.GetStream();
                receivedData = new Store();
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }

            public void SendData(Store store)
            {
                try
                {
                    if (socket != null)
                    {
                        stream.BeginWrite(store.ToArray, 0, store.Length, null, null);
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
                        Instance.Disconnect();
                        return;
                    }

                    byte[] data = new byte[byteLength];
                    Array.Copy(receiveBuffer, data, byteLength);

                    receivedData.Reset(HandleData(data));
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                }
                catch
                {
                    Disconnect();
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
                    ExecuteOnMainThread(() =>
                    {
                        using (Store packet = new Store(packetBytes))
                        {
                            int packetId = packet.PopInt();
                            Packets[packetId](packet);
                        }
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

            private void Disconnect()
            {
                Instance.Disconnect();
                stream = null;
                receivedData = null;
                receiveBuffer = null;
                socket = null;
            }
        }

        public class UDP
        {
            public UdpClient socket;
            public IPEndPoint endPoint;

            public UDP()
            {
                endPoint = new IPEndPoint(IPAddress.Parse(Instance.ServerIP), Instance.ServerPort);
            }

            public void Connect(int _localPort)
            {
                socket = new UdpClient(_localPort);
                socket.Connect(endPoint);
                socket.BeginReceive(ReceiveCallback, null);

                using (Store store = new Store())
                {
                    SendData(store);
                }
            }

            public void SendData(Store store)
            {
                try
                {
                    store.InsertInt(Instance.ConnectionId);
                    if (socket != null)
                    {
                        socket.BeginSend(store.ToArray, store.Length, null, null);
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
                    byte[] data = socket.EndReceive(result, ref endPoint);
                    socket.BeginReceive(ReceiveCallback, null);

                    if (data.Length < 4)
                    {
                        Instance.Disconnect();
                        return;
                    }

                    HandleData(data);
                }
                catch
                {
                    Disconnect();
                }
            }

            private void HandleData(byte[] data)
            {
                using (Store store = new Store(data))
                {
                    int packetLength = store.PopInt();
                    data = store.PopBytes(packetLength);
                }

                ExecuteOnMainThread(() =>
                {
                    using (Store store = new Store(data))
                    {
                        int packetId = store.PopInt();
                        Packets[packetId](store);
                    }
                });
            }

            private void Disconnect()
            {
                Instance.Disconnect();

                endPoint = null;
                socket = null;
            }
        }

        private void Disconnect()
        {
            if (isConnected)
            {
                isConnected = false;
                tcp.socket.Close();
                udp.socket.Close();

                Debug.Log("Disconnected from server.");
            }
        }

        public void ConnectToServer()
        {
            tcp = new TCP();
            udp = new UDP();
            isConnected = true;
            tcp.Connect();
        }
        #endregion

        public IEnumerator GetPing()
        {
            yield return new WaitForSecondsRealtime(60);
            System.Net.NetworkInformation.Ping pingSender = new System.Net.NetworkInformation.Ping();
            PingOptions options = new PingOptions();
            options.DontFragment = true;

            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 120;
            PingReply reply = pingSender.Send(ServerIP, timeout, buffer, options);
            if (reply.Status == IPStatus.Success)
            {
                RTT = reply.RoundtripTime;
            }
            StartCoroutine(GetPing());
        }
    }
}