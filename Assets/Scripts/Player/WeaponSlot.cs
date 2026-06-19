using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlot : InventorySlot
{
    public override void PutDownToSlot(Transform holdingItem, WeaponManagerFullBody wpnManager)
    {
        holdingItem.transform.parent = slotPath;
        base.PutDownToSlot(holdingItem, wpnManager);
    }
}
