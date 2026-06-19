using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.GridLayoutGroup;

public enum WeaponType
{
    Primary,
    Secondary,
    Melee,
    Consumables,
    Tools
}

public class WeaponItem : PickableStuff
{
    public Transform ArmedPoseParent;
    public WeaponType weaponType;
    public SurfaceImpact surfaceImpact;
    public bool oneHandedWeapon = false;

    [Header("Data")]
    public string weaponName;
    public BoxCollider[] interactableColliders;
    protected WeaponManagerFullBody owner;

    public virtual void DropWeapon()
    {
        if (CursorController.IsInteracting) return;
        owner.HeldWeapon = null;
        foreach (var col in interactableColliders)
        {
            col.enabled = false;
        }
        foreach (var col in colliders)
        {
            col.enabled = true;
        }
        _rb.useGravity = true;
        _rb.isKinematic = false;
        _rb.constraints = RigidbodyConstraints.None;
        transform.SetParent(null);
        //owner.playerIK.SetupRig(owner.cursor, owner.HoldingItem == null ? 2 : 1);
        owner.playerIK.AcivateLeftFingersIK = false;
        owner.playerIK.AcivateRightFingersIK = false;
        owner.cursor.parent = owner.ctx.cameraPosition;
        CursorController.SetInteractableActions(null);
        owner.ChangeCursorPosition(owner.ctx.cameraPosition.TransformPoint(Vector3.forward));
        gameObject.layer = LayerMask.NameToLayer("Pickable");
        if (!owner.ctx.IsControllingHand) owner.playerIK.ChangeLeftHandIKValue(0f, 0.5f);
        owner.playerIK.ChangeRightHandIKValue(0f, 0.5f);
        //owner.ctx.RMBhold = false;
        owner = null;
    }
}
