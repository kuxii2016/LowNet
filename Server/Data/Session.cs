/*  __                  _   __     __ 
   / /   ____ _      __/ | / /__  / /_
  / /   / __ \ | /| / /  |/ / _ \/ __/
 / /___/ /_/ / |/ |/ / /|  /  __/ /_  
/_____/\____/|__/|__/_/ |_/\___/\__/  
Simple Unity3D Solution ©2020 by Kuxii
*/
using LowNet.Data;
using System;
using UnityEngine;

namespace LowNet.Server.Data
{
    /// <summary>
    /// Minimal Player Session
    /// </summary>
    [Serializable]
    public class Session
    {
        /// <summary>
        /// Create new Playersession
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="server"></param>
        public Session(int Id, LowNetServer server)
        {
            ConnectionId = Id;
            GUID = Guid.NewGuid().ToString();
            Connected = DateTime.Now;
            MainServer = server;
            server.Debug($"Create new Playersession: {GUID}", this);
        }
        /// <summary>
        /// Main Server only for Logging Important
        /// </summary>
        private readonly LowNetServer MainServer;
        /// <summary>
        /// Connection or Player Id
        /// </summary>
        private readonly int ConnectionId;
        /// <summary>
        /// Temp. Network UUID
        /// </summary>
        private readonly string GUID;
        /// <summary>
        /// Playername
        /// </summary>
        private readonly string Playername = "LowNetplayer";
        /// <summary>
        /// Connected Timestamp
        /// </summary>
        internal DateTime Connected;
        /// <summary>
        /// Last Packet Timestamp
        /// </summary>
        internal DateTime Lastpacket;
#pragma warning disable
        /// <summary>
        /// Playerprefab
        /// </summary>
        private GameObject MyPrefab;
#pragma warning restore

#pragma warning disable IDE0059
        /// <summary>
        /// Get ConnectionId
        /// </summary>
        public int GetConnectionId { get { return ConnectionId; } }
        /// <summary>
        /// Get Client UUID
        /// </summary>
        public string GetUUID { get { return GUID; } }
        /// <summary>
        /// Get Local Playername
        /// </summary>
        public string GetPlayername { get { return Playername; } }
        /// <summary>
        /// Set Localplayername
        /// </summary>
        public string SetPlayername { set { string playername = Playername; } }
        /// <summary>
        /// Get Connection Timestamp
        /// </summary>
        public DateTime GetConnectedTime { get { return Connected; } }
        /// <summary>
        /// Get Lastpacket Timestamp
        /// </summary>
        public DateTime GetLastpacketTime { get { return Lastpacket; } }
        /// <summary>
        /// Get Localprefab
        /// </summary>
        public GameObject GetPrefab { get { return MyPrefab; } }
        /// <summary>
        /// Set Localprefab
        /// </summary>
        public GameObject SetPrefab { set { GameObject gameObject = MyPrefab; } }
#pragma warning restore IDE0059

        /// <summary>
        /// Add Position of my Prefab to Store
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        public Store Writeplayer(Store store)
        {
            store.PushVector3(MyPrefab.transform.position);
            store.PushQuaternion(MyPrefab.transform.rotation);
            return store;
        }

        /// <summary>
        /// Set Pos of my Prefab from Store
        /// </summary>
        /// <param name="store"></param>
        public void ReadPos(Store store)
        {
            Vector3 pos = store.PopVector3();
            Quaternion rot = store.PopQuaternion();

            MyPrefab.transform.position = pos;
            MyPrefab.transform.rotation = rot;
        }
    }
}