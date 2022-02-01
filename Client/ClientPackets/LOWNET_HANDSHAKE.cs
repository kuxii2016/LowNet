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
            string Playername = store.PopAscii();
            string GUID = store.PopAscii();
            int Playercount = store.PopInt();
            Client.Log("Server Has: " + Playercount + " Request Player spawn", LogType.LogDebug);
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