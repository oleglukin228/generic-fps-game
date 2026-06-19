using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PrefabPlacer : MonoBehaviour
{
    //[SerializeField]
    //private GameObject itemPrefab;

    public List<GameObject> PlaceEnemies(List<EnemyPlacementData> enemyPlacementData, ItemPlacementHelper itemPlacementHelper, Transform parents)
    {
        List<GameObject> placedObjects = new List<GameObject>();

        foreach (var placementData in enemyPlacementData)
        {
            for (int i = 0; i < placementData.Quantity; i++)
            {
                Vector3? possiblePlacementSpot = itemPlacementHelper.GetItemPlacementPosition(
                    PlacementType.OpenSpace,
                    100,
                    placementData.enemySize,
                    false
                    );
                if (possiblePlacementSpot.HasValue)
                {

                    placedObjects.Add(CreateObject(placementData.enemyPrefab, possiblePlacementSpot.Value + new Vector3(0.5f, 0, 0.5f), parents)); //Instantiate(placementData.enemyPrefab,possiblePlacementSpot.Value + new Vector2(0.5f, 0.5f), Quaternion.identity)
                }
            }
        }
        return placedObjects;
    }

    public List<GameObject> PlaceAllItems(List<ItemPlacementData> itemPlacementData, ItemPlacementHelper itemPlacementHelper, Transform parent)
    {
        List<GameObject> placedObjects = new List<GameObject>();

        IEnumerable<ItemPlacementData> sortedList = new List<ItemPlacementData>(itemPlacementData).OrderByDescending(placementData => placementData.itemData.size.x * placementData.itemData.size.z);

        foreach (var placementData in sortedList)
        {
            for (int i = 0; i < placementData.Quantity; i++)
            {
                Vector3? possiblePlacementSpot = itemPlacementHelper.GetItemPlacementPosition(
                    placementData.itemData.placementType,
                    100,
                    placementData.itemData.size,
                    placementData.itemData.addOffset);


                if (possiblePlacementSpot.HasValue)
                {
                    //placedObjects.Add(PlaceItem(placementData.itemData, possiblePlacementSpot.Value, parent));
                    placedObjects.Add(PlaceItem(placementData.itemData, possiblePlacementSpot.Value + new Vector3(1.5f, 0, 1.5f), parent));
                }
            }
        }
        return placedObjects;
    }
    private GameObject PlaceItem(ItemData item, Vector3 placementPosition, Transform parent)
    {
        GameObject newItem = Instantiate(item.prefab, placementPosition, Quaternion.identity, parent);
        //GameObject newItem = CreateObject(itemPrefab, placementPosition);
        //newItem.GetComponent<Item>().Initialize(item);
        return newItem;
    }

    public GameObject CreateObject(GameObject prefab, Vector3 placementPosition, Transform parent)
    {
        if (prefab == null)
            return null;
        GameObject newItem;
        newItem = Instantiate(prefab, placementPosition, Quaternion.identity, parent);
        /*if (Application.isPlaying)
        {
            newItem = Instantiate(prefab, placementPosition, Quaternion.identity, parent);
        }
        else
        {
        #if UNITY_EDITOR
            newItem = PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;
            newItem.transform.position = placementPosition;
            newItem.transform.rotation = Quaternion.identity;
        #endif
        }*/

        return newItem;
    }
}
