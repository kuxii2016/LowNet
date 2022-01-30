/*  __                  _   __     __ 
   / /   ____ _      __/ | / /__  / /_
  / /   / __ \ | /| / /  |/ / _ \/ __/
 / /___/ /_/ / |/ |/ / /|  /  __/ /_  
/_____/\____/|__/|__/_/ |_/\___/\__/  
Simple Unity3D Solution ©2022 by Kuxii
*/
using System;
using UnityEngine;

namespace LowNet.Unity3D
{
    class SmartObject : MonoBehaviour
    {
        public int SmartObjectId = -1;
        public string UUID;

        public void OnEnable()
        {
            UUID = Guid.NewGuid().ToString();
        }

        public void OnDisable()
        {

        }
    }
}
