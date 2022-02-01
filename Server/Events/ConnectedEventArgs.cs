using System;

namespace LowNet.Server.Events
{
    /// <summary>
    /// On Client Connect Trigger
    /// </summary>
    public class ConnectedEventArgs : EventArgs
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