namespace LowNet.Enums
{
    /// <summary>
    /// Packet Idendifyer
    /// </summary>
    public enum Packet
    {
        /// <summary>
        /// On Connect Main Packet
        /// </summary>
        LOWNET_CONNECT = 1,

        /// <summary>
        /// Player Data
        /// </summary>
        LOWNET_HANDSHAKE,

        /// <summary>
        /// Spawn Player
        /// </summary>
        LOWNET_PLAYER,

        /// <summary>
        /// Spawn Server Objeckts
        /// </summary>
        LOWNET_OBJECKTS,

        /// <summary>
        /// Send Player Sync Packet
        /// </summary>
        LOWNET_PLAYER_SYNC,

        /// <summary>
        /// Send Object Sync Packet
        /// </summary>
        LOWNET_OBJECKT_SYNC,

        /// <summary>
        /// Connect Udp Socket
        /// </summary>
        LOWNET_CONNECT_UDP
    }
}