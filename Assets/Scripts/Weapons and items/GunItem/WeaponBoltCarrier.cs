using LlamAcademy.Guns.Demo;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
public interface IReloader
{
    Transform ItemParent { get; }
    GameObject GetBullet();
    bool IsBoltInStartPosition();
    void MoveItem(Transform cursorPosition);
    void ActivateActions(bool i, PlayerIK playerIK);
    void ConsumeBullet(int amount);
    bool AmmoLoaded { get; set; }
    InteractableActions GetInteractActions();
}
public class WeaponBoltCarrier : InteractableEnvironment, IReloader
{
    [SerializeField] float timeToReturn = 0.1f;
    [SerializeField] GunItem weapon;
    [SerializeField] WeaponMagazineSlot magazineSlot;
    [SerializeField] ParticleSystem casingParticle;
    [SerializeField] bool isSelfLoading = true;
    [SerializeField] bool isBoltAction = false;
    public AudioClip lowAmmoIndicatorSound;
    public AudioClip lowAmmoDrySound;
    bool ammoLoaded;
    bool isAmmoUsed;
    float timeElapsed;
    public bool AmmoLoaded { get => ammoLoaded; set => ammoLoaded = value; }
    /*public override void Pickup(PlayerIK playerIK, WeaponManagerFullBody wpnManager)
    {
        base.Pickup(playerIK, wpnManager);
        wpnManager.TargetItem = this;
        wpnManager.HoldingItem = this.transform;
    }*/

    public bool CheckMagazine()
    {
        if (magazineSlot.magazine != null)
        {
            return true;
        }
        return false;
    }

    public bool CheckBulletsLeft()
    {
        if (magazineSlot.magazine.BulletsLeft > 0)
        {
            return true;
        }
        return false;
    }

    public GameObject GetBullet()
    {
        return magazineSlot.magazine.bullet;
    }

    public void ConsumeBullet(int amount)
    {
        if (magazineSlot.magazine != null && magazineSlot.magazine.isAttached)
        {
            if (isSelfLoading && ammoLoaded)
            {
                casingParticle.Emit(1);
                //ObjectPoolManager.SpawnObject(magazineSlot.magazine.casingPrefab, casingParticle.position, casingParticle.rotation, ObjectPoolManager.PoolType.ParticleSystems);
                if (magazineSlot.magazine.BulletsLeft > 0)
                {
                    magazineSlot.magazine.BulletsLeft--;
                    float ammoRatio = (float)magazineSlot.magazine.BulletsLeft / (magazineSlot.magazine.maxAmmo * 0.34f); // Определение соотношения патронов
                    float volume = Mathf.Lerp(0f, 1f, 1f - (ammoRatio)); // Изменение громкости в зависимости от соотношения патронов
                    if (lowAmmoIndicatorSound != null)
                        AudioSource.PlayClipAtPoint(lowAmmoIndicatorSound, transform.position, volume);
                }
                else
                {
                    isAmmoUsed = false;
                    ammoLoaded = false;
                    if (lowAmmoDrySound != null)
                        AudioSource.PlayClipAtPoint(lowAmmoDrySound, transform.position);
                    magazineSlot.magazine.BulletVusial.SetActive(false);
                }
                StartCoroutine(EjectShell());
            }
            else
            {
                isAmmoUsed = true;
                ammoLoaded = false;
            } 
        }
        else
        {
            if (isSelfLoading) StartCoroutine(EjectShell());
            isAmmoUsed = true;
            ammoLoaded = false;
        }
    }

    public override void DropItem(PlayerIK playerIK, WeaponManagerFullBody wpnManager)
    {
        if (!isBoltAction)
        {
            if (interactableActions != null)
            {
                interactableActions.ResetActions();
            }
            StartCoroutine(ResetBoltPosition());
        }
        base.DropItem(playerIK, wpnManager);
        WhoIsHolding = null;
        wpnManager.HoldingItem = null;
        wpnManager.TargetItem = null;
    }

    public override void MoveItem(Transform cursorPosition)
    {
        base.MoveItem(cursorPosition);
    }

    public override void InteractItem(Vector3 cursorPosition, Vector3 direction)
    {
        
        //transform.position = cursorPosition;
    }

    public void OnChangeActionTrigger()
    {
        if (magazineSlot.magazine != null && magazineSlot.magazine.isAttached)
        {
            if (!ammoLoaded && !isAmmoUsed && magazineSlot.magazine.BulletsLeft > 0)
            {
                ammoLoaded = true;
            }
            else if (!ammoLoaded && isAmmoUsed && magazineSlot.magazine.BulletsLeft > 0)
            {
                ammoLoaded = true;
                casingParticle.Emit(1);
                //ObjectPoolManager.SpawnObject(magazineSlot.magazine.casingPrefab, casingParticle.position, casingParticle.rotation, ObjectPoolManager.PoolType.ParticleSystems);
            }
            else
            {
                if (magazineSlot.magazine.BulletsLeft > 0)
                {
                    casingParticle.Emit(1);
                    //ObjectPoolManager.SpawnObject(magazineSlot.magazine.casingPrefab, casingParticle.position, casingParticle.rotation, ObjectPoolManager.PoolType.ParticleSystems);
                }
                else if (magazineSlot.magazine.BulletsLeft == 0)
                {
                    if (ammoLoaded && !isAmmoUsed)
                    {
                        casingParticle.Emit(1);
                        //ObjectPoolManager.SpawnObject(magazineSlot.magazine.casingPrefab, casingParticle.position, casingParticle.rotation, ObjectPoolManager.PoolType.ParticleSystems);
                        ammoLoaded = false;
                    }
                    else if (!ammoLoaded && isAmmoUsed)
                    {
                        casingParticle.Emit(1);
                        //ObjectPoolManager.SpawnObject(magazineSlot.magazine.casingPrefab, casingParticle.position, casingParticle.rotation, ObjectPoolManager.PoolType.ParticleSystems);
                        ammoLoaded = false;
                    }
                    magazineSlot.magazine.BulletVusial.SetActive(false);
                }
            }
            isAmmoUsed = false;
            if (magazineSlot.magazine.BulletsLeft > 0) magazineSlot.magazine.BulletsLeft--;
            else return;
            if (magazineSlot.magazine.BulletsLeft == 1) magazineSlot.magazine.BulletVusial.SetActive(false);
        }
        else
        {
            if (ammoLoaded && !isAmmoUsed)
            {
                ammoLoaded = false;
                //Instantiate(magazineSlot.magazine.casingPrefab, casingPivot.position, casingPivot.rotation);
                //ObjectPoolManager.SpawnObject(magazineSlot.magazine.casingPrefab, casingParticle.position, casingParticle.rotation, ObjectPoolManager.PoolType.ParticleSystems);
                casingParticle.Emit(1);
            }
            else if (!ammoLoaded && isAmmoUsed)
            {
                isAmmoUsed = false;
                //Instantiate(magazineSlot.magazine.casingPrefab, casingPivot.position, casingPivot.rotation);
                //ObjectPoolManager.SpawnObject(magazineSlot.magazine.casingPrefab, casingParticle.position, casingParticle.rotation, ObjectPoolManager.PoolType.ParticleSystems);
                casingParticle.Emit(1);
            }
        }
    }

    public IEnumerator EjectShell()
    {
        timeElapsed = 0;
        transform.position = interactableActions.items[0].tweens[0].transformEndPosition.position;
        
        while (timeElapsed < timeToReturn)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / timeToReturn;
            transform.position = Vector3.Lerp(transform.position, interactableActions.items[0].tweens[0].transformStartPosition.position, t);
            yield return null;
        }
    }

    IEnumerator ResetBoltPosition()
    {
        timeElapsed = 0;
        while (timeElapsed < timeToReturn)
        {
            timeElapsed += Time.fixedDeltaTime;
            float t = timeElapsed / timeToReturn;
            transform.position = Vector3.Lerp(transform.position, interactableActions.items[0].tweens[0].transformStartPosition.position, t);
            yield return null;
        }
        interactableActions.items[0].onStartActionTrigger.Invoke();
    }

    public bool IsBoltInStartPosition()
    {
        return Vector3.Distance(transform.position, interactableActions.items[0].tweens[0].transformStartPosition.position) > 0.01f;
    }

    public void ActivateActions(bool i, PlayerIK playerIK = null)
    {
        if (i)
        {
            WhoIsHolding = weapon.Owner;
        }
        else
        {
            WhoIsHolding = null;
        }
    }

    public InteractableActions GetInteractActions()
    {
        return interactableActions;
    }
}