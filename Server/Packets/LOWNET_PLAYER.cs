using LowNet.Enums;
using LowNet.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace LowNet.Server.Packets
{
    class LOWNET_PLAYER
    {
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
            Server.SendTCPData(toClient, store);
        }

        internal static void Readpacket(Client client, Store store)
        {
            //TODO: Read Model Switch and Sync it!
        }
    }
}