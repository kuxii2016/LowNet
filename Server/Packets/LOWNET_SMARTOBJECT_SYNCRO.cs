using LowNet.Data;
using LowNet.Packets;
using LowNet.Server.Data;
using LowNet.Unity3D;
using System;
using System.Collections.Generic;
using System.Text;

namespace LowNet.Server.Packets
{
    class LOWNET_SMARTOBJECT_SYNCRO
    {
        public static void Read(Client client, Store store)
        {
            ///Server Default
            int Checksum = store.PopInt();

            int TriggerClient = store.PopInt();
            int ListIndex = store.PopInt();
            int SyncCount = store.PopInt();
            string GUID = store.PopAscii();
            List<SyncObject> syncObj = new List<SyncObject>();

            for (int i = 0; i < SyncCount; i++)
            {
                SyncObject obj = new SyncObject();
                obj.LocalScale = store.PopVector3();
                obj.LocalPos = store.PopVector3();
                obj.LocalRot = store.PopQuaternion();
                obj.LocalState = store.PopBool();
                syncObj.Add(obj);
            }

            SmartObjectManager.mapObjects[ListIndex].ApplaySync(syncObj);
            Send(client, SmartObjectManager.mapObjects[ListIndex]);
        }

        public static void Send(Client client, SmartObject obj)
        {
            Store store = new Store(LowNetpacketOrder.LOWNET_SMARTOBJECT_SYNCRO);

            store.PushInt(client.ClientId);
            store.PushInt(obj.BuildingId);
            store.PushInt(obj.SyncObjects.Count);
            store.PushAscii(obj.GUID);
            for (int i = 0; i < obj.SyncObjects.Count; i++)
            {
                store.PushVector3(obj.SyncObjects[i].transform.localScale);
                store.PushVector3(obj.SyncObjects[i].transform.localPosition);
                store.PushQuaternion(obj.SyncObjects[i].transform.localRotation);
                store.PushBool((bool)obj.SyncObjects[i].activeSelf);
            }
            ServerNetworkmanager.NetworkManager.server.UDPLayer.SendAll(client, store);
        }
    }
}