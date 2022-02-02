using LowNet.Enums;
using LowNet.Server.Events;
using LowNet.Server.Packets;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LowNet.Unity3D
{
    internal class ServerNetworkmanager : MonoBehaviour
    {
        /// <summary>
        /// Player Holder
        /// </summary>
        public static Dictionary<int, GameObject> Player = new Dictionary<int, GameObject>();
        public static ServerNetworkmanager Instance;

        [Header("Server IPAdresse")]
        public string ServerIP = "127.0.0.1";
        [Header("Server Listenport")]
        public int ServerPort = 4900;
        [Header("Max Amount of Player"), Range(2, 1000)]
        public int Maxplayer = 50;
        [Header("Serverlisten Name")]
        public string ServerName = "LowNet-Server";
        [Header("Server Password")]
        public string ServerPassword = "";
        [Header("Network Update Rate")]
        public NetworkUpdate NetworkSpeed = NetworkUpdate.FixedUpdate;
        [Header("Server Log Mode")]
        public LogMode ServerLogging = LogMode.LogNormal;
        [Header("Auto Start on Start")]
        public bool Autostart = false;
        [Header("Is Server Running")]
        public bool IsRunning = false;
        public static Server.Server server;
        /// <summary>
        /// Player Spawn Models
        /// </summary>
        [Header("All Playermodels"), Tooltip("Min 1 is Needet")]
        public List<NetworkPlayer> PlayerModels;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Instance = this;
        }

        private void Start()
        {
            server = new Server.Server(ServerPassword, ServerName, ServerIP, ServerPort, Maxplayer);
            Server.Server.SetSettings(ServerLogging);
            server.ConnectedEvent += OnConnect;
            server.DisconnectedEvent += OnDisconnect;
            server.LogMessageEvent += OnServerLog;
            if (Autostart && server != null)
                IsRunning = server.Startserver();
        }

        private void OnServerLog(object sender, LogMessageEventArgs e)
        {
            string now = e.dateTime.Millisecond.ToString("0.00");
            switch (e.Type)
            {
                case Enums.LogType.LogDebug:
                    Debug.Log(string.Format($"<color=#c5ff00>[{now}][LowNet-Server]</color><color=#0083ff>[DEBUG]</color><color=#818181>{e.ClassInfo}::{e.Message}</color>"));
                    break;

                case Enums.LogType.LogNormal:
                    Debug.Log(string.Format($"<color=#c5ff00>[{now}][LowNet-Server]</color><color=#00ff23>[LOG]</color><color=#818181>{e.ClassInfo}::{e.Message}</color>"));
                    break;

                case Enums.LogType.LogWarning:
                    Debug.LogWarning(string.Format($"<color=#c5ff00>[{now}][LowNet-Server]</color><color=#ffa200>[WARNING]</color><color=#818181>{e.ClassInfo}::{e.Message}</color>"));
                    break;

                case Enums.LogType.LogError:
                    Debug.LogError(string.Format($"<color=#c5ff00>[{now}][LowNet-Server]</color><color=#ff0000>[ERROR]</color><color=#818181>{e.ClassInfo}::{e.Message}, {e.Exception}</color>"));
                    break;
            }
        }

        private void OnDisconnect(object sender, DisconnectedEventArgs e)
        {
            Destroy(Player[e.Client.Connectionid].gameObject);
            Player.Remove(e.Client.Connectionid);
            foreach (var item in Server.Server.Clients.Values)
            {
                if (item.Session != null)
                    LOWNET_PLAYER.SendPacket(item, e.Client);
            }
        }

        private void OnConnect(object sender, ConnectedEventArgs e)
        {
            ClientNetworkmanager cm;
            bool isMulti = TryGetComponent<ClientNetworkmanager>(out cm);
            if (!isMulti)
            {
                NetworkPlayer player = Instantiate(Instance.PlayerModels[e.Client.Session.ModelId]);
                player.Prefab = player.gameObject;
                player.transform.position = e.Client.Session.Position;
                player.transform.rotation = e.Client.Session.Rotation;
                player.IsMyView = false;
                player.PlayerId = e.Client.Connectionid;
                player.PlayerName = e.Client.PlayerName;
                Player.Add(e.Client.Connectionid, player.gameObject);
                player.gameObject.name = "SV: " + player.PlayerId + " | " + player.PlayerName;
            }
            else
            {
                Server.Server.Log("Project is Multibuild, Spawn only Clientside Player", Instance);
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

        private void OnApplicationQuit()
        {
            server.ConnectedEvent -= OnConnect;
            server.DisconnectedEvent -= OnDisconnect;
            server.LogMessageEvent -= OnServerLog;
        }

        #region Threadmanager

        private static readonly List<Action> executeOnMainThread = new List<Action>();
        private static readonly List<Action> executeCopiedOnMainThread = new List<Action>();
        private static bool actionToExecuteOnMainThread = false;

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
    }
}