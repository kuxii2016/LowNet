/*  __                  _   __     __ 
   / /   ____ _      __/ | / /__  / /_
  / /   / __ \ | /| / /  |/ / _ \/ __/
 / /___/ /_/ / |/ |/ / /|  /  __/ /_  
/_____/\____/|__/|__/_/ |_/\___/\__/  
Simple Unity3D Solution ©2022 by Kuxii
*/
using LowNet.Data;
using LowNet.Unity3D;
using UnityEngine;

namespace LowNet.Gameclient.Packets
{
    class LOWNET_PLAYER
    {
        internal static void Read(GameClient client, Store store)
        {
            int Checksum = store.PopInt();
            int clietnt = store.PopInt();
            bool Create = store.PopBool();
            string name = store.PopAscii();
            int playerId = store.PopInt();
            Vector3 pos = store.PopVector3();
            Quaternion rot = store.PopQuaternion();
            int model = store.PopInt();
            if (Create)
                ClientNetworkmanager.SpawnPlayer(model, playerId, pos, rot, name);
            else
                ClientNetworkmanager.RemovePlayer(playerId);
        }
    }
}