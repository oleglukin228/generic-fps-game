using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponAttachmentType
{
    Optic,
    Grip,
    Barrel,
    Side,
}
public class WeaponAttachment : MonoBehaviour
{
    public WeaponAttachmentType attachmentType;

    [Header("Shooting stat")]
    public float damageModifier = 1f;
    public float fireRateModifier = 1f;
    public float spreadModifier = 1f;
    public Vector3 aimPosModifier;

    public virtual void ApplyTo(WeaponStatsSO weaponStats)
    {

    }
    public virtual void RemoveFrom(WeaponStatsSO weaponStats, WeaponStatsSO baseStats)
    {

    }
}
