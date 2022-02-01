using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LowNet.Unity3D
{
    /// <summary>
    /// Network UI Script to Use UI With this Network System
    /// </summary>
    public abstract class NetworkScript : MonoBehaviour
    {
        private const int FrameWait = 2;

        /// <summary>
        /// On Script start
        /// </summary>
        public abstract void NetworkStart();

        /// <summary>
        /// On Script Update, Warning is not Every Frame!
        /// </summary>
        public abstract void NetworkUpdate();

        /// <summary>
        /// On Script Fixed Update, Warning is not Every Frame!
        /// </summary>
        public abstract void FixedNetworkUpdate();

        /// <summary>
        /// On Script Late Update, Warning is not Every Frame!
        /// </summary>
        public abstract void LateNetworkUpdate();

        /// <summary>
        /// default Unity 
        /// </summary>
        public void Start()
        {
            NetworkStart();
        }

        /// <summary>
        /// default Unity 
        /// </summary>
        private void Update()
        {
            if (Time.frameCount % FrameWait == 0)
            {
                NetworkUpdate();
            }
        }

        /// <summary>
        /// default Unity 
        /// </summary>
        private void FixedUpdate()
        {
            if (Time.frameCount % FrameWait == 0)
            {
                FixedNetworkUpdate();
            }
        }

        /// <summary>
        /// default Unity 
        /// </summary>
        private void LateUpdate()
        {
            if (Time.frameCount % FrameWait == 0)
            {
                LateNetworkUpdate();
            }
        }
    }
}