using LowNet.Enums;
using LowNet.Unity3D;
using LowNet.Utils;
using UnityEngine;

namespace LowNet.ClientPackets
{
    class LOWNET_PLAYER_SYNC
    {
        internal static void Sendpacket(Vector3 position, Quaternion rotation, byte[] animatorSync)
        {
            Store store = new Store((int)Packet.LOWNET_PLAYER_SYNC);
            store.PushInt(Client.GetPlayerId);
            store.PushVector3(position);
            store.PushQuaternion(rotation);
            store.PushInt(animatorSync.Length);
            store.PushBytes(animatorSync);
            Client.SendUDP(store);
        }

        internal static void Readpacket(Store store)
        {
            int PlayerId = store.PopInt();
            Vector3 pos = store.PopVector3();
            Quaternion rot = store.PopQuaternion();
            int Lenght = store.PopInt();
            byte[] sync = store.PopBytes(Lenght);

            if (ClientNetworkmanager.Player[PlayerId] != null)
            {
                ClientNetworkmanager.Player[PlayerId].gameObject.transform.position = pos;
                ClientNetworkmanager.Player[PlayerId].gameObject.transform.rotation = rot;
                ClientNetworkmanager.Player[PlayerId].GetComponent<NetworkPlayercontroller>().SetSync(sync);
            }
        }
    }
}