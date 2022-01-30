/*  __                  _   __     __ 
   / /   ____ _      __/ | / /__  / /_
  / /   / __ \ | /| / /  |/ / _ \/ __/
 / /___/ /_/ / |/ |/ / /|  /  __/ /_  
/_____/\____/|__/|__/_/ |_/\___/\__/  
Simple Unity3D Solution ©2022 by Kuxii
*/
using LowNet.ClientPackets;
using LowNet.Data;
using LowNet.Events;
using LowNet.Gameclient;
using LowNet.Gameclient.Transport;
using System.Collections.Generic;
using UnityEngine;

namespace LowNet.Unity3D
{
    /// <summary>
    /// Unity3d Client Network Managercomponent
    /// </summary>
    public class ClientNetworkmanager : MonoBehaviour
    {
        /// <summary>
        /// Networkplayer Holder
        /// </summary>
        public static Dictionary<int, NetworkPlayer> Player = new Dictionary<int, NetworkPlayer>();

        /// <summary>
        /// Client Instance
        /// </summary>
        public static ClientNetworkmanager Networkmanager;
        private GameClient Gameclient;
        /// <summary>
        /// Server IP to Connect
        /// </summary>
        [Header("Server IpAdress to Connect"), Tooltip("For Local you can use 'localhost'")]
        public string IPAdresse = "localhost";
        /// <summary>
        /// Server Port
        /// </summary>
        [Header("Server Listen Port")]
        public int Serverport = 4900;
        /// <summary>
        /// Server Password if not empty when Need to Connect
        /// </summary>
        [Header("Server Password"), Tooltip("Set this before you Connect to Server")]
        public string Serverpassword;
        /// <summary>
        /// Add Client Timeout
        /// </summary>
        [Range(0, 8500), Header("Send/Receive Timeout")]
        public int Timeout = 800;
        /// <summary>
        /// Client ConnectionId
        /// </summary>
        public int ConnectionId = -1;
        /// <summary>
        /// Autoconnect to Server on Start
        /// </summary>
        [Header("Autoconnect to Server"), Tooltip("When you have 2 Different Projects and No Masterserver List so you can Use this to Connect Client on Start")]
        public bool Autoconnect = false;
        /// <summary>
        /// Only Readable if Player Connected to Server
        /// </summary>
        [Header("Indicator is Client Connected")]
        public bool IsConnected = false;
        /// <summary>
        /// Editor Quick Connector
        /// </summary>
        [Header("Connect to Server if True"), Tooltip("Can use in the Editor for an Quick Connect")]
        public bool Connect = false;
        /// <summary>
        /// Define Client logging Mode
        /// </summary>
        [Header("Client Logging Mode")]
        public Logsettings Logging = Logsettings.Logging_Normal;
        /// <summary>
        /// Player Objects
        /// </summary>
        [Header("Player Spawnprefabs")]
        public List<NetworkPlayer> spawnPrefabs;

        void Awake()
        {
            if (Networkmanager == null)
                Networkmanager = this;
        }

        void Start()
        {
            Gameclient = new GameClient(Logging);
            Gameclient.OnClientog += Clientlog;

            Gameclient.discoveryLayer = new DiscoveryLayer(Gameclient);
            Gameclient.discoveryLayer.StartDiscovery();
            LowNetClientPackethandler.InitPackets(Gameclient);
            if (Autoconnect)
                ConnectServer();
        }

        void Update()
        {
            if (Connect)
            {
                Connect = false;
                ConnectServer();
            }
        }

        void LateUpdate()
        {
            if (Gameclient != null)
            {
                ConnectionId = Gameclient.GetConnectionId;
                IsConnected = Gameclient.isConnected;
            }

        }

        /// <summary>
        /// Set Server Connection to Client
        /// </summary>
        /// <param name="ServerIP"></param>
        /// <param name="ServerPort"></param>
        /// <param name="ServerPassword"></param>
        public void SetConnection(string ServerIP, int ServerPort, string ServerPassword)
        {
            IPAdresse = ServerIP;
            Serverport = ServerPort;
            Serverpassword = ServerPassword;
        }

        /// <summary>
        /// Connect the Client to Server
        /// </summary>
        public void ConnectServer()
        {
            if (Gameclient == null)
                return;

            Gameclient.Connect(IPAdresse, Serverport, Serverpassword, Timeout);
        }

        /// <summary>
        /// Disconnect Gameclient from Server
        /// </summary>
        public void Disconnect()
        {
            if (Gameclient != null)
                Gameclient.Disconnect();
        }

        void OnApplicationQuit()
        {
            Disconnect();
            try
            {
                Gameclient.StopListner();
            }
            catch { }
            Gameclient.OnClientog -= Clientlog;
        }

        private void Clientlog(object sender, ServerlogMessage e)
        {
            string now = e.TimeStamp.Millisecond.ToString("0.00");
            switch (e.LogType)
            {
                case Logmessage.Debug:
                    Debug.Log(string.Format("<color=#c5ff00>[" + now + "]</color><color=#0083ff>[DEBUG]</color> :: <color=#005bff>" + e.LogMessage + "</color>"));
                    break;

                case Logmessage.Warning:
                    Debug.Log(string.Format("<color=#c5ff00>[" + now + "]</color><color=#0083ff>[WARNING]</color> :: <color=#ffad00>" + e.LogMessage + "</color>"));
                    break;

                case Logmessage.Error:
                    Debug.Log(string.Format("<color=#c5ff00>[" + now + "]</color><color=#0083ff>[ERROR]</color> :: <color=#ff3000>" + e.LogMessage + "</color>"));
                    break;
                case Logmessage.Log:
                    Debug.Log(string.Format("<color=#c5ff00>[" + now + "]</color><color=#0083ff>[LOG]</color> :: <color=#00ff1d>" + e.LogMessage + "</color>"));
                    break;
            }
        }

        /// <summary>
        /// Create Clientside Playermodel, And Setflag is Own
        /// </summary>
        /// <param name="ModelId"></param>
        /// <param name="PlayerId"></param>
        /// <param name="Pos"></param>
        /// <param name="Rot"></param>
        /// <param name="Name"></param>
        public static void SpawnPlayer(int ModelId, int PlayerId, Vector3 Pos, Quaternion Rot, string Name = "LowNetPlayer")
        {
            NetworkPlayer model = Instantiate(Networkmanager.spawnPrefabs[ModelId]);
            model.SetPlayer(PlayerId, Name, Pos, Rot);
            Player.Add(PlayerId, model);
        }

        /// <summary>
        /// Remove Network Player
        /// </summary>
        /// <param name="PlayerId"></param>
        public static void RemovePlayer(int PlayerId)
        {
            GameObject player = Player[PlayerId].gameObject;
            Destroy(player);
            Player.Remove(PlayerId);
        }
    }
}