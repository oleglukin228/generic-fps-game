using LlamAcademy.Guns.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoHolder : PickableStuff, IPickableInterface
{
    public List<LoadableBullet> ammoSlots;
    public override void PickupItemFromSlot(InventorySlot slot, WeaponManagerFullBody wpnManager, PlayerIK playerIK)
    {
        if (wpnManager.HeldWeapon != null && wpnManager.Reloading)
        {
            if (ammoSlots.Count > 0)
            {
                ammoSlots[0].Pickup(playerIK, wpnManager);
                ammoSlots.RemoveAt(0);
            }
        }
        else
        {
            if (slot.poungeCap) slot.poungeCap.SetActive(true);
            wpnManager.HoldingItem = slot.OccupiedSlot;
            foreach (var col in colliders)
            {
                col.enabled = true;
            }
            WhoIsHolding = wpnManager;
            playerIK.SetupRig(PickupPoseParent, 0);
            slot.OccupiedSlot = null;
            wpnManager.TargetItem = this;
        }
    }

    public override void Pickup(PlayerIK playerIK, WeaponManagerFullBody wpnManager)
    {
        //if (wpnManager.HeldWeapon && wpnManager.HeldWeapon.Reloading) return;
        base.Pickup(playerIK, wpnManager);
    }
}
