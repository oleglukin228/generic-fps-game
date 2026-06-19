using LlamAcademy.Guns.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableEnvironment : MonoBehaviour, IPickableInterface
{
    [SerializeField] protected Transform itemParent;
    [SerializeField] protected Transform PickupPoseParent;
    [SerializeField] protected Collider[] colliders;
    protected WeaponManagerFullBody whoIsHolding;
    protected InteractableActions interactableActions;
    public Transform ItemParent { get => itemParent; }
    public WeaponManagerFullBody WhoIsHolding { get { return whoIsHolding; } set { whoIsHolding = value; } }

    protected virtual void Start()
    {
        TryGetComponent<InteractableActions>(out interactableActions);
    }
    public virtual void InteractItem(Vector3 position, Vector3 direction)
    {
        //itemParent.position = Vector3.MoveTowards(itemParent.position, position, 1f);
    }

    public virtual void MoveItem(Transform position)
    {
        var mousePosition = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse Y"));
        if (interactableActions != null)
            interactableActions.CheckForChangeAction(mousePosition);
    }

    public virtual void Pickup(PlayerIK playerIK, WeaponManagerFullBody wpnManager)
    {
        foreach (var col in colliders)
        {
            col.enabled = false;
        }

        whoIsHolding = wpnManager;
        whoIsHolding.TargetItem = this;
        whoIsHolding.HoldingItem = this.transform;
        CursorController.SetInteractableActions(interactableActions);

        playerIK.SetupRig(PickupPoseParent, 0);
    }

    /*public virtual void PutDown(Transform slot, PlayerIK playerIK, WeaponManagerFullBody wpnManager)
    {
        return;
    }*/

    public virtual void DropItem(PlayerIK playerIK, WeaponManagerFullBody wpnManager)
    {
        foreach (var col in colliders)
        {
            col.enabled = true;
        }

        wpnManager.ChangeCursorPosition(transform.position);
        wpnManager.TargetItem = null;
        wpnManager.HoldingItem = null;
        CursorController.SetInteractableActions(null);
        whoIsHolding = null;

        playerIK.SetupRig(wpnManager.cursor, 0);
    }

    public void PickupItemFromSlot(InventorySlot slot, WeaponManagerFullBody wpnManager, PlayerIK playerIK)
    {
        throw new System.NotImplementedException();
    }
}
