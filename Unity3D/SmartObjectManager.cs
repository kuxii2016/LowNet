/*  __                  _   __     __ 
   / /   ____ _      __/ | / /__  / /_
  / /   / __ \ | /| / /  |/ / _ \/ __/
 / /___/ /_/ / |/ |/ / /|  /  __/ /_  
/_____/\____/|__/|__/_/ |_/\___/\__/  
Simple Unity3D Solution ©2022 by Kuxii
*/
using System.Collections.Generic;
using UnityEngine;

namespace LowNet.Unity3D
{
    class SmartObjectManager : MonoBehaviour
    {
        public static SmartObjectManager Instance;

        public List<SmartObject> SmartObjects;

        public void Awake()
        {
            if (Instance == null)
                Instance = this;
        }
    }
}
