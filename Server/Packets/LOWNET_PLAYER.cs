/*  __                  _   __     __ 
   / /   ____ _      __/ | / /__  / /_
  / /   / __ \ | /| / /  |/ / _ \/ __/
 / /___/ /_/ / |/ |/ / /|  /  __/ /_  
/_____/\____/|__/|__/_/ |_/\___/\__/  
Simple Unity3D Solution ©2022 by Kuxii
*/
using LowNet.Data;
using LowNet.Server.Data;

namespace LowNet.Server.Packets
{
    class LOWNET_PLAYER
    {
        /// <summary>
        /// Destroy Player
        /// </summary>
        /// <param name="client"></param>
        /// <param name="store"></param>
        internal static void Read(Client client, Store store)
        {
            int Checksum = store.PopInt();
            int clietnt = store.PopInt();
        }

        /// <summary>
        /// Spawn Used from Disconnect
        /// </summary>
        internal static void Send(NetworkPlayer player, bool Create = true, Client client = null)
        {
            int PlayerId = -1;
            if (client != null)
                PlayerId = client.ClientId;

            Store store = new Store(LowNet.Packets.LowNetpacketOrder.LOWNET_PLAYER);
            store.PushInt(PlayerId);
            store.PushBool(Create);
            store.PushAscii(player.Name);
            store.PushInt(player.PlayerId);
            store.PushVector3(player.PlayerPos);
            store.PushQuaternion(player.PlayerRot);
            store.PushInt(player.Unity3dModel);

            LowNetServer.server.TCPLayer.SendAll(client, store);
        }

        /// <summary>
        /// Spawn New Player
        /// </summary>
        internal static void Create(NetworkPlayer player, bool Create = true, Client client = null)
        {
            int PlayerId = -1;
            if (client != null)
                PlayerId = client.ClientId;

            Store store = new Store(LowNet.Packets.LowNetpacketOrder.LOWNET_PLAYER);
            store.PushInt(PlayerId);
            store.PushBool(Create);
            store.PushAscii(player.Name);
            store.PushInt(player.PlayerId);
            store.PushVector3(player.PlayerPos);
            store.PushQuaternion(player.PlayerRot);
            store.PushInt(player.Unity3dModel);

            LowNetServer.server.TCPLayer.Send(client, store);
        }
    }
}
