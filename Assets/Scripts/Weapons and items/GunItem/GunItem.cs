using AudioSystem;
using LlamAcademy.Guns.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GunItem : WeaponItem, IPickableInterface, IPickableWeaponInterface
{
    [Header("Shooting stat")]
    public WeaponStatsSO baseStats;
    public InterfaceReference<IReloader> wpnBolt;
    private float InitialClickTime;
    private float StopShootingTime;
    protected float LastShootTime;

    [Header("In hand while run/safety position")]
    public bool liberateLeftHand;
    public Vector3 safetyWeaponPos;
    public Quaternion safetyWeaponRot;

    [Header("In hand while reloading position")]
    public Vector3 reloadWeaponPos;
    public Quaternion reloadWeaponRot;

    [Header("Wall collision position")]
    public float tuckedDistance = 1f;
    public float tuckedAngle = -60f;
    public float tuckedPosition = -0.1f;

    [Header("Data")]
    public Transform weaponPivotRotation;
    public ParticleSystem muzzleFlashParticle;
    public ParticleSystem particleShooter;
    protected WeaponStatsSO currentStats;
    protected WeaponAttachmentSystem attachmentSystem;
    protected int bulletsShot, bulletsPerTap;
    protected bool shooting, readyToShot = true;
    protected Vector3 shootDirection;
    protected Vector3 recoilPosition;
    protected float tuckedRotation;
    public WeaponManagerFullBody Owner => owner;
    public WeaponStatsSO CurrentStats => currentStats;
    public WeaponType GetWeaponType => weaponType;

    private void Awake()
    {
        currentStats = baseStats.Clone();
        attachmentSystem = GetComponent<WeaponAttachmentSystem>();
    }

    public virtual void UpdateWeapon()
    {
        ReloadState();

        if (Input.GetKeyDown(KeyCode.G))
        {
            attachmentSystem?.DisableAttachments();
            DropWeapon();
            ControlHint.Instance.UpdateWeaponHint();
            ControlHint.Instance.UpdateNumbersHint((int)weaponType);
            return;
        }

        if (readyToShot && CursorController.LMBdown && !owner.Reloading && !owner.ctx.IsControllingHand && wpnBolt.Value.AmmoLoaded)
        {
            LastShootTime += Time.deltaTime; // Обновить время последнего выстрела
            bulletsShot = bulletsPerTap;
            Shoot();
        }

        shootDirection = Vector3.Lerp(shootDirection, Vector3.zero, Time.deltaTime * currentStats.RecoilRecoverySpeed);
        recoilPosition = Vector3.Lerp(recoilPosition, Vector3.zero, currentStats.resetSmooth * Time.deltaTime);

        WeaponSway();
    }

    protected virtual void ReloadState()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (owner.ctx.IsRunning) return;
            Reload(!owner.Reloading);
            ControlHint.Instance.UpdateRKeyHint();
        }
    }

    protected void WeaponSway()
    {
        if (Time.frameCount % 3 == 0)
        {
            tuckedRotation = owner.ctx.GetRaycastDistanceFromCamera(tuckedDistance);
        }

        if (owner.Reloading) //if reloading
        {
            float clamp = Mathf.Lerp(owner.WpnManagerPosition, -owner.ctx.SmoothXRotation / 180f * 0.5f, Time.deltaTime * 5f);
            owner.WpnManagerPosition = Mathf.Clamp(clamp, -1f, 0f);
            owner.transform.localRotation = Quaternion.Lerp(owner.transform.localRotation, Quaternion.identity, Time.deltaTime * 5f);
            transform.SetLocalPositionAndRotation(Vector3.Lerp(transform.localPosition, reloadWeaponPos, Time.deltaTime * 5f),
                Quaternion.Lerp(transform.localRotation, reloadWeaponRot, Time.deltaTime * 5f));
        }
        else if (owner.ctx.IsRunning && !owner.ctx.IsControllingHand) //if running
        {
            owner.WpnManagerPosition = Mathf.Lerp(owner.WpnManagerPosition, 0f, Time.deltaTime * 5f);
            owner.transform.localRotation = Quaternion.Lerp(owner.transform.localRotation, Quaternion.identity, Time.deltaTime * 5f);
            transform.SetLocalPositionAndRotation(Vector3.Lerp(transform.localPosition, safetyWeaponPos, Time.deltaTime * 5f), 
                Quaternion.Lerp(transform.localRotation, safetyWeaponRot, Time.deltaTime * 5f));
        }
        else if (!owner.debugWeaponPosing && !owner.ctx.IsControllingHand) //if not doing actions above
        {
            Vector3 weaponDirection = owner.cursor.position + shootDirection +  - owner.ctx.cameraPosition.position;
            Quaternion tuckedRotation = Quaternion.Lerp(Quaternion.LookRotation(Vector3.up), Quaternion.identity, this.tuckedRotation / tuckedDistance);
            Quaternion targetRotation = Quaternion.LookRotation(weaponDirection, owner.ctx.cameraPosition.up) * tuckedRotation;
            Quaternion recoil = Quaternion.Euler(-shootDirection.magnitude * currentStats.spreadMultiplier, 0, 0);
            Vector3 tuckedOffset = Vector3.Lerp(new Vector3(0, 0, tuckedPosition), Vector3.zero, this.tuckedRotation / tuckedDistance);
            if (!CursorController.RMBhold)
            {
                float clamp = -owner.ctx.SmoothXRotation / 180f * 0.5f;
                owner.WpnManagerPosition = Mathf.Clamp(clamp, -1f, 0f);
                owner.transform.localRotation = Quaternion.Lerp(owner.transform.localRotation, Quaternion.AngleAxis(owner.ctx.XRotaion / 2f, Vector3.right), Time.deltaTime * 5f);

                Quaternion localSpaceRotation = Quaternion.Inverse(owner.weaponHolder.transform.rotation) * targetRotation;
                transform.localRotation = Quaternion.Lerp(transform.localRotation, localSpaceRotation * recoil, Time.deltaTime * 15f);
                owner.weaponHolder.localRotation = Quaternion.Lerp(owner.weaponHolder.localRotation, Quaternion.Euler(shootDirection), Time.deltaTime * 5f);
                transform.localPosition = Vector3.Lerp(transform.localPosition, recoilPosition + handPos + tuckedOffset, Time.deltaTime * 15f);
            }
            else
            {
                owner.WpnManagerPosition = Mathf.Lerp(owner.WpnManagerPosition, 0f, Time.deltaTime * 5f);
                transform.localPosition = Vector3.Lerp(transform.localPosition, currentStats.aimWeaponPos + recoilPosition + tuckedOffset, Time.deltaTime * 5f);
                transform.localRotation = Quaternion.Lerp(transform.localRotation, recoil, Time.deltaTime * 5f);
                owner.weaponHolder.rotation = Quaternion.Slerp(owner.weaponHolder.rotation, targetRotation, Time.deltaTime * 15f);
            }
        }
        else //if not doing all of this
        {
            owner.WpnManagerPosition = Mathf.Lerp(owner.WpnManagerPosition, 0f, Time.deltaTime * 5f);
            owner.transform.localRotation = Quaternion.Lerp(owner.transform.localRotation, Quaternion.identity, Time.deltaTime * 5f);
            transform.SetLocalPositionAndRotation(Vector3.Lerp(transform.localPosition, handPos, Time.deltaTime * 5f), 
                Quaternion.Lerp(transform.localRotation, Quaternion.identity, Time.deltaTime * 5f));
        }
    }

    protected virtual void Shoot()
    {
        readyToShot = false;
        shooting = true;
        // Получить случайный разброс на основе времени стрельбы
        shootDirection = transform.forward + GetSpread(LastShootTime);
        particleShooter.Emit(1);
        muzzleFlashParticle.Emit(1);
        SoundManager.Instance.CreateSoundBuilder().WithPosition(transform.position).Play(currentStats.shotSound);
        //GameObject bullet = ObjectPoolManager.SpawnObject(wpnBolt.Value.GetBullet(), muzzleFlashParticle.transform.position, Quaternion.identity);
        //bullet.GetComponent<BulletProjectile>().Setup(transform.forward, owner.transform, surfaceImpact, 300f, currentStats.hitForce, currentStats.damage);
        wpnBolt.Value.ConsumeBullet(1);
        bulletsPerTap--;
        LastShootTime += currentStats.shootingSpeed;
        recoilPosition.z -= currentStats.kickbackForce;
        float rand = Random.Range(-10f, 10f);
        if (!CursorController.RMBhold) transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z - rand);
        owner.ctx.recoilAngle.x = -Mathf.Clamp01(LastShootTime / currentStats.MaxSpreadTime) * 2;
        owner.ctx.recoilAngle.y = (bulletsShot % 2 != 0 ? -1f : 1f) * Mathf.Clamp01(LastShootTime / currentStats.MaxSpreadTime);
        PostProcessingController.ChangeRecoilVolumeValue(Mathf.Lerp(0f, 1f, LastShootTime / currentStats.MaxSpreadTime));
        Invoke(nameof(ResetShot), currentStats.shootingSpeed);
    }

    protected void ResetShot()
    {
        shooting = false;
        readyToShot = true;
        if (!CursorController.LMBhold || !wpnBolt.Value.AmmoLoaded)
        {
            StartCoroutine(nameof(CheckRecoilRecoveryCooldown));
            return;
        }
        else if (currentStats.allowButtonHold)
        {
            Shoot();
        }
    }

    IEnumerator CheckRecoilRecoveryCooldown()
    {
        LastShootTime = Mathf.Clamp01(LastShootTime);
        while (LastShootTime > 0f)
        {
            if (shooting) yield break;
            LastShootTime -= Time.deltaTime;
            PostProcessingController.ChangeRecoilVolumeValue(Mathf.Lerp(0f, 1f, LastShootTime / currentStats.MaxSpreadTime));
            yield return null;
        }
        yield return null;
    }


    public override void Pickup(PlayerIK playerIK, WeaponManagerFullBody wpnManager)
    {
        base.Pickup(playerIK, wpnManager);
        ControlHint.Instance.UpdateNumbersHint((int)weaponType);
        //wpnManager.ctx.cameraPosition.GetComponent<Volume>().profile.TryGet(out vignette);
    }

    public override void DropItem(PlayerIK playerIK, WeaponManagerFullBody wpnManager)
    {
        base.DropItem(playerIK, wpnManager);
        ControlHint.Instance.UpdateNumbersHint((int)weaponType);
        //if (owner != null) owner.ctx.RMBhold = false;
    }

    /*public override void PutDown(Transform slot, PlayerIK playerIK, WeaponManagerFullBody wpnManager)
    {
        if (!slot.TryGetComponent<InventorySlot>(out var invSlot)) return;
        if (invSlot.GetType().FullName == typeof(InventorySlot).FullName)
        {
            if (invSlot.OccupiedSlot != null) return;
            this.transform.parent = invSlot.slotPath;
            transform.SetLocalPositionAndRotation(invPos, invRot);
            invSlot.OccupiedSlot = this.transform;
            foreach (var col in colliders)
            {
                col.enabled = false;
            }
            wpnManager.HoldingItem = null;
            wpnManager.TargetItem = null;
            playerIK.SetupRig(wpnManager.itemRoot, 0);
        }
        if (magazineSlot != null) magazineSlot.gameObject.SetActive(true);
        if (owner != null) owner.ctx.TryToAim = false;
    }*/

    public IEnumerator BindWeapon(Transform slot, WeaponManagerFullBody wpnManager)
    {
        wpnManager.ctx.animator.SetFloat("inverse", 1);
        wpnManager.ctx.animator.SetBool("isUnequiping", true);
        wpnManager.IsBusy = true;
        foreach (var col in interactableColliders)
        {
            col.enabled = false;
        }
        transform.SetParent(wpnManager.ctx.animator.GetBoneTransform(HumanBodyBones.RightHand));
        switch (weaponType)
        {
            case WeaponType.Primary: wpnManager.ctx.animator.SetBool("isMainWeapon", true); break;
            case WeaponType.Secondary: wpnManager.ctx.animator.SetBool("isSecondWeapon", true); break;
            case WeaponType.Melee: wpnManager.ctx.animator.SetBool("isMeleeWeapon", true); break;
        }
        wpnManager.playerIK.ChangeRightHandIKValue(0f, 0.5f);
        wpnManager.playerIK.ChangeLeftHandIKValue(wpnManager.ctx.IsControllingHand ? 1 : 0, 0.5f);

        yield return new WaitForSeconds(1f);

        if (!wpnManager.ctx.IsControllingHand) wpnManager.playerIK.SetupRig(wpnManager.cursor, 0);

        if (!slot.TryGetComponent<WeaponInventorySlot>(out var invSlot)) yield break;
        if (invSlot.GetType().FullName == typeof(WeaponInventorySlot).FullName)
        {
            if (invSlot.OccupiedSlot != null) yield break;
            this.transform.SetParent(invSlot.slotPath);
            transform.SetLocalPositionAndRotation(invPos, invRot);
            invSlot.OccupiedSlot = this.transform;
            foreach (var col in colliders)
            {
                col.enabled = false;
            }
        }

        wpnManager.playerIK.ChangeRightHandIKValue(0f, 0.5f);

        yield return new WaitForSeconds(0.5f);

        switch (weaponType)
        {
            case WeaponType.Primary: wpnManager.ctx.animator.SetBool("isMainWeapon", false); break;
            case WeaponType.Secondary: wpnManager.ctx.animator.SetBool("isSecondWeapon", false); break;
            case WeaponType.Melee: wpnManager.ctx.animator.SetBool("isMeleeWeapon", false); break;
        }
        attachmentSystem?.DisableAttachments();
        wpnManager.ctx.animator.SetBool("isUnequiping", false);
        wpnManager.IsBusy = false;
        owner.ctx.animator.SetFloat("ArmedBlend", 0f, 0.5f, 1f);
        owner.playerIK.AcivateLeftFingersIK = false;
        owner.playerIK.AcivateRightFingersIK = false;
        //wpnBolt.magazineSlot.gameObject.SetActive(true);
        ControlHint.Instance.UpdateWeaponHint();
        ControlHint.Instance.UpdateNumbersHint((int)weaponType);
    }

    public virtual IEnumerator Equip(InventorySlot invSlot, WeaponManagerFullBody wpnOwner)
    {
        owner = wpnOwner;
        owner.ctx.animator.SetBool("isEquiping", true);
        owner.ctx.animator.SetFloat("inverse", 1);
        owner.IsBusy = true;

        switch (weaponType)
        {
            case WeaponType.Primary: owner.ctx.animator.SetBool("isMainWeapon", true); break;
            case WeaponType.Secondary: owner.ctx.animator.SetBool("isSecondWeapon", true); break;
            case WeaponType.Melee: owner.ctx.animator.SetBool("isMeleeWeapon", true); break;
        }

        yield return new WaitForSeconds(0.5f);
        owner.playerIK.SetupRig(ArmedPoseParent, owner.ctx.IsControllingHand || oneHandedWeapon ? 1 : 2);
        owner.playerIK.SetGunStyle(oneHandedWeapon);
        if (!owner.ctx.IsControllingHand)
        {
            if (!oneHandedWeapon) owner.playerIK.ChangeLeftHandIKValue(1f, 0.5f);
            owner.cursor.parent = transform;
        }
        owner.playerIK.ChangeRightHandIKValue(1f, 0.5f);
        yield return new WaitForSeconds(0.5f);
        switch (weaponType)
        {
            case WeaponType.Primary: owner.ctx.animator.SetBool("isMainWeapon", false); break;
            case WeaponType.Secondary: owner.ctx.animator.SetBool("isSecondWeapon", false); break;
            case WeaponType.Melee: owner.ctx.animator.SetBool("isMeleeWeapon", false); break;
        }
        transform.SetParent(owner.bothHandWeaponHolder);
        foreach (var col in interactableColliders)
        {
            col.enabled = true;
        }
        attachmentSystem?.ActivateAttachments();
        owner.HeldWeapon = this;
        owner.playerIK.AcivateLeftFingersIK = true;
        owner.playerIK.AcivateRightFingersIK = true;
        owner.ctx.animator.SetFloat("ArmedBlend", 1f, 1f, Time.deltaTime);
        owner.ctx.animator.SetBool("isEquiping", false);
        invSlot.OccupiedSlot = null;
        owner.IsBusy = false;
        ControlHint.Instance.UpdateWeaponHint();
        ControlHint.Instance.UpdateNumbersHint((int)weaponType);
    }

    public void Unequip(Transform transformParent)
    {
        //weaponBoltCarrier.enabled = false;
        foreach (var col in interactableColliders)
        {
            col.enabled = false;
        }
        transform.parent = transformParent;
    }

    public Vector3 GetSpread(float ShootTime = 0)
    {
        var maxSpread = CursorController.RMBhold ? currentStats.Spread / 2f : currentStats.Spread;
        Vector3 spread = Vector3.Lerp(
            new Vector3(
                Random.Range(-currentStats.MinSpread.x, currentStats.MinSpread.x),
                Random.Range(-currentStats.MinSpread.y, currentStats.MinSpread.y),
                Random.Range(-currentStats.MinSpread.z, currentStats.MinSpread.z)
            ),
            new Vector3(
                Random.Range(-maxSpread.x, maxSpread.x),
                Random.Range(-maxSpread.y, maxSpread.y),
                Random.Range(-maxSpread.z, maxSpread.z)
            ),
            Mathf.Clamp01(ShootTime / currentStats.MaxSpreadTime)
        );
        if (currentStats.absoluteSpreadValue) spread.y = Mathf.Abs(spread.y); 
        return spread;
    }

    public virtual void Reload(bool i)
    {
        owner.Reloading = i;
        owner.ctx.animator.SetBool("isReloading", owner.Reloading);
    }

    public void SwitchHand()
    {
        owner.playerIK.ControlRightHand = !owner.playerIK.ControlRightHand;
    }

    public virtual void OnHandSwitchTrigger(bool isControllingHand)
    {
        CursorController.EnableCursor(isControllingHand);
        if (isControllingHand)
        {
            if (owner.playerIK.ControlRightHand) owner.playerIK.SetupRig(owner.cursor, 1);
            else owner.playerIK.SetupRig(owner.cursor, 0);
            owner.playerIK.AcivateLeftFingersIK = true;
            if (oneHandedWeapon) owner.playerIK.ChangeLeftHandIKValue(1f, 0.5f);
            owner.cursor.parent = owner.ctx.cameraPosition;
        }
        else
        {
            owner.playerIK.AcivateLeftFingersIK = !oneHandedWeapon;
            owner.playerIK.AcivateRightFingersIK = true;
            if (oneHandedWeapon) owner.playerIK.ChangeLeftHandIKValue(0f, 0.5f);
            else owner.playerIK.SetupRig(ArmedPoseParent, 0);
            owner.cursor.parent = transform;
        }
    }

    public IEnumerator AssignWeaponToSlot(Transform slotTransform, WeaponManagerFullBody wpnManager)
    {
        wpnManager.ctx.animator.SetBool("isBinding", true);
        transform.SetParent(wpnManager.itemPivot);
        wpnManager.HoldingItem = null;
        wpnManager.IsBusy = true;
        switch (weaponType)
        {
            case WeaponType.Primary: wpnManager.ctx.animator.SetBool("isMainWeapon", true); break;
            case WeaponType.Secondary: wpnManager.ctx.animator.SetBool("isSecondWeapon", true); break;
            case WeaponType.Melee: wpnManager.ctx.animator.SetBool("isMeleeWeapon", true); break;
        }
        wpnManager.playerIK.ChangeLeftHandIKValue(0f, 0.5f);

        yield return new WaitForSeconds(1f);

        //holdingWeapon.PutDown(slot, playerIK, this);
        var slot = slotTransform.GetComponent<InventorySlot>();
        transform.parent = slot.slotPath;
        slot.PutDownToSlot(transform, wpnManager);
        wpnManager.playerIK.ChangeLeftHandIKValue(1f, 2f);

        yield return new WaitForSeconds(0.5f);

        switch (weaponType)
        {
            case WeaponType.Primary: wpnManager.ctx.animator.SetBool("isMainWeapon", false); break;
            case WeaponType.Secondary: wpnManager.ctx.animator.SetBool("isSecondWeapon", false); break;
            case WeaponType.Melee: wpnManager.ctx.animator.SetBool("isMeleeWeapon", false); break;
        }
        wpnManager.ctx.animator.SetFloat("ArmedBlend", 0f, 0.5f, 1f);
        wpnManager.ctx.animator.SetBool("isBinding", false);
        wpnManager.IsBusy = false;
        ControlHint.Instance.UpdateNumbersHint((int)weaponType);
        ControlHint.Instance.UpdateWeaponHint();
    }

    public void UpdateCursorPositionOverride(Transform cursor, Camera playerCamera, Vector3 mousePosition)
    {
        float swayPositionX = Mathf.Clamp(CursorController.MouseAxis.x * CursorController.swaySensitivity, 
            -CursorController.SwayBoundsX, CursorController.SwayBoundsX);
        float swayPositionY = Mathf.Clamp(CursorController.MouseAxis.y * CursorController.swaySensitivity,
            -CursorController.SwayBoundsX, CursorController.SwayBoundsX);
        Vector3 position = new Vector3(mousePosition.x + swayPositionX, mousePosition.y + swayPositionY, 1.5f);
        
        cursor.position = Vector3.Lerp(cursor.position, playerCamera.ScreenToWorldPoint(position), 1.0f - Mathf.Exp(-10f * Time.deltaTime));
    }

    public virtual void OnSprintingState(bool i)
    {
        if (!liberateLeftHand || owner.Reloading) return;
        if (i) owner.playerIK.ChangeLeftHandIKValue(0f, 1f);
        else owner.playerIK.ChangeLeftHandIKValue(1f, 1f);
    }

    public override void DropWeapon()
    {
        CursorController.EnableCursor(owner.ctx.IsControllingHand);
        base.DropWeapon();
        ControlHint.Instance.UpdateWeaponHint();
        ControlHint.Instance.UpdateNumbersHint((int)weaponType);
    }
}
