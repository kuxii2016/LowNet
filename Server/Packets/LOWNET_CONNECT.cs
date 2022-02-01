using LowNet.Enums;
using LowNet.Utils;

namespace LowNet.Server.Packets
{
    internal class LOWNET_CONNECT
    {
        internal static void Readpacket(Client client, Store store)
        {
            int clientId = store.PopInt();
            if (client.Connectionid != clientId)
                return;

            client.PlayerName = store.PopAscii();
            client.ConnectionGuid = store.PopAscii();
            LOWNET_HANDSHAKE.SendPacket(client);
        }

        internal static void SendPacket(Client client)
        {
            Store store = new Store((int)Packet.LOWNET_CONNECT);
            store.PushInt(client.Connectionid);
            store.PushAscii(Server.Servername);
            store.PushAscii(Server.Serverpassword);
            store.PushInt(Server.MaxPlayers);
            Server.SendTCPData(client, store);
        }
    }
}