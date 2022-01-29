using System;
using System.Collections.Generic;
using System.Text;
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
