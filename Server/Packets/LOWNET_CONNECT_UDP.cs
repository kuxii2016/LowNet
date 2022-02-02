using LowNet.Enums;
using LowNet.Utils;

namespace LowNet.Server.Packets
{
    /// <summary>
    /// Connect the UDP Layer
    /// </summary>
    class LOWNET_CONNECT_UDP
    {
        internal static void Readpacket(Client client, Store store)
        {
        }

        internal static void Sendpacket(Client client)
        {
            Store store = new Store((int)Packet.LOWNET_CONNECT_UDP);
            store.PushInt(client.Connectionid);
            store.PushAscii(client.ConnectionGuid);
            Server.SendTCPData(client, store);
        }
    }
}