namespace LowNet.Packets
{
    /// <summary>
    /// Lownet Packet Order
    /// </summary>
    public enum LowNetpacketOrder
    {
        /// <summary>
        /// On Client Connect
        /// </summary>
        LOWNET_CONNECT = 1,
        /// <summary>
        /// Handshake with Server
        /// </summary>
        LOWNET_HANDSHAKE,
        /// <summary>
        /// Lownet Data
        /// </summary>
        LOWNET_DATA,
        /// <summary>
        /// Used for Player Spawning
        /// </summary>
        LOWNET_PLAYER,
        /// <summary>
        /// Used for Objects Spawning
        /// </summary>
        LOWNET_OBJECT,
        /// <summary>
        /// Connects Player UDP Layer
        /// </summary>
        LOWNET_CONNECT_UDP,
    }
}