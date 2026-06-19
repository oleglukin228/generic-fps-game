using AudioSystem;
using LlamAcademy.Guns.Demo;
using System;
using System.Collections;
using UnityEngine;

public class PickableStuff : MonoBehaviour, IPickableInterface
{
    public string itemType;
    public Transform itemParent;
    public Transform PickupPoseParent;

    public Collider[] colliders;

    [Header("In inventory position")]
    public Vector3 invPos;
    public Quaternion invRot;

    [Header("In hand position")]
    public Vector3 handPos;
    public Quaternion handRot;

    protected Coroutine lerpCoroutine;
    protected Rigidbody _rb;
    protected WeaponManagerFullBody whoIsHolding;
    protected InteractableActions interactableActions;
    public Coroutine LerpCoroutine { get { return lerpCoroutine; } set { lerpCoroutine = value; } }
    public WeaponManagerFullBody WhoIsHolding { get { return whoIsHolding; } set { whoIsHolding = value; } }
    public InteractableActions InteractActions { get { return interactableActions; } set { interactableActions = value; } }
    public Transform ItemParent { get { return itemParent; } }
    public virtual bool IsInteracting => false;

    void Start()
    {
        //SetupAnimator(PlayerIK.instance);
        TryGetComponent<Rigidbody>(out _rb);
        TryGetComponent<InteractableActions>(out interactableActions);
        if (itemParent == null) itemParent = transform;
    }

    public virtual void Pickup(PlayerIK playerIK, WeaponManagerFullBody wpnManager)
    {
        //Destroy(_rb);
        whoIsHolding = wpnManager;
        foreach (var col in colliders)
        {
            col.enabled = true;
        }
        _rb.isKinematic = true;
        StartLerpCoroutine((t) => transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(wpnManager.transform.right), t), 
            0.5f, 
            () => ParentItem());
        //transform.localPosition = handParent.position;
        //playerIK.Overrider.SetAnimations(overrideAnimations);
        playerIK.SetupRig(PickupPoseParent, 0);
        WhoIsHolding.TargetItem = this;
        WhoIsHolding.HoldingItem = this.transform;
        SoundManager.Instance.CreateSoundBuilder().Play(wpnManager.ctx.PlayerSFX.pickupSound);
    }

    public virtual void PickupItemFromSlot(InventorySlot slot, WeaponManagerFullBody wpnManager, PlayerIK playerIK)
    {
        if (slot.poungeCap) slot.poungeCap.SetActive(true);
        wpnManager.HoldingItem = slot.OccupiedSlot;
        foreach (var col in colliders)
        {
            col.enabled = true;
        }
        whoIsHolding = wpnManager;
        playerIK.SetupRig(PickupPoseParent, 0);
        slot.OccupiedSlot = null;
        wpnManager.TargetItem = this;
        SoundManager.Instance.CreateSoundBuilder().Play(wpnManager.ctx.PlayerSFX.pickupSound);
    }

    public virtual void DropItem(PlayerIK playerIK, WeaponManagerFullBody wpnManager)
    {
        //_rb = gameObject.AddComponent<Rigidbody>();
        //_rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        foreach (var col in colliders)
        {
            col.enabled = true;
        }
        _rb.isKinematic = false; 
        playerIK.SetupRig(wpnManager.cursor, 0);
        transform.parent = null;
        whoIsHolding = null;
        wpnManager.HoldingItem = null;
        wpnManager.TargetItem = null;
    }

    public virtual void MoveItem(Transform position)
    {
        if (interactableActions != null)
            interactableActions.CheckForChangeAction(position.position);
        else
        {

            itemParent.position = position.transform.position;
        }
    }

    public virtual void InteractItem(Vector3 position, Vector3 direction)
    {
        return;
    }

    public virtual void OnChangeActionTrigger()
    {
        return;
    }

    public void StartLerpCoroutine(Action<float> action, float duration, Action onEndMethod = null)
    {
        if (lerpCoroutine != null)
        {
            StopCoroutine(lerpCoroutine);
            lerpCoroutine = null;
        }
        lerpCoroutine = StartCoroutine(LerpEnumerator.OnUpdate(action, onEndMethod, duration));
    }

    public InteractableActions GetInteractActions()
    {
        throw new NotImplementedException();
    }

    void ParentItem() { if (whoIsHolding != null && !whoIsHolding.IsBusy) transform.parent = whoIsHolding.transform; }
}
