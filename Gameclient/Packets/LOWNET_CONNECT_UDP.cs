/*  __                  _   __     __ 
   / /   ____ _      __/ | / /__  / /_
  / /   / __ \ | /| / /  |/ / _ \/ __/
 / /___/ /_/ / |/ |/ / /|  /  __/ /_  
/_____/\____/|__/|__/_/ |_/\___/\__/  
Simple Unity3D Solution ©2022 by Kuxii
*/
using LowNet.Data;

namespace LowNet.Gameclient.Packets
{
    class LOWNET_CONNECT_UDP
    {
        internal static void Read(GameClient client, Store store)
        {
            int Checksum = store.PopInt();
            int clietnt = store.PopInt();
        }
    }
}
