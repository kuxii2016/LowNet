using LowNet.Enums;
using LowNet.Server;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LowNet.Unity3D
{
    class ServerNetworkmanager : MonoBehaviour
    {
        public static ServerNetworkmanager Instance;
        [Header("Server IPAdresse")]
        public string ServerIP = "127.0.0.1";
        [Header("Server Listenport")]
        public int ServerPort = 4900;
        [Header("Max Amount of Player"), Range(2,1000)]
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
        public bool IsRunning = false;
        public static Server.Server server;

        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Instance = this;
        }

        void Start()
        {
            server = new Server.Server(ServerPassword, ServerName, ServerIP, ServerPort, Maxplayer);
            Server.Server.SetSettings(ServerLogging);

            if (Autostart && server != null)
                IsRunning = server.Startserver();
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
    }
}
