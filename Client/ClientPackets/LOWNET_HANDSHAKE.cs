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
                return;
            string Playername = store.PopAscii();
            if (Playername != Client.GetPlayername)
                return;
            string GUID = store.PopAscii();
            int Playercount = store.PopInt();
        }

        internal static void SendPacket(string Guid)
        {
        }
    }
}