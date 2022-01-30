/*  __                  _   __     __ 
   / /   ____ _      __/ | / /__  / /_
  / /   / __ \ | /| / /  |/ / _ \/ __/
 / /___/ /_/ / |/ |/ / /|  /  __/ /_  
/_____/\____/|__/|__/_/ |_/\___/\__/  
Simple Unity3D Solution ©2022 by Kuxii
*/
using LowNet.Data;
using LowNet.Events;
using LowNet.Server.Data;
using LowNet.ServerPackets;
using LowNet.Serverstore;
using LowNet.Transport;
using LowNet.Utils;
using System;
using System.Collections.Generic;

namespace LowNet.Server
{
    /// <summary>
    /// LowNet Server (Easy Unity3d Networking System)
    /// </summary>
    public class LowNetServer
    {
        #region Server Init
        internal static LowNetServer server;

        /// <summary>
        /// Create new Server Instance
        /// </summary>
        /// <param name="servername">Server Name from Server for Serverlist or Whatever</param>
        /// <param name="serverpassword">Server Passwort to block Public Player was have not the Password</param>
        /// <param name="serverport">Server Listen port</param>
        /// <param name="maxplayer">Max Amount of Player was can hold the Server</param>
        /// <param name="isUnity">Is a Unity3d Server or not, Important for Packet Sending/Reading</param>
        /// <param name="logsettings">Logmode Settings</param>
        public LowNetServer(string servername, string serverpassword, int serverport, int maxplayer, bool isUnity = true, Logsettings logsettings = Logsettings.Logging_Normal)
        {
            Logging = logsettings;
            this.Servername = servername;
            this.Serverpassword = serverpassword;
            this.Serverport = serverport;
            this.Maxplayer = maxplayer;
            this.IsUnityServer = isUnity;
            this.UDPLayer = new UDPLayer(Serverport, this);
            this.TCPLayer = new TCPLayer(Serverport, this);
            this.DiscoveryLayer = new DiscoveryLayer(Serverport + 1, this);
        }
        #endregion

        #region Private Propertys
        private readonly Logsettings Logging;
        /// <summary>
        /// Playerstore Instance
        /// </summary>
        public Playerstore Player;
        private readonly bool IsUnityServer = true;
        private readonly string Servername, Serverpassword;
        private readonly int Serverport, Maxplayer;
        #endregion

        #region Events
        /// <summary>
        /// Event Handler will Raise on new Player Connection
        /// </summary>
        public event EventHandler<ClientConnectedEventArgs> ClientConnected;
        /// <summary>
        /// Event Handler will Raise on Player Disconnect
        /// </summary>
        public event EventHandler<ClientDisconnectedEventArgs> ClientDisconnected;
        /// <summary>
        /// Event Handler will Raisei on new Server Log
        /// </summary>
        public event EventHandler<ServerlogMessage> OnServerlog;
        #endregion

        #region Public Propertys
        /// <summary>
        /// Get Server Listen Port
        /// </summary>
        public int GetServerport { get { return Serverport; } }
        /// <summary>
        /// Get Maximal Player Slots
        /// </summary>
        public int GetMaxplayer { get { return Maxplayer; } }
        /// <summary>
        /// Get Current Connected Player
        /// </summary>
        public int GetPlayer { get { return Player.GetPlayer(); } }
        /// <summary>
        /// Get Servername
        /// </summary>
        public string GetServername { get { return Servername; } }
        /// <summary>
        /// Get Serverpassword
        /// </summary>
        public string GetServerpassword { get { return Serverpassword; } }
        #endregion

        /// <summary>
        /// Call Packet Reader
        /// </summary>
        /// <param name="client">Sender Client</param>
        /// <param name="store">Sender Store</param>
        public delegate void PacketHandler(Client client, Store store);
        /// <summary>
        /// Do not Override the Handler, Or Lownet Components will not work without Create same Packets!
        /// </summary>
        public static Dictionary<int, PacketHandler> Serverpackets;

        /// <summary>
        /// Get UDP Transport Layer
        /// </summary>
        public UDPLayer UDPLayer { get; private set; }
        /// <summary>
        /// Get TCP Transport Layer
        /// </summary>
        public TCPLayer TCPLayer { get; private set; }
        /// <summary>
        /// Get Discovery Transport Layer
        /// </summary>
        public DiscoveryLayer DiscoveryLayer { get; private set; }

        /// <summary>
        /// Start Networking Server
        /// </summary>
        public void Start()
        {
            server = this;
            Serverpackets = new Dictionary<int, PacketHandler>();
            LowNetServerPackethander.InitPackets();
            Player = new Playerstore(this);
            DiscoveryLayer.Start();
            TCPLayer.Start();
            UDPLayer.Start();
            if (!IsUnityServer)
            {
                //TODO: Create Packet Send/Read Thread
            }
        }

        /// <summary>
        /// Stop Networking Server
        /// </summary>
        public void Stop()
        {
            DiscoveryLayer.Shutdown();
            UDPLayer.Shutdown();
            TCPLayer.Shutdown();
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

            if (OnServerlog != null && Logging == Logsettings.Logging_Normal || Logging == Logsettings.Logging_Normal || Logging == Logsettings.Logging_Debug)
                OnServerlog?.Invoke(this, new ServerlogMessage { LogType = Logmessage.Log, LogMessage = infos + "()=>" + Message, TimeStamp = DateTime.Now, });
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

            if (OnServerlog != null && Logging == Logsettings.Logging_Normal || Logging == Logsettings.Logging_Error || Logging == Logsettings.Logging_Debug)
                OnServerlog?.Invoke(this, new ServerlogMessage { LogType = Logmessage.Error, LogMessage = infos + "()=>" + Message, TimeStamp = DateTime.Now, });
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

            if (OnServerlog != null && Logging == Logsettings.Logging_Normal || Logging == Logsettings.Logging_Warning || Logging == Logsettings.Logging_Debug)
                OnServerlog?.Invoke(this, new ServerlogMessage { LogType = Logmessage.Warning, LogMessage = infos + "()=>" + Message, TimeStamp = DateTime.Now, });
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

            if (OnServerlog != null && Logging == Logsettings.Logging_Debug)
                OnServerlog?.Invoke(this, new ServerlogMessage { LogType = Logmessage.Debug, LogMessage = infos + "()=>" + Message, TimeStamp = DateTime.Now, });
        }

        /// <summary>
        /// Trigger Connect Event
        /// </summary>
        /// <param name="client"></param>
        public void InvokePlayerconnect(Client client)
        {
            ClientConnected?.Invoke(this, new ClientConnectedEventArgs { client = client, session = client.Session, Connected = DateTime.Now, });
        }

        /// <summary>
        /// Trigger Disconnect Event
        /// </summary>
        /// <param name="client"></param>
        public void InvokePlayerDisconnect(Client client)
        {
            ClientDisconnected?.Invoke(this, new ClientDisconnectedEventArgs { client = client, session = client.Session, Connected = DateTime.Now, });
        }
        #endregion
    }
}