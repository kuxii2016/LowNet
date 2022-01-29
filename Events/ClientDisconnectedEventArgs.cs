/*  __                  _   __     __ 
   / /   ____ _      __/ | / /__  / /_
  / /   / __ \ | /| / /  |/ / _ \/ __/
 / /___/ /_/ / |/ |/ / /|  /  __/ /_  
/_____/\____/|__/|__/_/ |_/\___/\__/  
Simple Unity3D Solution ©2020 by Kuxii
*/
using LowNet.Server.Data;
using System;

namespace LowNet.Events
{
    /// <summary>
    /// Client Connection Event
    /// </summary>
    public class ClientDisconnectedEventArgs : EventArgs
    {
        /// <summary>
        /// Client was triggert this Event
        /// </summary>
        public Client client;
        /// <summary>
        /// Session from Client
        /// </summary>
        public Session session;
        /// <summary>
        /// Fire Time
        /// </summary>
        public DateTime Connected;
    }
}
