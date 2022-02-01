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
            byte order = store.PopByte();
            int count = store.PopInt();
            Server.Debug($"Player: {client.PlayerName} Request Order: " + order, Server.Instance);
            switch (order)
            {
                case 0x01:
                    client.SendPlayers();
                    break;
                case 0x02:
                    client.SendObjeckt();
                    break;
                case 0x03:
                    throw new NotImplementedException();
                default:
                    //Its can not be Empty but Code analystic tool sayed it, So we make the Default xD
                    Server.Error("Invalid Request Type: " + order, "'LOWNET_HANDSHAKE", Server.Instance);
                    break;
            }
        }

        internal static void SendPacket(Client client)
        {
            Store store = new Store((int)Packet.LOWNET_HANDSHAKE);
            store.PushInt(client.Connectionid);
            store.PushAscii(client.PlayerName);
            store.PushAscii(client.ConnectionGuid);
            store.PushInt(20);
            Server.SendTCPData(client, store);
        }
    }
}