using LowNet.Enums;
using LowNet.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace LowNet.Server.Packets
{
    class LOWNET_CONNECT
    {
        internal static void Readpacket(Client client, Store store)
        {
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