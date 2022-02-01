using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LowNet.Server
{
    /// <summary>
    /// Playersession
    /// </summary>
    public class Session
    {
        /// <summary>
        /// Create new Playersession
        /// </summary>
        /// <param name="playername"></param>
        /// <param name="modelId"></param>
        public Session(string playername, int modelId)
        {
            Playername = playername;
            Joined = DateTime.Now;
            ModelId = modelId;
        }
        /// <summary>
        /// Playername
        /// </summary>
        public string Playername { get; private set; }
        /// <summary>
        /// Connecting Time
        /// </summary>
        public DateTime Joined { get; private set; }
        /// <summary>
        /// Player Position
        /// </summary>
        public Vector3 Position { get; set; }
        /// <summary>
        /// Player Rotation
        /// </summary>
        public Quaternion Rotation { get; set; }
        /// <summary>
        /// Spawnmanager ModelId
        /// </summary>
        public int ModelId { get; set; } = 0;
    }
}
