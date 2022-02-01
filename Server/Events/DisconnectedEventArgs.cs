using System;

namespace LowNet.Server.Events
{
    /// <summary>
    /// On Client Disconnect Trigger
    /// </summary>
    public class DisconnectedEventArgs : EventArgs
    {
        /// <summary>
        /// Trigger Client
        /// </summary>
        public Client Client;

        /// <summary>
        /// Message
        /// </summary>
        public string Message;

        /// <summary>
        /// Raising time
        /// </summary>
        public DateTime date = DateTime.Now;
    }
}