using UnityEngine;

namespace LowNet.Unity3D
{
    internal class NetworkUIManager : MonoBehaviour
    {
        public static NetworkUIManager Instance;

        [Header("Custom UI Was will Open on Disconnect"), Tooltip("Will only Open is One Object Set!")]
        public MonoBehaviour OnDisconnect;

        [Header("Custom UI Was will Open on Connection Lost"), Tooltip("Will only Open is One Object Set!")]
        public MonoBehaviour OnConnectionlost;

        [Header("Custom UI Was will Open on Missing Playername"), Tooltip("Will only Open is One Object Set!")]
        public MonoBehaviour OnEnterplayername;

        [Header("Custom UI Was will Open on Invalid Serverpassword"), Tooltip("Will only Open is One Object Set!")]
        public MonoBehaviour OnEnterserverPassword;

        private void Start()
        {
            Instance = this;
        }
    }
}