using AudioSystem;
using LlamAcademy.Guns.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PickableStuffInteractable : PickableStuff
{
    [Header("In magazine slot position")]
    public Vector3 magPos;
    public Quaternion magRot;

    public GameObject bullet;
    public GameObject casingPrefab;
    public string[] compaibleWeapon;
    public int maxAmmo;
    public GameObject BulletVusial;
    int bulletsLeft;
    InventorySlot targetSlot;
    public InventorySlot TargetSlot {  get { return targetSlot; } set { targetSlot = value; } }
    bool isPrepareToInteract => targetSlot != null;
    public int BulletsLeft { get { return bulletsLeft; } set { bulletsLeft = value; } }
    public bool isAttached;
    private void Awake()
    {
        bulletsLeft = maxAmmo;
    }
    public override void MoveItem(Transform position)
    {
        if (interactableActions != null && isAttached)
        {
            var mousePosition = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse Y"));
            interactableActions.CheckForChangeAction(mousePosition);
        }
        else if (!isPrepareToInteract)
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
        if (interactableActions != null)
        {
            //whoIsHolding.IsBusy = false;
            CursorController.SetInteractableActions(null);
            interactableActions.GetComponent<WeaponMagazineSlot>().EnableCollider();
            interactableActions.ResetActions();
            interactableActions = null;
            isAttached = false;
        }
    }

    public override void PickupItemFromSlot(InventorySlot slot, WeaponManagerFullBody wpnManager, PlayerIK playerIK)
    {
        if (slot.OccupiedSlot != null)
        {
            if (slot.poungeCap) slot.poungeCap.SetActive(true);
            wpnManager.HoldingItem = slot.OccupiedSlot;
            foreach (var col in colliders)
            {
                col.enabled = true;
            }
            WhoIsHolding = wpnManager;
            playerIK.SetupRig(PickupPoseParent, 0);
            interactableActions = slot.GetComponent<InteractableActions>();
            CursorController.SetInteractableActions(interactableActions);
            slot.OccupiedSlot = null;
            wpnManager.TargetItem = this;
            SoundManager.Instance.CreateSoundBuilder().Play(wpnManager.ctx.PlayerSFX.pickupSound);
            //whoIsHolding.IsBusy = true;
        }
    }

    /*public override bool CheckItemСompatibility(InventorySlot slot)
    {
        foreach (var tag in slot.compaibleItems)
        {
            if (itemType == tag) return true;
        }
        return false;
    }*/
}
