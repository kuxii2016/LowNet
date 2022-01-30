/*  __                  _   __     __ 
   / /   ____ _      __/ | / /__  / /_
  / /   / __ \ | /| / /  |/ / _ \/ __/
 / /___/ /_/ / |/ |/ / /|  /  __/ /_  
/_____/\____/|__/|__/_/ |_/\___/\__/  
Simple Unity3D Solution ©2022 by Kuxii
*/
using LowNet.Data;
using LowNet.Gameclient.Packets;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LowNet.Unity3D
{
    class SmartObject : MonoBehaviour
    {
        /// <summary>
        /// Stored Index
        /// </summary>
        public int CollectionId;
        /// <summary>
        /// Buildings Id
        /// </summary>
        public int BuildingId;
        /// <summary>
        /// Building Owner
        /// </summary>
        public string Owner;
        /// <summary>
        /// Metadata Most for Signs
        /// </summary>
        public string Metadata;
        /// <summary>
        /// Object Type
        /// </summary>
        public ObjectType Type;
        /// <summary>
        /// Objects was should sync the Network
        /// </summary>
        public List<GameObject> SyncObjects;
        public List<SyncObject> SyncObjectsOld;
        /// <summary>
        /// Will Insert default Editor Placed Objects
        /// </summary>
        public string GUID;
        /// <summary>
        /// Insert On Start Automatisch to MabObjects
        /// </summary>
        [Header("Will Set on Start to Synclist"), Tooltip("Use this when you Have Serverside Objects but not on Client")]
        public bool AutoInsert = false;
        /// <summary>
        /// Syncro Rate
        /// </summary>
        [Range(.001f, 10f)]
        public float SyncRate = .75f;
        /// <summary>
        /// Has Meta Text
        /// </summary>
        public bool HasMetatext = false;
        [Header("Controll Flag"), Tooltip("Define Buldtype Server or Client")]
        public OwnerType ownerType;

        public void Start()
        {
            SyncObjectsOld = new List<SyncObject>();
            for (int i = 0; i < SyncObjects.Count; i++)
            {
                SyncObjectsOld.Add(new SyncObject(SyncObjects[i]));
            }

            if (AutoInsert && ownerType == OwnerType.Client)
            {
                if (SmartObjectManager.Instance != null)
                {
                    SmartObjectManager.CreateMapobject(CollectionId, BuildingId, this.gameObject.transform.position, this.gameObject.transform.rotation, Owner, (ObjectType)Type, Metadata, this.gameObject.name);
                    Destroy(this.gameObject);
                }
            }
            else if (AutoInsert && ownerType == OwnerType.Server)
            {
                if (SmartObjectManager.Instance != null)
                {
                    int builing = SmartObjectManager.CreateMapobject(CollectionId, BuildingId, this.gameObject.transform.position, this.gameObject.transform.rotation, Owner, (ObjectType)Type, Metadata, this.gameObject.name);
                    Destroy(this.gameObject);
                    LowNet.Server.Packets.LOWNET_OBJECT.Send(new Server.Data.Client(-1, ServerNetworkmanager.NetworkManager.server), BuildingId, this.gameObject.transform.position, this.gameObject.transform.rotation, Owner, (ObjectType)Type, Metadata, this.gameObject.name);
                }
            }
        }

        public void Update()
        {
            if (ownerType == OwnerType.Client)
            {
                for (int i = 0; i < SyncObjects.Count; i++)
                {
                    ///Will Sync on Pos Change
                    if (Vector3.Distance(SyncObjects[i].transform.localPosition, SyncObjectsOld[i].LocalPos) >= SyncRate)
                    {
                        SyncObjectsOld[i].LocalPos = SyncObjects[i].transform.localPosition;
                        LOWNET_SMARTOBJECT_SYNCRO.Send(this);
                    }

                    ///Will sync when Gameobjects Change state Off/On
                    if (SyncObjects[i].activeSelf != SyncObjectsOld[i].LocalState)
                    {
                        SyncObjectsOld[i].LocalState = SyncObjects[i].activeSelf;
                        LOWNET_SMARTOBJECT_SYNCRO.Send(this);
                    }

                    ///Will Objects Sync when is Rotated
                    if (SyncObjects[i].transform.localRotation != SyncObjectsOld[i].LocalRot)
                    {
                        SyncObjectsOld[i].LocalRot = SyncObjects[i].transform.localRotation;
                        LOWNET_SMARTOBJECT_SYNCRO.Send(this);
                    }

                    ///Will sync Object Size
                    if (SyncObjects[i].transform.localScale != SyncObjectsOld[i].LocalScale)
                    {
                        SyncObjectsOld[i].LocalScale = SyncObjects[i].transform.localScale;
                        LOWNET_SMARTOBJECT_SYNCRO.Send(this);
                    }
                }
            }
        }

        public void ApplaySync(List<SyncObject> obj)
        {
            ///We must Set for Old Object without Client -> Server -> Otherclient -> Server so will this goe a while..
            for (int i = 0; i < obj.Count; i++)
            {
                SyncObjects[i].transform.localPosition = obj[i].LocalPos;
                SyncObjectsOld[i].LocalScale = obj[i].LocalPos;

                SyncObjects[i].transform.localRotation = obj[i].LocalRot;
                SyncObjectsOld[i].LocalRot = obj[i].LocalRot;

                SyncObjects[i].transform.localScale = obj[i].LocalScale;
                SyncObjectsOld[i].LocalScale = obj[i].LocalScale;

                SyncObjects[i].SetActive(obj[i].LocalState);
                SyncObjectsOld[i].LocalState = obj[i].LocalState;
            }
        }
    }
}