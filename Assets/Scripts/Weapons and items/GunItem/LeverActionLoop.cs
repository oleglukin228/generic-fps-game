using LlamAcademy.Guns.Demo;
using System.Collections;
using UnityEngine;

public class LeverActionLoop : InteractableEnvironment, IReloader
{
    [SerializeField] private GunItem weapon;
    public WeaponMagazineTube magazineTube;
    public bool isSelfLoading;
    public bool isBoltAction = false;
    public Transform hammer;
    public Transform hammerUnloadedPosition;
    public bool infiniteAmmo;
    public AudioClip lowAmmoIndicatorSound;
    public AudioClip lowAmmoDrySound;
    bool ammoLoaded;
    bool isBoltCocked;
    bool isAmmoUsed;
    public bool AmmoLoaded { get => ammoLoaded; set => ammoLoaded = value; }

    public void ActivateActions(bool i, PlayerIK playerIK = null)
    {
        if (i)
        {
            whoIsHolding = weapon.Owner;
            whoIsHolding.playerIK.SetupRig(PickupPoseParent, 1);
            if (!whoIsHolding.ctx.IsControllingHand) CursorController.SetInteractableActions(interactableActions);
        }
        else
        {
            whoIsHolding.playerIK.SetupRig(weapon.ArmedPoseParent, whoIsHolding.ctx.IsControllingHand ? 1 : 2);
            CursorController.SetInteractableActions();
            whoIsHolding = null;
        }
    }

    public void ConsumeBullet(int amount)
    {
        isAmmoUsed = true;
        ammoLoaded = false;
        isBoltCocked = false;

        hammer.SetPositionAndRotation(hammerUnloadedPosition.position, hammerUnloadedPosition.rotation);
        ParentHammer();

        float ammoRatio = (float)magazineTube.BulletsLeft / (magazineTube.maxAmmo * 0.34f); // Определение соотношения патронов
        float volume = Mathf.Lerp(0f, 1f, 1f - (ammoRatio)); // Изменение громкости в зависимости от соотношения патронов
        if (lowAmmoIndicatorSound != null)
            AudioSource.PlayClipAtPoint(lowAmmoIndicatorSound, transform.position, volume);
        if (lowAmmoDrySound != null && magazineTube.BulletsLeft == 0)
            AudioSource.PlayClipAtPoint(lowAmmoDrySound, transform.position);
    }

    public IEnumerator EjectShell()
    {
        //if (dependenceCarrier == null) yield break;
        //StartCoroutine(dependenceCarrier.EjectShell());
        yield return null;
    }

    public bool IsBoltInStartPosition()
    {
        return Quaternion.Angle(transform.rotation, interactableActions.items[0].tweens[0].transformStartPosition.rotation) > 0.01f;
    }

    public GameObject GetBullet()
    {
        return magazineTube.bullet;
    }

    public InteractableActions GetInteractActions()
    {
        return interactableActions;
    }

    public void OnChangeActionTrigger()
    {
        if (!ammoLoaded && magazineTube.BulletsLeft > 0)
        {
            isBoltCocked = true;
            if (isAmmoUsed)
            {
                magazineTube.casingParticle.Emit(1);
                //ObjectPoolManager.SpawnObject(magazineTube.casingPrefab, magazineTube.casingPivot.position, magazineTube.casingPivot.rotation, ObjectPoolManager.PoolType.ParticleSystems);
            }
        }
        else
        {
            if (magazineTube.BulletsLeft > 0)
            {
                magazineTube.casingParticle.Emit(1);
                //ObjectPoolManager.SpawnObject(magazineTube.casingPrefab, magazineTube.casingPivot.position, magazineTube.casingPivot.rotation, ObjectPoolManager.PoolType.ParticleSystems);
            }
            else if (magazineTube.BulletsLeft == 0)
            {
                if (ammoLoaded && !isAmmoUsed)
                {
                    magazineTube.casingParticle.Emit(1);
                    //ObjectPoolManager.SpawnObject(magazineTube.casingPrefab, magazineTube.casingPivot.position, magazineTube.casingPivot.rotation, ObjectPoolManager.PoolType.ParticleSystems);
                    //ammoLoaded = false;
                }
                else if (!ammoLoaded && isAmmoUsed)
                {
                    magazineTube.casingParticle.Emit(1);
                    //ObjectPoolManager.SpawnObject(magazineTube.casingPrefab, magazineTube.casingPivot.position, magazineTube.casingPivot.rotation, ObjectPoolManager.PoolType.ParticleSystems);
                    //ammoLoaded = false;
                }
                ammoLoaded = false;
                isBoltCocked = false;
            }
        }
        if (magazineTube.BulletsLeft > 0 && !infiniteAmmo) magazineTube.BulletsLeft--;
        isAmmoUsed = false;
    }

    public void LoadAmmo()
    {
        if (isBoltCocked)
        {
            ammoLoaded = true;
        }
    }

    public void UnparentHammer() { hammer.SetParent(weapon.transform); }
    public void ParentHammer() { hammer.SetParent(this.transform); }
}
