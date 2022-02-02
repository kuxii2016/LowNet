using LowNet.Enums;
using LowNet.Utils;

namespace LowNet.Server.Packets
{
    /// <summary>
    /// Player Sending
    /// </summary>
    class LOWNET_PLAYER
    {
        /// <summary>
        /// Send Player to Player
        /// </summary>
        /// <param name="toClient">Player was is Receiving</param>
        /// <param name="client">Player was should send</param>
        /// <param name="Creating">Create or Delete</param>
        internal static void SendPacket(Client toClient, Client client, bool Creating = false)
        {
            Store store = new Store((int)Packet.LOWNET_PLAYER);
            store.PushInt(toClient.Connectionid);
            store.PushBool(Creating);
            //Send Playerinfo
            store.PushInt(client.Connectionid);
            store.PushInt(client.Session.ModelId);
            store.PushAscii(client.Session.Playername);
            store.PushAscii(client.Session.Joined.ToString());
            store.PushVector3(client.Session.Position);
            store.PushQuaternion(client.Session.Rotation);
            Server.Debug($"Send Player: {client.Connectionid}:{client.Session.ModelId},{client.Session.Playername},{client.Session.Position} to Player: {toClient.Connectionid}", Server.Instance);
            Server.SendTCPData(toClient, store);
        }

        internal static void Readpacket(Client client, Store store)
        {
            //TODO: Read Model Switch and Sync it!
        }
    }
}