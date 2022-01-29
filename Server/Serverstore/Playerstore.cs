/*  __                  _   __     __ 
   / /   ____ _      __/ | / /__  / /_
  / /   / __ \ | /| / /  |/ / _ \/ __/
 / /___/ /_/ / |/ |/ / /|  /  __/ /_  
/_____/\____/|__/|__/_/ |_/\___/\__/  
Simple Unity3D Solution ©2020 by Kuxii
*/
using LowNet.Server;
using LowNet.Server.Data;
using System.Collections.Generic;

namespace LowNet.Serverstore
{
    /// <summary>
    /// Player Store hold All Current Playerslots
    /// </summary>
    public class Playerstore
    {
        /// <summary>
        /// Create new Playerstore Instance
        /// </summary>
        /// <param name="server">Main Server only for Logging</param>
        public Playerstore(LowNetServer server)
        {
            Instance = this;
            Mainserver = server;
            Mainserver.Debug("Init new Playerstore.", this);

            for (int i = 1; i < Mainserver.GetMaxplayer; i++)
            {
                Clients.Add(i, new Client(i, Mainserver));
            }
        }
        /// <summary>
        /// Get Playerstore instance
        /// </summary>
        public static Playerstore Instance;
        private readonly LowNetServer Mainserver;
        /// <summary>
        /// Client Slots, Holds All Slots If Empty or not.!
        /// </summary>
        public Dictionary<int, Client> Clients = new Dictionary<int, Client>();

        /// <summary>
        /// Get Store Playercount
        /// </summary>
        /// <returns></returns>
        public int GetPlayer()
        {
            int playercount = 0;
            foreach (var item in Clients.Values)
            {
                if (item.Session != null)
                    playercount++;
            }
            return playercount;
        }
    }
}