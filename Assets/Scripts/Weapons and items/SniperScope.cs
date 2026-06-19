using Kineractive;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperScope : WeaponAttachment
{
    public GameObject scopeLense;
    public float aimSensitivityMultiplier = 0.5f;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1)) scopeLense.SetActive(true);
        if (Input.GetKeyUp(KeyCode.Mouse1)) scopeLense.SetActive(false);
    }

    public override void ApplyTo(WeaponStatsSO weaponStats)
    {
        weaponStats.aimWeaponPos += aimPosModifier;
        weaponStats.aimSensitivityMultiplier = aimSensitivityMultiplier;
    }
    public override void RemoveFrom(WeaponStatsSO weaponStats, WeaponStatsSO baseStats)
    {
        weaponStats.aimWeaponPos = baseStats.aimWeaponPos;
        weaponStats.aimSensitivityMultiplier = baseStats.aimSensitivityMultiplier;
    }
}
