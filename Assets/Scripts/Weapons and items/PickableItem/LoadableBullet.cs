using LlamAcademy.Guns.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadableBullet : PickableStuff
{
    public string[] compaibleWeapon;
    public bool isAttached;
    public bool isPreparingToInteract;
    public override void MoveItem(Transform position)
    {
        if (InteractActions != null && isAttached)
        {
            var mousePosition = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse Y"));
            InteractActions.CheckForChangeAction(mousePosition);
        }
        else if (!isPreparingToInteract)
        {
            itemParent.position = position.transform.position;
        }
    }

    public override void Pickup(PlayerIK playerIK, WeaponManagerFullBody wpnManager)
    {
        base.Pickup(playerIK, wpnManager);
    }

    public override void DropItem(PlayerIK playerIK, WeaponManagerFullBody wpnManager)
    {
        base.DropItem(playerIK, wpnManager);
        if (InteractActions != null)
        {
            InteractActions.GetComponent<WeaponMagazineTube>().EnableCollider();
            InteractActions.ResetActions();
            InteractActions = null;
            isAttached = false;
        }
    }

    public override void PickupItemFromSlot(InventorySlot slot, WeaponManagerFullBody wpnManager, PlayerIK playerIK)
    {
        if (slot.OccupiedSlot != null)
        {
            wpnManager.HoldingItem = slot.OccupiedSlot;
            WhoIsHolding = wpnManager;
            playerIK.SetupRig(PickupPoseParent, 0);
            InteractActions = slot.GetComponent<InteractableActions>();
            slot.OccupiedSlot = null;
            wpnManager.TargetItem = this;
        }
    }

    public void ChangeHandPose(Transform rigParent)
    {
        WhoIsHolding.playerIK.SetupRig(rigParent, 0);
    }
}
