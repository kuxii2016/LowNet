using LowNet.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace LowNet.Server.Events
{
    /// <summary>
    /// NowNet Log Message
    /// </summary>
    public class LogMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Type of Message
        /// </summary>
        public LogType Type = LogType.LogDebug;
        /// <summary>
        /// Message Self
        /// </summary>
        public string Message = null;
        /// <summary>
        /// On Error with More Infos
        /// </summary>
        public string Exception = null;
        /// <summary>
        /// Class was Trigger
        /// </summary>
        public string ClassInfo = null;
        /// <summary>
        /// Timestamp on Raise
        /// </summary>
        public DateTime dateTime = DateTime.Now;
    }
}
