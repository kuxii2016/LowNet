using LowNet.ClientPackets;
using LowNet.Enums;
using LowNet.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using static LowNet.Unity3D.ClientNetworkmanager;

namespace LowNet.Unity3D
{
    /// <summary>
    /// Unity3D Client Network Manager
    /// </summary>
    public class ClientNetworkmanager : MonoBehaviour
    {
        /// <summary>
        /// Player Holder
        /// </summary>
        public static Dictionary<int, NetworkPlayer> Player = new Dictionary<int, NetworkPlayer>();

        #region Public Propertys
        /// <summary>
        /// Networkmanager Instance
        /// </summary>
        public static ClientNetworkmanager Instance { get; private set; }
        /// <summary>
        /// IPAdresse who the Client Connect
        /// </summary>
        [Header("Server IPAdresse")]
        public string ServerIP = "127.0.0.1";
        /// <summary>
        /// Server Port
        /// </summary>
        [Header("Server Listenport")]
        public int ServerPort = 4900;
        /// <summary>
        /// Server Password need for Connection
        /// </summary>
        [Header("Server Password")]
        public string ServerPassword = "";
        /// <summary>
        /// Network Worker Update rate
        /// </summary>
        [Header("Network Update Rate")]
        public NetworkUpdate NetworkSpeed = NetworkUpdate.Update;
        /// <summary>
        /// Client Logging Mode
        /// </summary>
        [Header("Server Log Mode")]
        public LogMode ServerLogging = LogMode.LogNormal;
        /// <summary>
        /// Auto Connect on Start
        /// </summary>
        [Header("Auto Connect to Server on Start")]
        public bool AutoConnect = false;
        /// <summary>
        /// Playername from this Player
        /// </summary>
        [Header("Client Playername")]
        public string Playername = "LowNetplayer";
        /// <summary>
        /// Player Spawn Models
        /// </summary>
        [Header("All Playermodels"), Tooltip("Min 1 is Needet")]
        public List<NetworkPlayer> PlayerModels;
        #endregion

        #region Private Propertys
        /// <summary>
        /// Client TCP-Layer
        /// </summary>
        internal TCP tcp;
        /// <summary>
        /// Client UDP-Layer
        /// </summary>
        internal UDP udp;
        /// <summary>
        /// Round Trip Time to Server
        /// </summary>
        public long RTT;
        #endregion

        #region Packets
        /// <summary>
        /// On Incomming Packet
        /// </summary>
        /// <param name="store"></param>
        public delegate void PacketHandler(Store store);
        /// <summary>
        /// Regestrierte Packets
        /// </summary>
        public static Dictionary<int, PacketHandler> Packets;
        #endregion Packets

        #region Unity3d Events
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Instance = this;
        }

        private void Start()
        {
            InitPackets();
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
        #endregion Unity3d Events

        #region Threadmanager
        private static readonly List<Action> executeOnMainThread = new List<Action>();
        private static readonly List<Action> executeCopiedOnMainThread = new List<Action>();
        private static bool actionToExecuteOnMainThread = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        public static void ExecuteOnMainThread(Action action)
        {
            if (action == null)
                return;

            lock (executeOnMainThread)
            {
                executeOnMainThread.Add(action);
                actionToExecuteOnMainThread = true;
            }
        }
        /// <summary>
        /// 
        /// </summary>
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
        #endregion Threadmanager

        #region Networking
        /// <summary>
        /// Packet buffer
        /// </summary>
        private const int dataBufferSize = 4096;
        /// <summary>
        /// Client ConnectionId
        /// </summary>
        [Header("My Player Connection Id")]
        public int ConnectionId = 0;
        /// <summary>
        /// Is Player Connected 
        /// </summary>
        internal bool isConnected = false;

        internal class TCP
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
                    store.WriteLength();
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

        internal class UDP
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
                    if (Client.IsConnected)
                    {
                        store.InsertInt(Instance.ConnectionId);
                        store.WriteLength();
                        if (socket != null)
                        {
                            socket.BeginSend(store.ToArray, store.Length, null, null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Client.Log("Failed Send data over UDP: " + ex.Message, Enums.LogType.LogError);
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
                catch (Exception ex)
                {
                    Client.Log("Failed Read data over UDP: " + ex.Message, Enums.LogType.LogError);
                    Disconnect();
                }
            }

            private void HandleData(byte[] data)
            {
                if (Client.IsConnected)
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
            }

            private void Disconnect()
            {
                Instance.Disconnect();
                endPoint = null;
                socket = null;
            }
        }

        /// <summary>
        /// Disconnect Client from Server
        /// </summary>
        public void Disconnect()
        {
            if (isConnected)
            {
                isConnected = false;
                tcp.socket.Close();
                if (udp.socket != null)
                    udp.socket.Close();

                Debug.Log("Disconnected from server.");
            }
        }

        /// <summary>
        /// Connect Client to Server
        /// </summary>
        public void ConnectToServer()
        {
            tcp = new TCP();
            udp = new UDP();
            isConnected = true;
            tcp.Connect();
        }

        /// <summary>
        /// Get Client RTT Time
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetPing()
        {
            if (isConnected)
            {
                yield return new WaitForSecondsRealtime(60);
                System.Net.NetworkInformation.Ping pingSender = new System.Net.NetworkInformation.Ping();
                PingOptions options = new PingOptions { DontFragment = true };
                string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                int timeout = 120;
                PingReply reply = pingSender.Send(ServerIP, timeout, buffer, options);
                if (reply.Status == IPStatus.Success) { RTT = reply.RoundtripTime; }
                StartCoroutine(GetPing());
            }
        }
        #endregion Networking

        #region Player name Change
        /// <summary>
        /// Set Playername
        /// </summary>
        /// <param name="Name"></param>
        public void SetPlayername(string Name) => Playername = Name;

        /// <summary>
        /// Static version to Update Playername
        /// </summary>
        /// <param name="name"></param>
        public static void Updateplayername(string name) => Instance.SetPlayername(name);
        #endregion Player name Change

        #region Player Spawn Despawn
        /// <summary>
        /// Despawn Player
        /// </summary>
        /// <param name="PlayerId"></param>
        public static void DeSpawnPlayer(int PlayerId)
        {
            GameObject player = Player[PlayerId].gameObject;
            Destroy(player);
            Player.Remove(PlayerId);
        }

        /// <summary>
        /// Spawn Player
        /// </summary>
        /// <param name="ModelId"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <param name="playername"></param>
        /// <param name="isMyPlayer"></param>
        /// <param name="PlayerId"></param>
        public static void SpawnPlayer(int ModelId, Vector3 pos, Quaternion rot, string playername, bool isMyPlayer = false, int PlayerId = -1)
        {
            NetworkPlayer player = Instantiate(Instance.PlayerModels[ModelId]);
            player.transform.position = pos;
            player.transform.rotation = rot;
            player.IsMyView = isMyPlayer;
            player.PlayerId = PlayerId;
            player.PlayerName = playername;
            Player.Add(PlayerId, player);
            player.gameObject.name = "CL: " + player.PlayerId + " | " + player.PlayerName;
        }
        #endregion Player Spawn Despawn

        /// <summary>
        /// Init Client Packets
        /// </summary>
        public static void InitPackets()
        {
            Dictionary<int, PacketHandler> packets = new Dictionary<int, PacketHandler>()
            {
                {(int)Packet.LOWNET_CONNECT, LOWNET_CONNECT.Readpacket },
                {(int)Packet.LOWNET_HANDSHAKE, LOWNET_HANDSHAKE.Readpacket },
                {(int)Packet.LOWNET_PLAYER, LOWNET_PLAYER.Readpacket },
                {(int)Packet.LOWNET_CONNECT_UDP, LOWNET_CONNECT_UDP.Readpacket },
                {(int)Packet.LOWNET_PLAYER_SYNC, LOWNET_PLAYER_SYNC.Readpacket }
            };
            Packets = packets;
        }
    }

    /// <summary>
    /// Smal Helper to Write smaler Classname
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Send Data to Server over UDP-Layer
        /// </summary>
        public static void SendUDP(Store store) => ClientNetworkmanager.Instance.udp.SendData(store);

        /// <summary>
        /// Send Data to Server over TCP-Layer
        /// </summary>
        public static void SendTCP(Store store) => ClientNetworkmanager.Instance.tcp.SendData(store);

        /// <summary>
        /// Disconnect client from Server
        /// </summary>
        public static void Disconnect() => ClientNetworkmanager.Instance.Disconnect();

        /// <summary>
        /// Get PlayerId
        /// </summary>
        /// <returns></returns>
        public static int GetPlayerId
        { get { return ClientNetworkmanager.Instance.ConnectionId; } }

        /// <summary>
        /// Get Round Trip Time
        /// </summary>
        /// <returns></returns>
        public static long GetRTT
        { get { return ClientNetworkmanager.Instance.RTT; } }

        /// <summary>
        /// Get Playername
        /// </summary>
        /// <returns></returns>
        public static string GetPlayername
        { get { return ClientNetworkmanager.Instance.Playername; } }

        /// <summary>
        /// Set Serverpasswort to Client
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string SetServerPassword(string password) => ClientNetworkmanager.Instance.ServerPassword = password;

        /// <summary>
        /// Set Playername to Client
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string SetPlayername(string name) => ClientNetworkmanager.Instance.Playername = name;

        /// <summary>
        /// Is Client Connected or not
        /// </summary>
        public static bool IsConnected => ClientNetworkmanager.Instance.isConnected;

        /// <summary>
        /// Get Network Instance
        /// </summary>
        public static ClientNetworkmanager GetInstance
        { get { return ClientNetworkmanager.Instance; } }

        /// <summary>
        /// Trigger Client Log Message
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="logType"></param>
        public static void Log(string Message, Enums.LogType logType)
        {
            string now = DateTime.Now.Millisecond.ToString("0.00");
            switch (logType)
            {
                case Enums.LogType.LogDebug:
                    Debug.Log(string.Format($"<color=#c5ff00>[{now}][LowNet-Client]</color><color=#0083ff>[DEBUG]</color><color=#818181>{Message}</color>"));
                    break;

                case Enums.LogType.LogNormal:
                    Debug.Log(string.Format($"<color=#c5ff00>[{now}][LowNet-Client]</color><color=#00ff23>[LOG]</color><color=#818181>{Message}</color>"));
                    break;

                case Enums.LogType.LogWarning:
                    Debug.LogWarning(string.Format($"<color=#c5ff00>[{now}][LowNet-Client]</color><color=#ffa200>[WARNING]</color><color=#818181>{Message}</color>"));
                    break;

                case Enums.LogType.LogError:
                    Debug.LogError(string.Format($"<color=#c5ff00>[{now}][LowNet-Client]</color><color=#ff0000>[ERROR]</color><color=#818181>{Message}</color>"));
                    break;
            }
        }

        /// <summary>
        /// Overwrite exist Packet
        /// </summary>
        /// <param name="packetId"></param>
        /// <param name="packet"></param>
        public static void OverwritePacket(int packetId, PacketHandler packet)
        {
            if (ClientNetworkmanager.Packets[packetId] != null)
                ClientNetworkmanager.Packets.Remove(packetId);
            ClientNetworkmanager.Packets.Add(packetId, packet);
        }

        /// <summary>
        /// Add new Packets
        /// </summary>
        /// <param name="packets"></param>
        public static void AddPackets(Dictionary<int, PacketHandler> packets)
        {
            int Old = ClientNetworkmanager.Packets.Count + 1;
            for (int i = 0; i < packets.Count; i++)
            {
                ClientNetworkmanager.Packets.Add((i + Old), packets[i]);
            }
        }
    }
}