using UnityEngine;

[CreateAssetMenu(menuName = "Generic fps/Procedural generation/Item data")]
public class ItemData : ScriptableObject
{
    public GameObject prefab;
    public Vector3Int size = new Vector3Int(1, 0, 1);
    public PlacementType placementType;
    public bool addOffset = false;
    public int health = -1;
    public bool nonDestructible = true;
}

[System.Serializable]
public class ItemPlacementInfo
{
    public int minQuantity = 5;
    public int maxQuantity = 10;
    public ItemData itemData;
}
