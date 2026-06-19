using AudioSystem;
using UnityEngine;

public class ShotgunItem : GunItem, IPickableInterface, IPickableWeaponInterface
{
    public int pelleteAmount = 12;
    public Vector3 pelleteSpread = Vector3.one;
    public override void Reload(bool i)
    {
        //Debug.Log(Vector3.Distance(wpnBolt.Value.ItemParent.position, wpnBolt.Value.GetBoltPosition()));
        if (wpnBolt.Value.IsBoltInStartPosition()) return; 
        owner.Reloading = i;
        owner.ctx.animator.SetBool("isReloading", owner.Reloading);
    }

    public override void UpdateWeapon()
    {
        if (Owner.Reloading && wpnBolt != null) 
            if (!Owner.ctx.IsControllingHand) wpnBolt.Value.MoveItem(null);

        base.UpdateWeapon();
    }

    protected override void ReloadState()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (owner.ctx.IsRunPressed) return;
            Reload(!owner.Reloading);
            wpnBolt.Value.ActivateActions(owner.Reloading, owner.playerIK);
        }
    }

    protected override void Shoot()
    {
        LastShootTime += baseStats.shootingSpeed;
        readyToShot = false;
        shooting = true;
        shootDirection = transform.forward + GetSpread(LastShootTime);
        muzzleFlashParticle.Emit(1);
        particleShooter.Emit(pelleteAmount);
        wpnBolt.Value.ConsumeBullet(1);
        bulletsPerTap--;
        recoilPosition.z -= baseStats.kickbackForce;
        float rand = Random.Range(-10f, 10f);
        if (!CursorController.RMBhold) transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z - rand);
        owner.ctx.recoilAngle.x = 2;
        owner.ctx.recoilAngle.y = (bulletsShot % 2 != 0 ? -1f : 1f);
        SoundManager.Instance.CreateSoundBuilder().WithPosition(transform.position).Play(currentStats.shotSound);
        PostProcessingController.ChangeRecoilVolumeValue(Mathf.Lerp(0f, 1f, LastShootTime / currentStats.MaxSpreadTime));
        Invoke(nameof(ResetShot), baseStats.shootingSpeed);
    }

    public override void DropWeapon()
    {
        //wpnBolt.Value.ItemParent.position = wpnBolt.Value.IsBoltInStartPosition();
        base.DropWeapon();
    }

    public Vector3 GetPelleteSpread()
    {
        return new Vector3(
                Random.Range(-pelleteSpread.x, pelleteSpread.x),
                Random.Range(-pelleteSpread.y, pelleteSpread.y),
                Random.Range(-pelleteSpread.z, pelleteSpread.z)
            );
    }

    public override void OnHandSwitchTrigger(bool isControllingHand)
    {
        CursorController.EnableCursor(isControllingHand);
        if (isControllingHand)
        {
            if (owner.playerIK.ControlRightHand) owner.playerIK.SetupRig(owner.cursor, 1);
            else owner.playerIK.SetupRig(owner.cursor, 0);
            owner.playerIK.AcivateLeftFingersIK = true;
            if (oneHandedWeapon) owner.playerIK.ChangeLeftHandIKValue(1f, 0.5f);
            owner.cursor.parent = owner.ctx.cameraPosition;
            CursorController.SetInteractableActions();
        }
        else
        {
            owner.playerIK.SetupRig(ArmedPoseParent, 0);
            owner.playerIK.AcivateLeftFingersIK = !oneHandedWeapon;
            owner.playerIK.AcivateRightFingersIK = true;
            if (oneHandedWeapon) owner.playerIK.ChangeLeftHandIKValue(0f, 0.5f);
            owner.cursor.parent = transform;
            if (owner.Reloading) CursorController.SetInteractableActions(wpnBolt.Value.GetInteractActions());
        }
    }
}
