using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LowNet.Unity3D
{
    /// <summary>
    /// Network Player
    /// </summary>
    [System.Serializable]
    public class NetworkPlayer : MonoBehaviour
    {
        /// <summary>
        /// Connection Id from Server
        /// </summary>
        [Header("Player ConnectionId")]
        public int PlayerId;
        /// <summary>
        /// Playername from this Connection
        /// </summary>
        [Header("Playername")]
        public string PlayerName;
        /// <summary>
        /// Player prefab Owner yes or no
        /// </summary>
        [Header("Is My Prefab")]
        public bool IsMyView = false;
    }
}