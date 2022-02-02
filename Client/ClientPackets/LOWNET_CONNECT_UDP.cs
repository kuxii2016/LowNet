using LowNet.Enums;
using LowNet.Unity3D;
using LowNet.Utils;
using System.Net;

namespace LowNet.ClientPackets
{
    /// <summary>
    /// Connect the UDP Layer
    /// </summary>
    class LOWNET_CONNECT_UDP
    {
        internal static void Readpacket(Store store)
        {
            int clientId = store.PopInt();
            if (clientId != Client.GetPlayerId)
                return;
            store.PopAscii();
            Client.Log("Connecting UDP Layer", LogType.LogDebug);
            ClientNetworkmanager.Instance.udp.Connect(((IPEndPoint)ClientNetworkmanager.Instance.tcp.socket.Client.LocalEndPoint).Port);
        }

        internal static void Sendpacket()
        {
            Store store = new Store((int)Packet.LOWNET_CONNECT_UDP);
        }
    }
}