using System.Collections;
using System.Collections.Generic;
using TeoGames.Mesh_Combiner.Scripts.Extension;
using Unity.VisualScripting;
using UnityEngine;

public class BreakActionBulletTube : InventorySlot
{
    public BreakActionBarrel barrel;
    public LoadableBullet currentBullet;
    public Transform bulletParent;
    public Transform casingPivot;
    public BreakActionGunItem weapon;
    InteractableActions interactableActions;
    public InteractableActions InteractActions {  get { return interactableActions; } }

    private void Start()
    {
        if (slotCollider == null) slotCollider = GetComponent<BoxCollider>();
        if (slotPath == null) slotPath = GetComponent<Transform>();
        interactableActions = GetComponent<InteractableActions>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.TryGetComponent<LoadableBullet>(out var shell)) return;
        if (shell.WhoIsHolding == null) return;
        if (shell.WhoIsHolding.HoldingItem == null) return;
        if (OccupiedSlot != null) return;
        foreach (var tag in compaibleItems)
        {
            if (shell.itemType == tag)
            {
                DisableCollider();
                shell.WhoIsHolding.IsBusy = true;
                shell.isPreparingToInteract = true;
                var targetPosition = interactableActions.items[0].tweens[0].transformStartPosition;
                CursorController.SetInteractableActions(interactableActions, true);
                foreach (var collision in shell.colliders)
                {
                    collision.enabled = false;
                }
                shell.StartLerpCoroutine((t) => {
                    shell.transform.SetPositionAndRotation(
                        Vector3.Lerp(shell.transform.position, targetPosition.position, t),
                        Quaternion.Lerp(shell.transform.rotation, targetPosition.rotation, t));
                }, 0.25f, () => ApplyShellRotation(shell.transform, this.transform, shell));

                break;
            }
        }
    }

    void ApplyShellRotation(Transform itemTransform, Transform parent, LoadableBullet item)
    {
        itemTransform.parent = bulletParent;
        currentBullet = item;
        currentBullet.InteractActions = interactableActions;
        currentBullet.InteractActions.ResetActions();
        currentBullet.isAttached = true;
    }

    public void ExitReloadState()
    {
        DisableCollider();
        currentBullet.isAttached = false;
        CursorController.SetInteractableActions();
        currentBullet.WhoIsHolding.IsBusy = false;
        currentBullet.isPreparingToInteract = false;
        currentBullet.WhoIsHolding.ChangeCursorPosition(currentBullet.transform.position);
        currentBullet.InteractActions.ResetActions();
        currentBullet.InteractActions = null;
        foreach (var col in currentBullet.colliders)
        {
            col.enabled = true;
        }
        currentBullet = null;
        Invoke(nameof(EnableCollider), 1f);
    }

    public void InsertShell()
    {
        currentBullet.WhoIsHolding.IsBusy = false;
        currentBullet.WhoIsHolding.ChangeCursorPosition(currentBullet.transform.position);
        currentBullet.isPreparingToInteract = false;
        DisableCollider();

        PutDownToSlot(currentBullet.transform, currentBullet.WhoIsHolding);
        currentBullet.InteractActions = null;
        CursorController.SetInteractableActions();
        //Invoke(nameof(EnableCollider), 0.1f);
        currentBullet = null;
    }

    public override void PutDownToSlot(Transform holdingItem, WeaponManagerFullBody wpnManager)
    {
        if (OccupiedSlot != null) return;
        foreach (var tag in compaibleItems)
        {
            if (currentBullet.itemType == tag)
            {
                wpnManager.HoldingItem = null;
                wpnManager.TargetItem = null;
                currentBullet.WhoIsHolding = null;
                //Destroy(currentBullet.gameObject);
                wpnManager.playerIK.SetupRig(wpnManager.cursor, 0);
                OccupiedSlot = holdingItem;
                barrel.LoadedTubes.Add(this);
                break;
            }
        }

    }

    public void ChangeHandPose(Transform rigParent)
    {
        currentBullet.ChangeHandPose(rigParent);
    }
}
