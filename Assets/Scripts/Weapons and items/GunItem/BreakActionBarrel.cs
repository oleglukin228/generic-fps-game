using LlamAcademy.Guns.Demo;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BreakActionBarrel : InteractableEnvironment, IReloader
{
    public BreakActionBulletTube[] barrelTubes;
    public ParticleSystem casingParticle;
    List<BreakActionBulletTube> loadedTubes = new List<BreakActionBulletTube>();
    List<BreakActionBulletTube> usedTubes = new List<BreakActionBulletTube>();
    public GameObject bullet;
    bool ammoLoaded;
    bool isBoltCocked;
    bool isAmmoUsed;
    float timeElapsed;
    public List<BreakActionBulletTube> LoadedTubes { get => loadedTubes; set => loadedTubes = value; }
    public bool AmmoLoaded { get => ammoLoaded; set => ammoLoaded = value; }

    public override void MoveItem(Transform position)
    {
        base.MoveItem(position);
    }

    public void ConsumeBullet(int amount)
    {
        usedTubes.Add(loadedTubes[0]);
        loadedTubes.Remove(loadedTubes[0]);
        if (loadedTubes.Count == 0) ammoLoaded = false;
    }

    public GameObject GetBullet()
    {
        return bullet;
    }

    public void OnBarrelOpen()
    {
        foreach (var barrel in barrelTubes)
        {
            barrel.EnableCollider();
        }
        if (usedTubes.Count > 0)
        {
            foreach (var barrel in usedTubes)
            {
                Destroy(barrel.OccupiedSlot.gameObject);
                barrel.OccupiedSlot = null;
                barrel.InteractActions.ResetActions();
            }
            casingParticle.Emit(usedTubes.Count);
            usedTubes.Clear();
        }
    }

    public void OnBarrelClose()
    {
        foreach (var barrel in barrelTubes)
        {
            barrel.DisableCollider();
        }
        if (loadedTubes.Count > 0) ammoLoaded = true;
    }

    public bool IsBoltInStartPosition()
    {
        throw new System.NotImplementedException();
    }

    public void ActivateActions(bool i, PlayerIK playerIK = null)
    {
        return;
    }

    public InteractableActions GetInteractActions()
    {
        return interactableActions;
    }
}
