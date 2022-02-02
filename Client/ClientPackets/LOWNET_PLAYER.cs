using LowNet.Unity3D;
using LowNet.Utils;
using UnityEngine;

namespace LowNet.ClientPackets
{
    class LOWNET_PLAYER
    {
        //Can be used for Charmodel Switch
        internal static void Sendpacket()
        {
            //TODO: Add function to Switch Model
        }

        internal static void Readpacket(Store store)
        {
            bool myview = false;
            int player = store.PopInt();
            bool create = store.PopBool();
            //Playerinfos
            int PlayerId = store.PopInt();
            int ModelId = store.PopInt();
            string Playername = store.PopAscii();
            string Joined = store.PopAscii();
            Vector3 pos = store.PopVector3();
            Quaternion rot = store.PopQuaternion();

            if (player == PlayerId)
            {
                myview = true;
                Client.Log("Received Local Player.", Enums.LogType.LogDebug);
                //My Playerobject enable controll on this
            }
            else
            {
                Client.Log("Received Remote Player.", Enums.LogType.LogDebug);
            }
            if (create)
                ClientNetworkmanager.SpawnPlayer(ModelId, pos, rot, Playername, myview, PlayerId);
            else
                ClientNetworkmanager.DeSpawnPlayer(PlayerId);
        }
    }
}
