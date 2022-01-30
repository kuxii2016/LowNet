using UnityEngine;

namespace LowNet.Data
{
    /// <summary>
    /// LowNet Base Networkplayer
    /// </summary>
    public class NetworkPlayer
    {
        /// <summary>
        /// Name From GameObject
        /// </summary>
        public string Name = "NetworkPlayer";
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
        /// Ininity 3d Modellist Id
        /// </summary>
        public int Unity3dModel { get; set; } = 0;

        #region Set Playerinfo
        /// <summary>
        /// Set Player
        /// </summary>
        /// <param name="playerId"></param>
        public void SetPlayer(int playerId)
        {
            PlayerId = playerId;
        }
        /// <summary>
        /// Set Player
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="name"></param>
        public void SetPlayer(int playerId, string name)
        {
            PlayerId = playerId;
            Name = name;
        }
        /// <summary>
        /// Set Player
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="Pos"></param>
        public void SetPlayer(int playerId, Vector3 Pos)
        {
            PlayerId = playerId;
            GameobjectPos = Pos;
        }
        /// <summary>
        /// Set Player
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="name"></param>
        /// <param name="Pos"></param>
        public void SetPlayer(int playerId, string name, Vector3 Pos)
        {
            PlayerId = playerId;
            Name = name;
            GameobjectPos = Pos;
        }
        /// <summary>
        /// Set Player
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="Pos"></param>
        /// <param name="Rot"></param>
        public void SetPlayer(int playerId, Vector3 Pos, Quaternion Rot)
        {
            PlayerId = playerId;
            GameobjectPos = Pos;
            GameobjectRot = Rot;
        }
        /// <summary>
        /// Set Player
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="name"></param>
        /// <param name="Pos"></param>
        /// <param name="Rot"></param>
        public void SetPlayer(int playerId, string name, Vector3 Pos, Quaternion Rot)
        {
            PlayerId = playerId;
            Name = name;
            GameobjectPos = Pos;
            GameobjectRot = Rot;
        }
        /// <summary>
        /// Set Player Model
        /// </summary>
        /// <param name="Model"></param>
        public void SetModel(int Model) => Unity3dModel = Model;
        #endregion
    }
}