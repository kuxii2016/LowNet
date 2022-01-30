/*  __                  _   __     __ 
   / /   ____ _      __/ | / /__  / /_
  / /   / __ \ | /| / /  |/ / _ \/ __/
 / /___/ /_/ / |/ |/ / /|  /  __/ /_  
/_____/\____/|__/|__/_/ |_/\___/\__/  
Simple Unity3D Solution ©2022 by Kuxii
*/
using LowNet.Data;
using LowNet.Events;
using LowNet.Server;
using LowNet.Server.Data;
using LowNet.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LowNet.Unity3D
{
    /// <summary>
    /// Unity3d Server Networkmanager Component
    /// </summary>
    public class ServerNetworkmanager : MonoBehaviour
    {
        /// <summary>
        /// Networkplayer Holder
        /// </summary>
        public static Dictionary<int, NetworkPlayer> Player = new Dictionary<int, NetworkPlayer>();

        /// <summary>
        /// Server Instance
        /// </summary>
        public LowNetServer server;
        /// <summary>
        /// Network Manager Instance
        /// </summary>
        public static ServerNetworkmanager NetworkManager { get; private set; }
        /// <summary>
        /// Server ServerIP
        /// </summary>
        public string ServerIP = "127.0.0.1";
        /// <summary>
        /// Server Listenport
        /// </summary>
        public int Serverport = 4900;
        /// <summary>
        /// Server Name
        /// </summary>
        public string Servername = "LowNet Server";
        /// <summary>
        /// Server Password
        /// </summary>
        public string Serverpasword;
        /// <summary>
        /// Server Max Player Amount
        /// </summary>
        [Range(0, 500)]
        public int MaxPlayer = 25;
        /// <summary>
        /// Autostart on Start from this Component
        /// </summary>
        public bool Autostart = false;
        /// <summary>
        /// Logmode Settings
        /// </summary>
        [Header("Server Logging Settings")]
        public Logsettings Logging;
        /// <summary>
        /// Player Objects
        /// </summary>
        [Header("Player Spawnprefabs")]
        public List<NetworkPlayer> spawnPrefabs;

        void Awake()
        {
            if (NetworkManager == null)
                NetworkManager = this;
        }

        void Start()
        {
            server = new LowNetServer(Servername, Serverpasword, Serverport, MaxPlayer, true, Logging);
            server.OnServerlog += Serverlog;
            server.ClientConnected += CreatePlayer;
            server.ClientDisconnected += RemovePlayer;
            if (Autostart)
            {
                server.Start();
            }
            else
                Serverlog(this, new ServerlogMessage { LogType = Logmessage.Warning, LogMessage = ClassUtils.TryGetClass(this) + "()=>" + "Autostart is Disabled, Call 'NetworkManager.Startserver()'", TimeStamp = DateTime.Now, });
        }

        private void CreatePlayer(object sender, ClientConnectedEventArgs e)
        {
            Debug.Log("Connect Spawn Player: " + e.client.ClientId);
            NetworkPlayer model = Instantiate(spawnPrefabs[e.client.Networkplayer.Unity3dModel]);
            model.SetPlayer(e.client.ClientId, e.client.Networkplayer.Name, e.client.Networkplayer.PlayerPos, e.client.Networkplayer.PlayerRot);
            model.Prefab = model.gameObject;
            Player.Add(e.client.ClientId, model);
        }

        private void RemovePlayer(object sender, ClientDisconnectedEventArgs e)
        {
            int player = e.client.ClientId;
            StartCoroutine(DestroyPlayer(player));
        }

        IEnumerator DestroyPlayer(int player)
        {
            NetworkPlayer p = Player[player];
            Destroy(p.gameObject);
            Player.Remove(p.PlayerId);
            yield return null;
        }

        void Update()
        {

        }

        void OnApplicationQuit()
        {
            if (server != null)
            {
                server.Stop();
                server.OnServerlog -= Serverlog;
                server.ClientConnected -= CreatePlayer;
                server.ClientDisconnected -= RemovePlayer;
            }
        }

        private void Serverlog(object sender, ServerlogMessage e)
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

        #region UDP Sending
        /// <summary>
        /// Send UDP Data
        /// </summary>
        public static void SendUDP(Client client, Store store)
        {
            NetworkManager.server.UDPLayer.SendUDP(client, store);
        }

        /// <summary>
        /// Send UDP Data to All
        /// </summary>
        public static void SendUDPAll(Store store)
        {
            NetworkManager.server.UDPLayer.SendAll(store);
        }

        /// <summary>
        /// Send UDP Data to All but not on this Player
        /// </summary>
        public static void SendUDPAll(Client client, Store store)
        {
            NetworkManager.server.UDPLayer.SendAll(client, store);
        }
        #endregion

        #region TCP Sending
        /// <summary>
        /// Send data only to Client
        /// </summary>
        /// <param name="client">Client</param>
        /// <param name="store">Packet Store</param>
        public static void SendTCP(Client client, Store store)
        {
            NetworkManager.server.TCPLayer.Send(client, store);
        }

        /// <summary>
        /// Send data to All Clients
        /// </summary>
        /// <param name="store">Packet Store</param>
        public static void SendTCPAll(Store store)
        {
            NetworkManager.server.TCPLayer.SendAll(store);
        }

        /// <summary>
        /// Send data to all Clients only not this Client
        /// </summary>
        /// <param name="client">Client</param>
        /// <param name="store">Packet Store</param>
        public static void SendTCPAll(Client client, Store store)
        {
            NetworkManager.server.TCPLayer.SendAll(client, store);
        }

        /// <summary>
        /// Send data only to Client
        /// </summary>
        /// <param name="Client">ClientId</param>
        /// <param name="store">Packet Store</param>
        public static void SendTCP(int Client, Store store)
        {
            NetworkManager.server.TCPLayer.Send(Client, store);
        }

        /// <summary>
        /// Send data to all Clients only not this Client
        /// </summary>
        /// <param name="Client">ClientId</param>
        /// <param name="store">Packet Store</param>
        public static void SendTCPAll(int Client, Store store)
        {
            NetworkManager.server.TCPLayer.SendAll(Client, store);
        }
        #endregion

    }
}