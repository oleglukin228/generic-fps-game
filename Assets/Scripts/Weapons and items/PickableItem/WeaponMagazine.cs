using LlamAcademy.Guns.Demo;
using UnityEngine;

public class WeaponMagazine : PickableStuff
{
    [Header("In magazine slot position")]
    public Vector3 magPos;
    public Quaternion magRot;

    public GameObject bullet;
    public GameObject casingPrefab;
    public string[] compaibleWeapon;
    public int maxAmmo;
    int bulletsLeft;

    public int BulletsLeft {  get { return bulletsLeft; } set { bulletsLeft = value; } }

    private void Awake()
    {
        bulletsLeft = maxAmmo;
    }

    public override void DropItem(PlayerIK playerIK, WeaponManagerFullBody wpnManager)
    {
        base.DropItem(playerIK, wpnManager);
    }

    /*public override void PutDown(Transform slot, PlayerIK playerIK, WeaponManagerFullBody wpnManager)
    {
        if (slot.TryGetComponent<InventorySlot>(out var invSlot))
        {
            if (invSlot.OccupiedSlot != null) return;
            if (CheckItemСompatibility(invSlot))
            {
                invSlot.OccupiedSlot = wpnManager.HoldingItem;
                //this.transform.parent = wpnManager.TargetedInvSlot;
                //transform.SetLocalPositionAndRotation(invPos, invRot);
                foreach (var col in colliders)
                {
                    col.enabled = false;
                }
                wpnManager.HoldingItem = null;
                wpnManager.TargetItem = null;
                playerIK.SetupRig(wpnManager.itemRoot, 0);
            }
            else
            {
                DropItem(playerIK, wpnManager);
            }
        }
    }*/

    public override void MoveItem(Transform position)
    {
        if (InteractActions != null && InteractActions.enabled)
        {
            var mousePosition = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse Y"));
            InteractActions.CheckForChangeAction(mousePosition);
        }
        else
            itemParent.position = position.transform.position;
    }

    public override void Pickup(PlayerIK playerIK, WeaponManagerFullBody wpnManager)
    {
        WhoIsHolding = wpnManager;
        playerIK.SetupRig(PickupPoseParent, 0);
    }

    public override void PickupItemFromSlot(InventorySlot slot, WeaponManagerFullBody wpnManager, PlayerIK playerIK)
    {
        if (slot.OccupiedSlot != null)
        {
            wpnManager.HoldingItem = slot.OccupiedSlot;
            WhoIsHolding = wpnManager;
            playerIK.SetupRig(PickupPoseParent, 0);
            slot.OccupiedSlot = null;
            wpnManager.TargetItem = this;
        }
    }

    /*public override bool CheckItemСompatibility(WeaponManagerFullBody wpnManager)
    {
        if (!wpnManager.TargetedInvSlot.TryGetComponent<WeaponMagazineSlot>(out var slot)) return false;
        foreach (string weaponName in compaibleWeapon)
        {
            if (weaponName == slot.weapon.weaponName)
            {
                slot.magazine = this;
                InteractActions.enabled = true;
                return true;
            }
        }
        return false;
    }*/

    /*private void OnCollisionEnter(Collision collision)
    {
        if (whoIsHolding == null) return;
        Debug.Log(collision.collider.name);
        if (!collision.collider.GetComponent<InventorySlot>()) return; 
        if (collision.collider.transform.TryGetComponent<WeaponMagazineSlot>(out var magSlot))
        {
            whoIsHolding.TargetedInvSlot = collision.collider.transform;
            StartCoroutine(ApplyItemRotation(whoIsHolding, collision.collider.transform, magRot));
        }
        else
        {
            whoIsHolding.TargetedInvSlot = collision.collider.transform;
            StartCoroutine(ApplyItemRotation(whoIsHolding, collision.collider.transform, invRot));
        }
    }*/
}
