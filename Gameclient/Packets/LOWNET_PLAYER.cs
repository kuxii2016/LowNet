/*  __                  _   __     __ 
   / /   ____ _      __/ | / /__  / /_
  / /   / __ \ | /| / /  |/ / _ \/ __/
 / /___/ /_/ / |/ |/ / /|  /  __/ /_  
/_____/\____/|__/|__/_/ |_/\___/\__/  
Simple Unity3D Solution ©2020 by Kuxii
*/
using LowNet.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace LowNet.Gameclient.Packets
{
    class LOWNET_PLAYER
    {
        internal static void Read(GameClient client, Store store)
        {
            int Checksum = store.PopInt();
            int clietnt = store.PopInt();
        }
    }
}
