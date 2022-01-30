using LowNet.Data;
using System;
using UnityEngine;

namespace LowNet.Unity3D
{
    class LowNetlog : MonoBehaviour
    {

        public static void PrintLog(string Message, Logmessage LogType)
        {
            string now = DateTime.Now.Millisecond.ToString("0.00");
            switch (LogType)
            {
                case Logmessage.Debug:
                    Debug.Log(string.Format("<color=#c5ff00>[" + now + "]</color><color=#0083ff>[DEBUG]</color> :: <color=#005bff>" + Message + "</color>"));
                    break;

                case Logmessage.Warning:
                    Debug.Log(string.Format("<color=#c5ff00>[" + now + "]</color><color=#0083ff>[WARNING]</color> :: <color=#ffad00>" + Message + "</color>"));
                    break;

                case Logmessage.Error:
                    Debug.Log(string.Format("<color=#c5ff00>[" + now + "]</color><color=#0083ff>[ERROR]</color> :: <color=#ff3000>" + Message + "</color>"));
                    break;
                case Logmessage.Log:
                    Debug.Log(string.Format("<color=#c5ff00>[" + now + "]</color><color=#0083ff>[LOG]</color> :: <color=#00ff1d>" + Message + "</color>"));
                    break;
            }
        }
    }
}
