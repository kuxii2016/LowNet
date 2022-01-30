/*  __                  _   __     __ 
   / /   ____ _      __/ | / /__  / /_
  / /   / __ \ | /| / /  |/ / _ \/ __/
 / /___/ /_/ / |/ |/ / /|  /  __/ /_  
/_____/\____/|__/|__/_/ |_/\___/\__/  
Simple Unity3D Solution ©2022 by Kuxii
*/
using LowNet.Data;
using LowNet.Packets;
using LowNet.Server.Data;
using LowNet.Unity3D;
using UnityEngine;

namespace LowNet.Server.Packets
{
    class LOWNET_OBJECT
    {
        internal static void Read(Client client, Store store)
        {
            int Checksum = store.PopInt();
            int clietnt = store.PopInt();
            int ListIndex = store.PopInt();
            int BuildingId = store.PopInt();
            Vector3 Pos = store.PopVector3();
            Quaternion Rot = store.PopQuaternion();
            string Owner = store.PopAscii();
            int Type = store.PopInt();
            string Meta = store.PopAscii();
            string Name = store.PopAscii();
            bool Create = store.PopBool();

            if (Create)
            {
                if (SmartObjectManager.Instance != null)
                    SmartObjectManager.CreateMapobject(ListIndex, BuildingId, Pos, Rot, Owner, (ObjectType)Type, Meta, Name);

                Send(client, BuildingId, Pos, Rot, Owner, (ObjectType)Type, Meta, Name, Create);
            }
            else
            {
                SmartObjectManager.RemoveMapobject(ListIndex);
            }
        }

        public static void Send(Client client, int Buildingid, Vector3 pos, Quaternion rot, string Meta, ObjectType ownerType, string Metadata, string Name, bool Create)
        {
            Store store = new Store(LowNetpacketOrder.LOWNET_OBJECT);
            store.PushInt(client.ClientId);
            store.PushInt(Buildingid);
            store.PushVector3(pos);
            store.PushQuaternion(rot);
            store.PushAscii(Meta);//Owner
            store.PushInt((int)ownerType);
            store.PushAscii(Metadata);
            store.PushAscii(Name);
            store.PushBool(Create);
            LowNetServer.server.TCPLayer.SendAll(client, store);
        }
    }
}
