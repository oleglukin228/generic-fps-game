using UnityEngine;

public class WeaponMagazineSlot : InventorySlot
{
    [SerializeField] private GunItem weapon;
    public Transform magazineParent;
    public PickableStuffInteractable magazine;
    InteractableActions interactableActions;
    private void Start()
    {
        if (slotCollider == null) slotCollider = GetComponent<BoxCollider>();
        if (slotPath == null) slotPath = GetComponent<Transform>();
        interactableActions = GetComponent<InteractableActions>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.TryGetComponent<PickableStuffInteractable>(out var mag)) return;
        if (mag.WhoIsHolding == null) return;
        if (mag.WhoIsHolding.HoldingItem == null) return;
        if (OccupiedSlot != null) return;
        if (!CheckItemСompatibility(mag)) return;
        if (!CheckMagСompatibility(mag)) return;

        DisableCollider();
        mag.WhoIsHolding.IsBusy = true;
        mag.TargetSlot = this;
        mag.transform.parent = magazineParent;
        CursorController.SetInteractableActions(interactableActions);
        var targetPosition = interactableActions.items[0].tweens[0].transformStartPosition;

        mag.StartLerpCoroutine((t) => {
            mag.transform.SetPositionAndRotation(
                Vector3.Lerp(mag.transform.position, targetPosition.position, t), 
                Quaternion.Lerp(mag.transform.rotation, targetPosition.rotation, t));
        }, 0.25f, () => ApplyMagRotation(mag.transform, this.transform, mag));
    }

    void ApplyMagRotation(Transform itemTransform, Transform parent, PickableStuffInteractable item)
    {
        foreach (var collision in item.colliders)
        {
            collision.enabled = false;
        }
        magazine = item;
        magazine.InteractActions = interactableActions;
        magazine.isAttached = true;
    }

    public void ExitReloadState()
    {
        magazine.transform.parent = magazine.WhoIsHolding.transform;
        DisableCollider();
        Invoke(nameof(EnableCollider), 1f);
        magazine.InteractActions.ResetActions();
        magazine.InteractActions = null;
        magazine.StartLerpCoroutine(
            (t) => magazine.transform.position = Vector3.Lerp(magazine.transform.position, magazine.WhoIsHolding.cursor.position, t), 0.25f, 
            () => {
                magazine.isAttached = false;
                magazine.WhoIsHolding.IsBusy = false;
                //magazine.WhoIsHolding.ChangeCursorPosition(magazine.transform.position);
                CursorController.SetInteractableActions(null);
                magazine.TargetSlot = null;
                transform.gameObject.layer = LayerMask.NameToLayer("Insertable");
                foreach (var col in magazine.colliders)
                {
                    col.enabled = true;
                }
            });
    }
    public void InsertMag()
    {
        magazine.TargetSlot = null;
        magazine.WhoIsHolding.IsBusy = false;
        magazine.InteractActions = null;
        //magazine.WhoIsHolding.ChangeCursorPosition(magazine.transform.position);
        CursorController.SetInteractableActions(null);
        transform.gameObject.layer = LayerMask.NameToLayer("UI");
        DisableCollider();
        //magazine.PutDown(this.transform, magazine.whoIsHolding.playerIK, magazine.whoIsHolding);
        PutDownToSlot(magazine.transform, magazine.WhoIsHolding);
        Invoke(nameof(EnableCollider), 1f);
    }

    public override void PutDownToSlot(Transform holdingItem, WeaponManagerFullBody wpnManager)
    {
        if (OccupiedSlot != null) return;
        var mag = holdingItem.GetComponent<PickableStuffInteractable>();

        OccupiedSlot = holdingItem;
        //holdingItem.parent = this.transform;
        holdingItem.SetLocalPositionAndRotation(mag.magPos, mag.magRot);
        foreach (var col in mag.colliders)
        {
            col.enabled = false;
        }
        wpnManager.HoldingItem = null;
        wpnManager.TargetItem = null;
        CursorController.SetInteractableActions(null);
        mag.WhoIsHolding = null;
        wpnManager.playerIK.SetupRig(wpnManager.cursor, 0);
    }

    public override void PickupItemFromSlot(WeaponManagerFullBody wpnManager)
    {
        wpnManager.IsBusy = true;
        wpnManager.HoldingItem = OccupiedSlot;
        magazine.WhoIsHolding = wpnManager;
        wpnManager.playerIK.SetupRig(magazine.PickupPoseParent, 0);
        magazine.InteractActions = interactableActions;
        CursorController.SetInteractableActions(interactableActions);
        OccupiedSlot = null;
        wpnManager.TargetItem = magazine;
    }

    public override bool CheckItemСompatibility(PickableStuff item)
    {
        if (item == null) return false;
        foreach (var tag in compaibleItems)
        {
            if (tag.Equals(item.itemType)) return true;
        }
        return false;
    }

    public virtual bool CheckMagСompatibility(PickableStuffInteractable mag)
    {
        if (mag == null) return false;
        foreach (var tag in mag.compaibleWeapon)
        {
            if (tag.Equals(weapon.weaponName)) return true;
        }
        return false;
    }
}
