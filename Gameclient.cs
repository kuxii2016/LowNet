/*  __                  _   __     __ 
   / /   ____ _      __/ | / /__  / /_
  / /   / __ \ | /| / /  |/ / _ \/ __/
 / /___/ /_/ / |/ |/ / /|  /  __/ /_  
/_____/\____/|__/|__/_/ |_/\___/\__/  
Simple Unity3D Solution ©2022 by Kuxii
*/
using LowNet.Data;
using LowNet.Events;
using LowNet.Gameclient.Transport;
using LowNet.Utils;
using System;
using System.Collections.Generic;

namespace LowNet.Gameclient
{
    /// <summary>
    /// Player Client
    /// </summary>
    public class GameClient
    {
        /// <summary>
        /// Create new Player Client
        /// </summary>
        /// <param name="log"></param>
        public GameClient(Logsettings log = Logsettings.Logging_Normal)
        {
            Instance = this;
            Logging = log;
        }

        /// <summary>
        /// Event Handler will Raisei on new Server Log
        /// </summary>
        public event EventHandler<ServerlogMessage> OnClientog;
        /// <summary>
        /// Gameclient Instance
        /// </summary>
        public static GameClient Instance { get; private set; }
        /// <summary>
        /// Client UDP Layer
        /// </summary>
        internal UDPLayer UDP;
        /// <summary>
        /// Client TCP Layer
        /// </summary>
        internal TCPLayer TCP;
        /// <summary>
        /// Client Discoverlayer
        /// </summary>
        internal DiscoveryLayer discoveryLayer;
        /// <summary>
        /// Call Packet Reader
        /// </summary>
        /// <param name="client">Sender Client</param>
        /// <param name="store">Sender Store</param>
        public delegate void PacketHandler(GameClient client, Store store);
        /// <summary>
        /// Do not Override the Handler, Or Lownet Components will not work without Create same Packets!
        /// </summary>
        public static Dictionary<int, PacketHandler> ClientPackets;
        /// <summary>
        /// Client Log Settings
        /// </summary>
        private readonly Logsettings Logging;
        /// <summary>
        /// Internal ClientId
        /// </summary>
        private static int PlayerId = -1;
        /// <summary>
        /// Is Client Connected or Not
        /// </summary>
        internal bool isConnected = false;
        /// <summary>
        /// Time Out 
        /// </summary>
        internal int Timeout = 800;
        /// <summary>
        /// Get Client Connection/Player Id
        /// </summary>
        public int GetConnectionId { get { return PlayerId; } }
        /// <summary>
        /// Server Password
        /// </summary>
        public string Serverpassword { get; set; }
        /// <summary>
        /// TempUUID renew on new Connection
        /// </summary>
        public string TempUUID { get; set; }

        /// <summary>
        /// Disconnect from Server
        /// </summary>
        public void Disconnect()
        {
            isConnected = false;
            UDP.Disconnect();
            TCP.Disconnect();
            UDP = null;
            TCP = null;
        }

        /// <summary>
        /// Connect to Server
        /// </summary>
        /// <param name="ServerIp"></param>
        /// <param name="Serverport"></param>
        /// <param name="serverpasswd"></param>
        /// <param name="clienttimeout"></param>
        public void Connect(string ServerIp, int Serverport, string serverpasswd, int clienttimeout)
        {
            Serverpassword = serverpasswd;
            Timeout = clienttimeout;
            UDP = new UDPLayer(this);
            TCP = new TCPLayer(this);
            UDP.SetServer(ServerIp, Serverport);
            TCP.SetServer(ServerIp, Serverport);
            TCP.Connect();
        }

        /// <summary>
        /// Connect UDP Layer
        /// </summary>
        public void ConnectUDP()
        {
            UDP.Connect();
        }

        /// <summary>
        /// Send Data to Server via TCP
        /// </summary>
        /// <param name="store"></param>
        public void SendTCP(Store store)
        {
            if (TCP != null)
                TCP.SendData(store);
        }

        /// <summary>
        /// Send Data to Server via UDP
        /// </summary>
        /// <param name="store"></param>
        public void SendUDP(Store store)
        {
            if (UDP != null)
                UDP.SendData(store);
        }

        /// <summary>
        /// Overwrite Current ConnectionId
        /// </summary>
        /// <param name="value"></param>
        public void SetplayerId(int value) => PlayerId = value;

        /// <summary>
        /// Stop Listner on Application Close
        /// </summary>
        public void StopListner()
        {
            if (discoveryLayer != null)
                discoveryLayer.Shutdown();
            if (UDP != null)
                UDP.Shutdown();
        }

        #region Logsender
        /// <summary>
        /// Print Logmessage
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="scriptclass"></param>
        public void Log(string Message, object scriptclass = null)
        {
            string infos = "";
            if (scriptclass != null)
                infos = ClassUtils.TryGetClass(scriptclass);

            if (OnClientog != null && Logging == Logsettings.Logging_Normal || Logging == Logsettings.Logging_Normal || Logging == Logsettings.Logging_Debug)
                OnClientog?.Invoke(this, new ServerlogMessage { LogType = Logmessage.Log, LogMessage = infos + "()=>" + Message, TimeStamp = DateTime.Now, });
        }

        /// <summary>
        /// Print Errormessage
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="scriptclass"></param>
        public void Error(string Message, object scriptclass = null)
        {
            string infos = "";
            if (scriptclass != null)
                infos = ClassUtils.TryGetClass(scriptclass);

            if (OnClientog != null && Logging == Logsettings.Logging_Normal || Logging == Logsettings.Logging_Error || Logging == Logsettings.Logging_Debug)
                OnClientog?.Invoke(this, new ServerlogMessage { LogType = Logmessage.Error, LogMessage = infos + "()=>" + Message, TimeStamp = DateTime.Now, });
        }

        /// <summary>
        /// Print Warning Message
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="scriptclass"></param>
        public void Warning(string Message, object scriptclass = null)
        {
            string infos = "";
            if (scriptclass != null)
                infos = ClassUtils.TryGetClass(scriptclass);

            if (OnClientog != null && Logging == Logsettings.Logging_Normal || Logging == Logsettings.Logging_Warning || Logging == Logsettings.Logging_Debug)
                OnClientog?.Invoke(this, new ServerlogMessage { LogType = Logmessage.Warning, LogMessage = infos + "()=>" + Message, TimeStamp = DateTime.Now, });
        }

        /// <summary>
        /// Print Debug Message
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="scriptclass"></param>
        public void Debug(string Message, object scriptclass = null)
        {
            string infos = "";
            if (scriptclass != null)
                infos = ClassUtils.TryGetClass(scriptclass);

            if (OnClientog != null && Logging == Logsettings.Logging_Debug)
                OnClientog?.Invoke(this, new ServerlogMessage { LogType = Logmessage.Debug, LogMessage = infos + "()=>" + Message, TimeStamp = DateTime.Now, });
        }
        #endregion
    }
}