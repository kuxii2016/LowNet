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
        /// Own Player Instance
        /// </summary>
        public static NetworkPlayer networkPlayer;
        /// <summary>
        /// Prefab Model
        /// </summary>
        public GameObject Prefab;
        /// <summary>
        /// Network PlayerId
        /// </summary>
        public int PlayerId { get; set; }
        /// <summary>
        /// Private PlayerPos
        /// </summary>
        private Vector3 GameobjectPos;
        /// <summary>
        /// Public PlayerPos
        /// </summary>
        public Vector3 PlayerPos { get { return GameobjectPos; } }
        /// <summary>
        /// Private PlayerRot
        /// </summary>
        private Quaternion GameobjectRot;
        /// <summary>
        /// Public PlayerRot
        /// </summary>
        public Quaternion PlayerRot { get { return GameobjectRot; } }
        /// <summary>
        /// Is this My Player or Networkclient
        /// </summary>
        public bool IsMyView { get; private set; } = false;

        #region Set Playerinfos
        /// <summary>
        /// Set Player Infos
        /// </summary>
        /// <param name="playerId"></param>
        public void SetPlayer(int playerId)
        {
            PlayerId = playerId;
            if (ClientNetworkmanager.Networkmanager != null && ClientNetworkmanager.Networkmanager.ConnectionId == playerId)
                IsMyView = true;
            SetInstance();
        }
        /// <summary>
        /// Set Player Infos
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="Name"></param>
        public void SetPlayer(int playerId, string Name)
        {
            PlayerId = playerId;
            gameObject.name = "[" + PlayerId + "] " + Name;
            if (ClientNetworkmanager.Networkmanager != null && ClientNetworkmanager.Networkmanager.ConnectionId == playerId)
                IsMyView = true;
            SetInstance();
        }
        /// <summary>
        /// Set Player Infos
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="Pos"></param>
        public void SetPlayer(int playerId, Vector3 Pos)
        {
            PlayerId = playerId;
            GameobjectPos = Pos;
            gameObject.transform.position = Pos;
            if (ClientNetworkmanager.Networkmanager != null && ClientNetworkmanager.Networkmanager.ConnectionId == playerId)
                IsMyView = true;
            SetInstance();
        }
        /// <summary>
        /// Set Player Infos
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="Name"></param>
        /// <param name="Pos"></param>
        public void SetPlayer(int playerId, string Name, Vector3 Pos)
        {
            PlayerId = playerId;
            gameObject.name = "[" + PlayerId + "] " + Name;
            GameobjectPos = Pos;
            gameObject.transform.position = Pos;
            if (ClientNetworkmanager.Networkmanager != null && ClientNetworkmanager.Networkmanager.ConnectionId == playerId)
                IsMyView = true;
            SetInstance();
        }
        /// <summary>
        /// Set Player Infos
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="Pos"></param>
        /// <param name="Rot"></param>
        public void SetPlayer(int playerId, Vector3 Pos, Quaternion Rot)
        {
            PlayerId = playerId;
            GameobjectPos = Pos;
            gameObject.transform.position = Pos;
            GameobjectRot = Rot;
            gameObject.transform.rotation = Rot;
            if (ClientNetworkmanager.Networkmanager != null && ClientNetworkmanager.Networkmanager.ConnectionId == playerId)
                IsMyView = true;
            SetInstance();
        }
        /// <summary>
        /// Set Player Infos
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="Name"></param>
        /// <param name="Pos"></param>
        /// <param name="Rot"></param>
        public void SetPlayer(int playerId, string Name, Vector3 Pos, Quaternion Rot)
        {
            PlayerId = playerId;
            gameObject.name = "[" + PlayerId + "] " + Name;
            GameobjectPos = Pos;
            gameObject.transform.position = Pos;
            GameobjectRot = Rot;
            gameObject.transform.rotation = Rot;
            if (ClientNetworkmanager.Networkmanager != null && ClientNetworkmanager.Networkmanager.ConnectionId == playerId)
                IsMyView = true;
            SetInstance();
        }
        /// <summary>
        /// Set Static instance on Client when is my Ownplayer
        /// </summary>
        public void SetInstance()
        {
            if (IsMyView)
                networkPlayer = this;
        }
        #endregion

        //TODO: Network Pos, Rot, Sync
    }
}