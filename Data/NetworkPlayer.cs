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
        public int Unity3dModel { get; set; } = 0;

        public void SetPlayer(int playerId)
        {
            PlayerId = playerId;
        }

        public void SetPlayer(int playerId, string Name)
        {
            PlayerId = playerId;
            Name = Name;
        }

        public void SetPlayer(int playerId, Vector3 Pos)
        {
            PlayerId = playerId;
            GameobjectPos = Pos;
        }

        public void SetPlayer(int playerId, string Name, Vector3 Pos)
        {
            PlayerId = playerId;
            Name = Name;
            GameobjectPos = Pos;
        }

        public void SetPlayer(int playerId, Vector3 Pos, Quaternion Rot)
        {
            PlayerId = playerId;
            GameobjectPos = Pos;
            GameobjectRot = Rot;
        }

        public void SetPlayer(int playerId, string Name, Vector3 Pos, Quaternion Rot)
        {
            PlayerId = playerId;
            Name = Name;
            GameobjectPos = Pos;
            GameobjectRot = Rot;
        }

        public void SetModel(int Model) => Unity3dModel = Model;
    }
}
