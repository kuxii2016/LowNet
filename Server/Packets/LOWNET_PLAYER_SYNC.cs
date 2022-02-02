using LowNet.Enums;
using LowNet.Unity3D;
using LowNet.Utils;
using UnityEngine;

namespace LowNet.Server.Packets
{
    class LOWNET_PLAYER_SYNC
    {
        internal static void Readpacket(Client client, Store store)
        {
            int PlayerId = store.PopInt();
            Vector3 pos = store.PopVector3();
            Quaternion rot = store.PopQuaternion();
            int Lenght = store.PopInt();
            byte[] sync = store.PopBytes(Lenght);

            //Send first the Packets before we set it on server Player is Importanter as Server
            Sendpacket(client, pos, rot, sync);

            //Set Pos and Rot of this Player, new Player will know on Connect the Pos and Rot for Spawning
            Server.Clients[PlayerId].Session.Position = pos;
            Server.Clients[PlayerId].Session.Rotation = rot;
            ServerNetworkmanager.Player[PlayerId].gameObject.transform.position = pos;
            ServerNetworkmanager.Player[PlayerId].gameObject.transform.rotation = rot;
            ServerNetworkmanager.Player[PlayerId].GetComponent<NetworkPlayercontroller>().SetSync(sync);
            //TODO: Applay Sync for Animator
        }

        internal static void Sendpacket(Client client, Vector3 position, Quaternion rotation, byte[] animatorSync)
        {
            Store store = new Store((int)Packet.LOWNET_PLAYER_SYNC);
            store.PushInt(client.Connectionid);
            store.PushVector3(position);
            store.PushQuaternion(rotation);
            store.PushInt(animatorSync.Length);
            store.PushBytes(animatorSync);
            Server.SendTCPDataToAll(client, store);
        }
    }
}