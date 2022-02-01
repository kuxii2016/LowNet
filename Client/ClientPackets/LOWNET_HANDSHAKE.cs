using LowNet.Enums;
using LowNet.Unity3D;
using LowNet.Utils;

namespace LowNet.ClientPackets
{
    internal class LOWNET_HANDSHAKE
    {
        internal static void Readpacket(Store store)
        {
            int id = store.PopInt();
            if (id != Client.GetPlayerId)
            {
                Client.Log("Localplayer != Remoteplayer", LogType.LogWarning);
                return;
            }
            string Playername = store.PopAscii();
            if (Playername != Client.GetPlayername)
            {
                Client.Log("Localname != Remotename", LogType.LogWarning);
                return;
            }
            string GUID = store.PopAscii();
            int Playercount = store.PopInt();
            SendPacket(GUID, Playercount);
        }

        internal static void SendPacket(string Guid, int Count)
        {
            Store store = new Store((int)Packet.LOWNET_HANDSHAKE);
            store.PushAscii(Guid);
            store.PushInt(Client.GetPlayerId);
            store.PushByte(0x01); //Player
            store.PushInt(Count);
            Client.SendTCP(store);
        }
    }
}