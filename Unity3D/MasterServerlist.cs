/*  __                  _   __     __ 
   / /   ____ _      __/ | / /__  / /_
  / /   / __ \ | /| / /  |/ / _ \/ __/
 / /___/ /_/ / |/ |/ / /|  /  __/ /_  
/_____/\____/|__/|__/_/ |_/\___/\__/  
Simple Unity3D Solution ©2022 by Kuxii
*/
using System;
using System.Collections;
using System.Collections.Generic;
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
        /// <summary>
        /// Server Id from this server
        /// </summary>
        private int ServerId;
        /// <summary>
        /// Failed Update
        /// </summary>
        private bool Error = false;

        public void start()
        {
            if (AutoinsertServer && buildType == BuildType.Server)
            {
                StartCoroutine(InsertServer());
                StartCoroutine(UpdateServer());
            }
        }

        public IEnumerator InsertServer()
        {
            yield return new WaitForSeconds(10);
            string Message = $"{ServerNetworkmanager.NetworkManager.Servername}|{ServerNetworkmanager.NetworkManager.ServerIP}|{ServerNetworkmanager.NetworkManager.Serverport}|{ServerNetworkmanager.NetworkManager.Serverpasword}|{ServerNetworkmanager.NetworkManager.MaxPlayer}|7";
            SendData(1, Message);
        }

        public IEnumerator UpdateServer()
        {
            yield return new WaitForSeconds(110);
            if (!Error)
            {
                string Message = $"{ServerId}|{ServerNetworkmanager.NetworkManager.server.GetPlayer}|{ServerNetworkmanager.NetworkManager.Serverpasword}|";
                SendData(2, Message);
            }
            else
            {
                StartCoroutine(InsertServer());
            }
            StartCoroutine(UpdateServer());
        }

        public void SendData(int packetId, string text)
        {
            try
            {
                string message = $"{(uint)packetId}|{text}|<eof>";
                byte[] sendbuf = Encoding.ASCII.GetBytes(message);
                UdpClient client = new UdpClient();
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(Masterip), Masterport);
                client.Connect(endpoint);
                client.Client.SendTimeout = 800;
                client.Client.ReceiveTimeout = 800;
                try
                {
                    client.Send(sendbuf, sendbuf.Length);
                }
                catch (SocketException)
                {
                    Error = true;
                    ServerId = -1;
                }
                try
                {
                    byte[] bytes = client.Receive(ref endpoint);
                    string ServerMsg = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                    string[] data = new string[1] { "" };
                    data = ServerMsg.Split('|');
                        ServerId = int.Parse(data[1]);
                        Error = false;
                }
                catch (SocketException)
                {
                    Error = true;
                    ServerId = -1;
                }
                client.Close();
            }
            catch (Exception)
            {
                Error = true;
                ServerId = -1;
            }
        }
    }
}