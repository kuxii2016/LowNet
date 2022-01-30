/*  __                  _   __     __ 
   / /   ____ _      __/ | / /__  / /_
  / /   / __ \ | /| / /  |/ / _ \/ __/
 / /___/ /_/ / |/ |/ / /|  /  __/ /_  
/_____/\____/|__/|__/_/ |_/\___/\__/  
Simple Unity3D Solution ©2022 by Kuxii
*/
using LowNet.Data;
using System.Collections.Generic;
using UnityEngine;

namespace LowNet.Unity3D
{
    class SmartObjectManager : MonoBehaviour
    {
        public static Dictionary<int, SmartObject> mapObjects = new Dictionary<int, SmartObject>();
        public static SmartObjectManager Instance;
        public int ObjectCount = 0;

        public List<SmartObject> SmartObjects;

        public void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        public static int CreateMapobject(int ListIndex, int id, Vector3 pos, Quaternion rot, string Owner, ObjectType type, string Meta, string name)
        {
            SmartObject building = Instantiate(Instance.SmartObjects[id], pos, rot);
            building.name = name;
            building.AutoInsert = false;
            building.Metadata = Meta;
            building.Owner = Owner;
            building.Type = type;
            building.CollectionId = id;
            //TODO: Create Object Transform Root
            //building.transform.parent = GameManager.GetObjectTransform();
            building.BuildingId = mapObjects.Count + 1;
            mapObjects.Add(mapObjects.Count + 1, building);
            LowNetlog.PrintLog($"Create SmartObject: {ListIndex} ObjectType: {id} Pos=({pos.x},{pos.y},{pos.z})", Logmessage.Debug);
            Instance.ObjectCount++;
            return building.BuildingId;
        }

        public static void RemoveMapobject(int id)
        {
            if (id == -1)
                return;
            LowNetlog.PrintLog($"Remove SmartObject: {id} ObjectType: {mapObjects[id].CollectionId} Pos=({mapObjects[id].gameObject.transform.position.x},{mapObjects[id].gameObject.transform.position.y},{mapObjects[id].gameObject.transform.position.z})", Logmessage.Debug);
            GameObject building = mapObjects[id].gameObject;
            mapObjects.Remove(id);
            Instance.ObjectCount--;
            Destroy(building);
        }
    }
}
