using AudioSystem;
using UnityEngine;
[CreateAssetMenu(menuName = "Generic fps/Weapon Stats")]
public class WeaponStatsSO : ScriptableObject
{
    [Header("Shooting stat")]
    public float damage;
    public float hitForce = 3.5f;
    public float shootingSpeed;
    public bool allowButtonHold;
    public Vector3 Spread = new Vector3(0.1f, 0.1f, 0.1f);
    public Vector3 MinSpread = Vector3.zero;
    public bool absoluteSpreadValue = false;
    public float RecoilRecoverySpeed = 1f;
    public float MaxSpreadTime = 1f;
    public float spreadMultiplier = 1f;
    public float kickbackForce = 0.05f;
    public float resetSmooth = 8f;

    [Header("Weaopon sounds")]
    public SoundData shotSound;
    public SoundData dryFireSound;

    [Header("In hand while aiming position")]
    public float aimSensitivityMultiplier = 1f;
    public float focusDistance = 0.125f;
    public Vector3 aimWeaponPos;
    public Quaternion aimWeaponRot;

    public WeaponStatsSO Clone()
    {
        WeaponStatsSO clone = CreateInstance<WeaponStatsSO>();
        clone.damage = damage;
        clone.hitForce = hitForce;
        clone.shootingSpeed = shootingSpeed;
        clone.allowButtonHold = allowButtonHold;
        clone.Spread = Spread;
        clone.MinSpread = MinSpread;
        clone.absoluteSpreadValue = absoluteSpreadValue;
        clone.RecoilRecoverySpeed = RecoilRecoverySpeed;
        clone.MaxSpreadTime = MaxSpreadTime;
        clone.spreadMultiplier = spreadMultiplier;
        clone.kickbackForce = kickbackForce;
        clone.resetSmooth = resetSmooth;
        clone.aimSensitivityMultiplier = aimSensitivityMultiplier;
        clone.aimWeaponPos = aimWeaponPos;
        clone.aimWeaponRot = aimWeaponRot;
        clone.shotSound = shotSound;
        clone.dryFireSound = dryFireSound;
        clone.focusDistance = focusDistance;
        return clone;
    }
}
