using AudioSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakActionGunItem : GunItem, IPickableInterface
{
    public int pelleteAmount = 12;
    public Vector3 pelleteSpread = Vector3.one;
    public override void Reload(bool i)
    {
        owner.Reloading = i;
        owner.ctx.animator.SetBool("isReloading", owner.Reloading);
        if (!owner.ctx.IsRunning) wpnBolt.Value.ActivateActions(owner.Reloading, owner.playerIK);
    }

    protected override void ReloadState()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (owner.ctx.IsRunning) return;
            Reload(!owner.Reloading);
            ControlHint.Instance.UpdateRKeyHint();
        }
    }

    protected override void Shoot()
    {
        readyToShot = false;
        shooting = true;
        // Получить случайный разброс на основе времени стрельбы
        shootDirection = transform.forward + GetSpread(LastShootTime);
        muzzleFlashParticle.Emit(1);
        particleShooter.Emit(pelleteAmount);
        SoundManager.Instance.CreateSoundBuilder().WithPosition(transform.position).Play(currentStats.shotSound);
        /*for (int i = 0; i < pelleteAmount; i++)
        {
            var pelleteSpread = transform.forward + GetPelleteSpread();
            GameObject bullet = ObjectPoolManager.SpawnObject(wpnBolt.Value.GetBullet(), muzzleFlashParticle.transform.position, Quaternion.identity);
            bullet.GetComponent<BulletProjectile>().Setup(pelleteSpread, owner.transform, surfaceImpact, 300f, baseStats.hitForce, baseStats.damage);
        }*/
        wpnBolt.Value.ConsumeBullet(1);
        bulletsPerTap--;
        LastShootTime += baseStats.shootingSpeed;
        //StartCoroutine(wpnBolt.EjectShell());
        recoilPosition.z -= baseStats.kickbackForce;
        float rand = Random.Range(-10f, 10f);
        if (!CursorController.RMBhold) transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z - rand);
        owner.ctx.recoilAngle.x = -Mathf.Clamp01(LastShootTime / baseStats.MaxSpreadTime) * 2;
        owner.ctx.recoilAngle.y = (bulletsShot % 2 != 0 ? -1f : 1f) * Mathf.Clamp01(LastShootTime / baseStats.MaxSpreadTime);
        //vignette.intensity.value = Mathf.Clamp(LastShootTime / baseStats.MaxSpreadTime * 0.2f, 0f, 0.2f);
        PostProcessingController.ChangeRecoilVolumeValue(Mathf.Lerp(0f, 1f, LastShootTime / currentStats.MaxSpreadTime));
        //StartCoroutine(SpreadCoroutine());
        Invoke(nameof(ResetShot), baseStats.shootingSpeed);
    }

    public Vector3 GetPelleteSpread()
    {
        return new Vector3(
                Random.Range(-pelleteSpread.x, pelleteSpread.x),
                Random.Range(-pelleteSpread.y, pelleteSpread.y),
                Random.Range(-pelleteSpread.z, pelleteSpread.z)
            );
    }
}
