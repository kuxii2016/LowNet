using UnityEngine;

namespace LowNet.Unity3D
{
    internal class NetworkUIManager : MonoBehaviour
    {
        public static NetworkUIManager Instance;
        /// <summary>
        /// UI Element was is Calling on Disconnect
        /// </summary>
        [Header("Custom UI Was will Open on Disconnect"), Tooltip("Will only Open is One Object Set!")]
        public MonoBehaviour OnDisconnect;
        /// <summary>
        /// UI Onconnection Lost
        /// </summary>
        [Header("Custom UI Was will Open on Connection Lost"), Tooltip("Will only Open is One Object Set!")]
        public MonoBehaviour OnConnectionlost;
        /// <summary>
        /// On Enter Playername
        /// </summary>
        [Header("Custom UI Was will Open on Missing Playername"), Tooltip("Will only Open is One Object Set!")]
        public MonoBehaviour OnEnterplayername;
        /// <summary>
        /// On enter serverpassword
        /// </summary>
        [Header("Custom UI Was will Open on Invalid Serverpassword"), Tooltip("Will only Open is One Object Set!")]
        public MonoBehaviour OnEnterserverPassword;

        private void Start()
        {
            Instance = this;
        }
    }
}