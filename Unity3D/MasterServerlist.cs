/*  __                  _   __     __ 
   / /   ____ _      __/ | / /__  / /_
  / /   / __ \ | /| / /  |/ / _ \/ __/
 / /___/ /_/ / |/ |/ / /|  /  __/ /_  
/_____/\____/|__/|__/_/ |_/\___/\__/  
Simple Unity3D Solution ©2022 by Kuxii
*/
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace LowNet.Unity3D
{
    /// <summary>
    /// Current Programm
    /// </summary>
    public enum BuildType
    {
        /// <summary>
        /// Server
        /// </summary>
        Server,
        /// <summary>
        /// Client
        /// </summary>
        Client,
        /// <summary>
        /// Beides
        /// </summary>
        Both
    }

    class MasterServerlist : MonoBehaviour
    {
        /// <summary>
        /// Programtype
        /// </summary>
        [Header("Define Build Type")]
        public BuildType buildType = BuildType.Server;
        /// <summary>
        /// Master ServerIP
        /// </summary>
        [Header("Masterserver IP"), Tooltip("Changeable Master need only Same Protokoll")]
        public string Masterip = "151.80.80.190";
        /// <summary>
        /// Master Port
        /// </summary>
        [Header("Masterserver Port")]
        public int Masterport = 6667;
        /// <summary>
        /// Gamename
        /// </summary>
        [Header("Gamename for Querry")]
        public string Gamename = "LowNet Example";
        /// <summary>
        /// Write Server On Start
        /// </summary>
        [Header("Insert Server, when build is Server on Start")]
        public bool AutoinsertServer = false;
        /// <summary>
        /// Load Server auto on Start
        /// </summary>
        [Header("Autoload List, when Build is Client")]
        public bool AutoloadServerliste = false;

        public int MasterServerId = 0;
        public int ResponsedMaster = -1;
        public int Lasttested = 0;

        void Start()
        {
            StartCoroutine(Registerserver());
        }

        IEnumerator Registerserver()
        {
            yield return new WaitForSeconds(5);
            string Message = $"{ServerNetworkmanager.NetworkManager.Servername}|{ServerNetworkmanager.NetworkManager.ServerIP}|{ServerNetworkmanager.NetworkManager.Serverport}|{ServerNetworkmanager.NetworkManager.Serverpasword}|{ServerNetworkmanager.NetworkManager.MaxPlayer}|8";
            var reponse = StartCoroutine(SendMessage(0, Message, OnComplete));
        }

        IEnumerator Updateserver()
        {
            yield return new WaitForSecondsRealtime(2 * 50);
            StartCoroutine(SendCheckserver());
            StartCoroutine(Updateserver());
        }

        IEnumerator RemoveServer()
        {
            string Message = $"{MasterServerId}";
            var reponse = StartCoroutine(SendMessage(3, Message, OnComplete));
            yield return new WaitForSecondsRealtime(1);
        }

        IEnumerator SendCheckserver()
        {
            yield return new WaitForSecondsRealtime(1);
            string Message = $"{MasterServerId}|{ServerNetworkmanager.NetworkManager.server.GetPlayer}|{ServerNetworkmanager.NetworkManager.Serverpasword}";
            var reponse = StartCoroutine(SendMessage(2, Message, OnComplete));
        }

        public IEnumerator SendMessage(int packetId, string text, System.Action<int, string> callbackOnFinish)
        {
            try
            {
                string message = $"{(uint)packetId}|{text}|<eof>";
                byte[] sendbuf = Encoding.ASCII.GetBytes(message);
                var client = new UdpClient();
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(Masterip), Masterport);
                client.Connect(endpoint);
                client.Client.SendTimeout = 5000;
                client.Client.ReceiveTimeout = 5000;
                // send data
                client.Send(sendbuf, sendbuf.Length);
                byte[] bytes = client.Receive(ref endpoint);
                var ServerMsg = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                callbackOnFinish(packetId, ServerMsg);
            }
            catch (Exception)
            {
            }

            yield return null;
        }

        private void OnApplicationQuit()
        {
            StartCoroutine(RemoveServer());
        }

        public void OnComplete(int packetId, string value)
        {
            string[] data;
            data = new string[] { "" };
            data = value.Split('|');
            if (value.Contains("Yes iam Live"))
            {
                Lasttested = 0;
                StartCoroutine(Registerserver());
            }
            if (value.Contains("Server_Add"))
            {
                MasterServerId = int.Parse(data[1]);
                StartCoroutine(Updateserver());
                Debug.Log("Master Responsed");
            }
            if (value.Contains("OK"))
            {
                Debug.Log("Master Responsed");
            }
        }
    }
}