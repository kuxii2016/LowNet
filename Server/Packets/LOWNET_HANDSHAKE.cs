using LowNet.Enums;
using LowNet.Utils;
using System;

namespace LowNet.Server.Packets
{
    internal class LOWNET_HANDSHAKE
    {
        internal static void Readpacket(Client client, Store store)
        {
            string guid = store.PopAscii();
            int id = store.PopInt();
            if (id != client.Connectionid)
                return;
            byte order = store.PopByte();
            int count = store.PopInt();
            switch (order)
            {
                case 0:
                    client.SendPlayers();
                    break;
                case 1:
                    client.SendObjeckt();
                    break;
            }
        }

        internal static void SendPacket(Client client)
        {
            Store store = new Store((int)Packet.LOWNET_HANDSHAKE);
            store.PushInt(client.Connectionid);
            store.PushAscii(client.PlayerName);
            store.PushAscii(client.ConnectionGuid);
            store.PushInt(Server.Clients.Count);
            Server.SendTCPData(client, store);
        }
    }
}