/*  __                  _   __     __ 
   / /   ____ _      __/ | / /__  / /_
  / /   / __ \ | /| / /  |/ / _ \/ __/
 / /___/ /_/ / |/ |/ / /|  /  __/ /_  
/_____/\____/|__/|__/_/ |_/\___/\__/  
Simple Unity3D Solution ©2020 by Kuxii
*/
using LowNet.Data;
using System;

namespace LowNet.Events
{
    /// <summary>
    /// Server Logmessage Event
    /// </summary>
    public class ServerlogMessage : EventArgs
    {
        /// <summary>
        /// Logtype of Message
        /// </summary>
        public Logmessage LogType;
        /// <summary>
        /// Log Message
        /// </summary>
        public string LogMessage;
        /// <summary>
        /// Fire Time
        /// </summary>
        public DateTime TimeStamp;
    }
}
