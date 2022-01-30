/*  __                  _   __     __ 
   / /   ____ _      __/ | / /__  / /_
  / /   / __ \ | /| / /  |/ / _ \/ __/
 / /___/ /_/ / |/ |/ / /|  /  __/ /_  
/_____/\____/|__/|__/_/ |_/\___/\__/  
Simple Unity3D Solution ©2022 by Kuxii
*/
using UnityEngine;

namespace LowNet.Data
{
    /// <summary>
    /// Custom Sync Object
    /// </summary>
    public class SyncObject
    {
        /// <summary>
        /// Scale
        /// </summary>
        public Vector3 LocalScale;
        /// <summary>
        /// Position
        /// </summary>
        public Vector3 LocalPos;
        /// <summary>
        /// Rotation
        /// </summary>
        public Quaternion LocalRot;
        /// <summary>
        /// Enabled/Disabled
        /// </summary>
        public bool LocalState;

        /// <summary>
        /// New SyncObject
        /// </summary>
        public SyncObject()
        {
            LocalScale = Vector3.zero;
            LocalPos = Vector3.zero;
            LocalRot = Quaternion.identity;
            LocalState = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public SyncObject(GameObject obj)
        {
            LocalScale = obj.transform.localScale;
            LocalPos = obj.transform.localPosition;
            LocalRot = obj.transform.localRotation;
            LocalState = obj.activeSelf;
        }
    }
}