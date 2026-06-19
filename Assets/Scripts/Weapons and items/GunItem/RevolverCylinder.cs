using LlamAcademy.Guns.Demo;
using System.Collections.Generic;
using UnityEngine;

public class RevolverCylinder : InteractableEnvironment, IReloader
{
    [SerializeField] private GunItem weapon;
    [SerializeField] private InteractableActions hammerAnimation;
    [SerializeField] private InteractableActions spinAnimation;
    [SerializeField] private InteractableActions cylinderHolderAnimation;
    [SerializeField] protected Transform OnReloadHandPose;
    [SerializeField] protected Transform OnReloadEndHandPose;
    [SerializeField] RevolverBulletSlot[] barrelTubes;
    [SerializeField] bool onReloadOpenCylinder;
    [SerializeField] AudioClip lowAmmoIndicatorSound;
    [SerializeField] AudioClip lowAmmoDrySound;
    List<RevolverBulletSlot> loadedTubes = new List<RevolverBulletSlot>();
    List<RevolverBulletSlot> usedTubes = new List<RevolverBulletSlot>();
    [SerializeField] GameObject bullet;
    [SerializeField] bool infiniteAmmo;
    bool ammoLoaded;
    public List<RevolverBulletSlot> LoadedTubes { get => loadedTubes; set => loadedTubes = value; }
    public bool AmmoLoaded { get => ammoLoaded; set => ammoLoaded = value; }

    public override void MoveItem(Transform position)
    {
        base.MoveItem(position);
    }

    public void ConsumeBullet(int amount)
    {
        if (!infiniteAmmo)
        {
            usedTubes.Add(loadedTubes[^1]);
            loadedTubes.Remove(loadedTubes[^1]);
        }

        hammerAnimation.PlayAnimations(0.1f);
        spinAnimation.PlayAnimations(0.1f);

        if (loadedTubes.Count > 0)
        {
            float ammoRatio = (float)loadedTubes.Count / (barrelTubes.Length * 0.34f); // Определение соотношения патронов
            float volume = Mathf.Lerp(0f, 1f, 1f - (ammoRatio)); // Изменение громкости в зависимости от соотношения патронов
            if (lowAmmoIndicatorSound != null)
                AudioSource.PlayClipAtPoint(lowAmmoIndicatorSound, transform.position, volume);
        }
        else
        {
            ammoLoaded = false;
            if (lowAmmoDrySound != null)
                AudioSource.PlayClipAtPoint(lowAmmoDrySound, transform.position);
        }
    }

    public GameObject GetBullet()
    {
        return bullet;
    }

    public void OnCylinderOpen()
    {
        if (loadedTubes.Count == barrelTubes.Length) return;
        barrelTubes[loadedTubes.Count].EnableCollider();
        if (usedTubes.Count > 0)
        {
            foreach (var barrel in usedTubes)
            {
                Destroy(barrel.OccupiedSlot.gameObject);
                barrel.OccupiedSlot = null;
                barrel.casingParticle.Emit(1);
                //ObjectPoolManager.SpawnObject(barrel.casingParticle, barrel.casingPivot.position, barrel.casingPivot.rotation, ObjectPoolManager.PoolType.ParticleSystems);
                barrel.InteractActions.ResetActions();
            }
            usedTubes.Clear();
        }
    }

    public void OnCylinderClose()
    {
        foreach (var barrel in barrelTubes)
        {
            barrel.DisableCollider();
        }
        if (loadedTubes.Count > 0) ammoLoaded = true;
    }

    public bool IsBoltInStartPosition()
    {
        return false;
    }

    public void ActivateActions(bool i, PlayerIK playerIK)
    {
        if (i)
        {
            cylinderHolderAnimation.PlayAnimations(0.1f);
            spinAnimation.ResetActions();
            playerIK.SetupRig(OnReloadHandPose, 1);
        }
        else
        {
            cylinderHolderAnimation.PlayAnimationsBackwards(0.1f);
            playerIK.SetupRig(OnReloadEndHandPose, 1);
        }
    }

    public InteractableActions GetInteractActions()
    {
        return interactableActions;
    }

    public void ActivateNextSlot()
    {
        if (loadedTubes.Count != barrelTubes.Length) 
            barrelTubes[loadedTubes.Count].EnableCollider();
    }
}
