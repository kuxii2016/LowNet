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
    class LOWNET_CONNECT_UDP
    {
        internal static void Read(Client client, Store store)
        {
            int Checksum = store.PopInt();
            int clietnt = store.PopInt();
        }
    }
}
