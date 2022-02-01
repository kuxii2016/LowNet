using LowNet.Enums;
using LowNet.Server.Events;
using LowNet.Server.Packets;
using LowNet.Server.Transport;
using LowNet.Utils;
using System;
using System.Collections.Generic;

namespace LowNet.Server
{
    /// <summary>
    /// LowNet V1 Server
    /// </summary>
    public class Server
    {
        /// <summary>
        /// Create Server Instance
        /// </summary>
        /// <param name="password">Server Password</param>
        /// <param name="servername">Server List Name</param>
        /// <param name="serverip">Server List IPAdresse</param>
        /// <param name="port">Server Listen Port</param>
        /// <param name="maxplayer">Max Amount of Player</param>
        public Server(string password, string servername, string serverip, int port, int maxplayer)
        {
            Instance = this;
            MaxPlayers = maxplayer;
            ServerIp = serverip;
            ServerPort = port;
            Servername = servername;
            Serverpassword = password;
        }

        /// <summary>
        /// Packet Handler will Fire on Incomming Packets
        /// </summary>
        /// <param name="client"></param>
        /// <param name="store"></param>
        public delegate void PacketHandler(Client client, Store store);
        /// <summary>
        /// Client Storage
        /// </summary>
        public static Dictionary<int, Client> Clients = new Dictionary<int, Client>();
        /// <summary>
        /// Server Regestrierte Packets
        /// </summary>
        public static Dictionary<int, PacketHandler> Packets;

        internal UDPLayer UDPService;
        internal TCPLayer TCPService;

        #region Public getValues
        /// <summary>
        /// Static Server Instance
        /// </summary>
        public static Server Instance { get; private set; }
        /// <summary>
        /// Server List IPAdresse
        /// </summary>
        public static string ServerIp { get; private set; } = "127.0.0.1";
        /// <summary>
        /// Server Listen Port
        /// </summary>
        public static int ServerPort { get; private set; } = 4900;
        /// <summary>
        /// Server List Name
        /// </summary>
        public static string Servername { get; private set; } = "LowNet-Server";
        /// <summary>
        /// Server Password
        /// </summary>
        public static string Serverpassword { get; private set; } = "";
        /// <summary>
        /// Max Amount of Player
        /// </summary>
        public static int MaxPlayers { get; private set; } = 50;
        /// <summary>
        /// Server Log Modus
        /// </summary>
        public static LogMode ServerLogging { get; private set; } = LogMode.LogNormal;
        #endregion

        #region Events
        /// <summary>
        /// Event Handler will Raise on new Player Connection
        /// </summary>
        public event EventHandler<ConnectedEventArgs> ConnectedEvent;
        /// <summary>
        /// Event Handler will Raise on Player Disconnect
        /// </summary>
        public event EventHandler<DisconnectedEventArgs> DisconnectedEvent;
        /// <summary>
        /// Event Handler will Raisei on new Server Log
        /// </summary>
        public event EventHandler<LogMessageEventArgs> LogMessageEvent;
        #endregion

        #region Server Data Sending
        /// <summary>Sends a packet to a client via TCP.</summary>
        /// <param name="client">The client to send the packet the packet to.</param>
        /// <param name="store">The packet to send to the client.</param>
        public static void SendTCPData(Client client, Store store)
        {
            store.WriteLength();
            Clients[client.Connectionid].Tcp.SendData(store);
        }

        /// <summary>Sends a packet to a client via UDP.</summary>
        /// <param name="client">The client to send the packet the packet to.</param>
        /// <param name="store">The packet to send to the client.</param>
        public static void SendUDPData(Client client, Store store)
        {
            store.WriteLength();
            Clients[client.Connectionid].Udp.Send(store);
        }

        /// <summary>Sends a packet to all clients via TCP.</summary>
        /// <param name="store">The packet to send.</param>
        public static void SendTCPDataToAll(Store store)
        {
            store.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Clients[i].Tcp.SendData(store);
            }
        }

        /// <summary>Sends a packet to all clients except one via TCP.</summary>
        /// <param name="excluded">The client to NOT send the data to.</param>
        /// <param name="store">The packet to send.</param>
        public static void SendTCPDataToAll(Client excluded, Store store)
        {
            store.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i != excluded.Connectionid)
                {
                    Clients[i].Udp.Send(store);
                }
            }
        }

        /// <summary>Sends a packet to all clients via UDP.</summary>
        /// <param name="store">The packet to send.</param>
        public static void SendUDPDataToAll(Store store)
        {
            store.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Clients[i].Udp.Send(store);
            }
        }

        /// <summary>Sends a packet to all clients except one via UDP.</summary>
        /// <param name="excluded">The client to NOT send the data to.</param>
        /// <param name="store">The packet to send.</param>
        public static void SendUDPDataToAll(Client excluded, Store store)
        {
            store.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i != excluded.Connectionid)
                {
                    Clients[i].Udp.Send(store);
                }
            }
        }
        #endregion

        /// <summary>
        /// Start Server, Is Extra non Static, Call it over Server
        /// </summary>
        public bool Startserver()
        {
            UDPService = new UDPLayer(ServerPort);
            TCPService = new TCPLayer(ServerPort);
            for (int i = 1; i <= MaxPlayers; i++)
            {
                Clients.Add(i, new Client(i));
            }
            Log($"LowNet Server Started, Bind to: {ServerIp}:{ServerPort} for: {MaxPlayers}", Instance);
            return true;
        }

        /// <summary>
        /// Stop Server and disconnect all clients
        /// </summary>
        public void Stopserver()
        {
            TCPLayer.Shutdown();
            UDPLayer.Shutdown();
            Log("Server Stopped.");
        }

        /// <summary>
        /// Set Serversettings
        /// </summary>
        /// <param name="log"></param>
        public static void SetSettings(LogMode log)
        {
            ServerLogging = log;
        }

        #region Log Events Invoke
        /// <summary>
        /// Prints Normal Logmessage
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="script"></param>
        public static void Log(string Message, object script = null)
        {
            string classInfo = ClassUtils.TryGetClass(script);
            if (Instance.LogMessageEvent != null && ServerLogging == LogMode.LogNormal || ServerLogging == LogMode.LogAll || ServerLogging == LogMode.LogDebug)
                Instance.LogMessageEvent?.Invoke(Instance, new LogMessageEventArgs { Type = LogType.LogNormal, Message = Message, ClassInfo = classInfo });
        }

        /// <summary>
        /// Prints Normal Logmessage
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="script"></param>
        public static void Warning(string Message, object script = null)
        {
            string classInfo = ClassUtils.TryGetClass(script);
            if (Instance.LogMessageEvent != null && ServerLogging == LogMode.LogWarning || ServerLogging == LogMode.LogNormal || ServerLogging == LogMode.LogAll || ServerLogging == LogMode.LogDebug)
                Instance.LogMessageEvent?.Invoke(Instance, new LogMessageEventArgs { Type = LogType.LogWarning, Message = Message, ClassInfo = classInfo });
        }

        /// <summary>
        /// Prints Normal Logmessage
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="ex"></param>
        /// <param name="script"></param>
        public static void Error(string Message, string ex, object script = null)
        {
            string classInfo = ClassUtils.TryGetClass(script);
            if (Instance.LogMessageEvent != null && ServerLogging == LogMode.LogError || ServerLogging == LogMode.LogNormal || ServerLogging == LogMode.LogAll || ServerLogging == LogMode.LogDebug)
                Instance.LogMessageEvent?.Invoke(Instance, new LogMessageEventArgs { Type = LogType.LogWarning, Message = Message, ClassInfo = classInfo, Exception = ex });
        }

        /// <summary>
        /// Prints Normal Logmessage
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="script"></param>
        public static void Debug(string Message, object script = null)
        {
            string classInfo = ClassUtils.TryGetClass(script);
            if (Instance.LogMessageEvent != null && ServerLogging == LogMode.LogAll || ServerLogging == LogMode.LogDebug)
                Instance.LogMessageEvent?.Invoke(Instance, new LogMessageEventArgs { Type = LogType.LogDebug, Message = Message, ClassInfo = classInfo });
        }
        #endregion

        #region Player Events Invoke
        /// <summary>
        /// Fired on new Player Connection
        /// </summary>
        /// <param name="client"></param>
        public static void OnPlayerconnect(Client client)
        {
            Instance.ConnectedEvent?.Invoke(Instance, new ConnectedEventArgs { Client = client, Message = "Joint the Game." });
        }

        /// <summary>
        /// Fired on Player Disconnect
        /// </summary>
        /// <param name="client"></param>
        public static void OnPlayerDisconnect(Client client)
        {
            Instance.DisconnectedEvent?.Invoke(Instance, new DisconnectedEventArgs { Client = client, Message = "Left the Game." });
        }
        #endregion

        /// <summary>
        /// System Packet handler
        /// </summary>
        public static void InitPackets()
        {
            Dictionary<int, PacketHandler> packets = new Dictionary<int, PacketHandler>()
            {
                {(int)Packet.LOWNET_CONNECT, LOWNET_CONNECT.Readpacket }
            };
            Packets = packets;
        }
    }
}