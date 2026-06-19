using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class WeaponMagazineTube : InventorySlot
{
    public LoadableBullet currentBullet;
    public Transform bulletParent;
    public ParticleSystem casingParticle;
    public GameObject bullet;
    [SerializeField] private GunItem weapon;
    public int maxAmmo;
    int bulletsLeft;
    InteractableActions interactableActions;
    public int BulletsLeft { get { return bulletsLeft; } set { bulletsLeft = value; } }

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
        if (bulletsLeft == maxAmmo) return;
        foreach (var tag in compaibleItems)
        {
            if (shell.itemType == tag)
            {
                DisableCollider();
                shell.WhoIsHolding.IsBusy = true;
                shell.isPreparingToInteract = true;
                var targetPosition = interactableActions.items[0].tweens[0].transformStartPosition;
                CursorController.SetInteractableActions(interactableActions, true);
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
        foreach (var collision in item.colliders)
        {
            collision.enabled = false;
        }
        itemTransform.parent = bulletParent;
        currentBullet = item;
        currentBullet.InteractActions = interactableActions;
        currentBullet.isAttached = true;
        //CursorController.InteractAction = interactableActions;
        //currentBullet.InteractActions.CurrentItem = item;
    }

    public void ExitReloadState()
    {
        DisableCollider();
        currentBullet.isAttached = false;
        currentBullet.WhoIsHolding.IsBusy = false;
        currentBullet.InteractActions = null;
        CursorController.SetInteractableActions();
        currentBullet.isPreparingToInteract = false;
        //currentBullet.WhoIsHolding.ChangeCursorPosition(currentBullet.transform.position);
        foreach (var col in currentBullet.colliders)
        {
            col.enabled = true;
        }
        currentBullet = null;
        Invoke(nameof(EnableCollider), 1f);
    }

    public void InsertShell()
    {
        currentBullet.WhoIsHolding.HoldingItem = null;
        currentBullet.WhoIsHolding.TargetItem = null;
        currentBullet.WhoIsHolding.playerIK.SetupRig(currentBullet.WhoIsHolding.cursor, 0);

        CursorController.SetInteractableActions();
        currentBullet.InteractActions = null;
        currentBullet.isPreparingToInteract = false;
        DisableCollider();
        bulletsLeft++;

        currentBullet.WhoIsHolding.IsBusy = false;
        currentBullet.WhoIsHolding = null;
        
        if (maxAmmo > 1)
        {
            Destroy(currentBullet.gameObject);
            currentBullet = null;
        } 

        interactableActions.ResetActions();
        Invoke(nameof(EnableCollider), 0.1f);
    }

    public void ChangeHandPose(Transform rigParent)
    {
        currentBullet.ChangeHandPose(rigParent);
    }

    public void ParentBulletToBolt(Transform bolt)
    {
        currentBullet.transform.SetParent(bolt);
    }

    public void DestroyCurrentBullet(AudioClip sound)
    {
        if (currentBullet != null)
        {
            if (sound != null) AudioSource.PlayClipAtPoint(sound, transform.position);
            Destroy(currentBullet.gameObject);
            currentBullet = null;
        }
    }
}
