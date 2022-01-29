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
