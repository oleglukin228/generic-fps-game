using LlamAcademy.Guns.Demo;
using System.Collections;
using UnityEngine;

public class ShotgunForend : InteractableEnvironment, IReloader
{
    public float timeToReturn = 0.1f;
    [SerializeField] private GunItem weapon;
    public WeaponMagazineTube magazineTube;
    public WeaponBoltCarrier dependenceCarrier;
    public bool isSelfLoading;
    public bool isBoltAction = false;
    public bool infiniteAmmo;
    public AudioClip lowAmmoIndicatorSound;
    public AudioClip lowAmmoDrySound;
    bool ammoLoaded;
    bool isBoltCocked;
    bool isAmmoUsed;
    public bool AmmoLoaded { get => ammoLoaded; set => ammoLoaded = value; }

    public override void InteractItem(Vector3 cursorPosition, Vector3 direction)
    {

        //transform.position = cursorPosition;
    }

    public void LoadAmmo()
    {
        if (isBoltCocked)
        {
            ammoLoaded = true;
        }
    }

    public void BoltActionLoadAmmo()
    {
        if (isBoltAction && magazineTube.BulletsLeft > 0)
        {
            magazineTube.BulletsLeft--;
            ammoLoaded = true;
        }
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
        if (magazineTube.BulletsLeft > 0) magazineTube.BulletsLeft--;
        isAmmoUsed = false;
    }

    public IEnumerator EjectShell()
    {
        if (dependenceCarrier == null) yield break;
        StartCoroutine(dependenceCarrier.EjectShell());
    }

    public GameObject GetBullet()
    {
        return magazineTube.bullet;
    }

    public void ConsumeBullet(int amount)
    {
        //magazineTube.BulletsLeft -= amount;
        //ObjectPoolManager.SpawnObject(magazineTube.casingPrefab, magazineTube.casingPivot.position, magazineTube.casingPivot.rotation, ObjectPoolManager.PoolType.ParticleSystems);

        if (isSelfLoading && ammoLoaded)
        {
            magazineTube.casingParticle.Emit(1);
            //ObjectPoolManager.SpawnObject(magazineTube.casingPrefab, magazineTube.casingPivot.position, magazineTube.casingPivot.rotation, ObjectPoolManager.PoolType.ParticleSystems);
            if (magazineTube.BulletsLeft > 0)
            {
                if (!infiniteAmmo) magazineTube.BulletsLeft--;
            }
            else
            {
                if (!infiniteAmmo)
                {
                    isAmmoUsed = false;
                    ammoLoaded = false;
                }
            }
            StartCoroutine(EjectShell());
        }
        else
        {
            if (!infiniteAmmo)
            {
                isAmmoUsed = true;
                ammoLoaded = false;
                isBoltCocked = false;
            }
        }

        float ammoRatio = (float)magazineTube.BulletsLeft / (magazineTube.maxAmmo * 0.34f); // Определение соотношения патронов
        float volume = Mathf.Lerp(0f, 1f, 1f - (ammoRatio)); // Изменение громкости в зависимости от соотношения патронов
        if (lowAmmoIndicatorSound != null)
            AudioSource.PlayClipAtPoint(lowAmmoIndicatorSound, transform.position, volume);
        if (lowAmmoDrySound != null && magazineTube.BulletsLeft == 0)
            AudioSource.PlayClipAtPoint(lowAmmoDrySound, transform.position);
    }

    public void AppllyPose(Transform parent)
    {
        weapon.Owner.playerIK.SetupRig(parent, 1);
    }

    public bool IsBoltInStartPosition()
    {
        return Vector3.Distance(transform.position, interactableActions.items[0].tweens[0].transformStartPosition.position) > 0.01f;
    }

    public void ActivateActions(bool i, PlayerIK playerIK = null)
    {
        if (i)
        {
            //WhoIsHolding.IsBusy = true;
            whoIsHolding = weapon.Owner;
            //interactableActions.CurrentItem = this;
            if (!whoIsHolding.ctx.IsControllingHand) CursorController.SetInteractableActions(interactableActions);
        }
        else
        {
            //WhoIsHolding.IsBusy = false;
            CursorController.SetInteractableActions();
            //interactableActions.CurrentItem = null;
            whoIsHolding = null;
        }
    }

    public InteractableActions GetInteractActions()
    {
        return interactableActions;
    }
}
