using Kineractive;
using LlamAcademy.Guns.Demo;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.GridLayoutGroup;
using static UnityEngine.UI.Image;

public class MeleeItem : WeaponItem, IPickableInterface, IPickableWeaponInterface
{
    [Header("In hand while attacking")]
    public Vector3 attackPos;
    public Quaternion attackRot = Quaternion.Euler(45f, 0f, 0f);

    [Header("Shooting stat")]
    public float damage;
    public float hitForce;
    public float swingSpeed = 1.25f;
    public float bladeSideRotation = 30f;
    bool isSwinging;
    float swingFactor = 0.3f;

    [Header("Data")]
    public GameObject swingParticle;
    public float distanceThreshold = 0.15f;
    public Vector3 hurtBoxSize = new Vector3(0.1f, 0.1f, 0.6f);
    private Vector3 lastEmitPosition;
    [SerializeField] LayerMask ballisticLayersToHit;
    public WeaponType GetWeaponType => weaponType;

    public WeaponStatsSO CurrentStats => throw new System.NotImplementedException();

    private void Start()
    {
        TryGetComponent<Rigidbody>(out _rb);
        TryGetComponent<InteractableActions>(out interactableActions);
        if (itemParent == null) itemParent = transform;
    }

    public virtual void UpdateWeapon()
    {
        //owner.ctx.LMBhold = Input.GetKey(KeyCode.Mouse0);
        if (Input.GetKeyDown(KeyCode.G))
        {
            CursorController.EnableCursor(owner.ctx.IsControllingHand);
            DropWeapon();
            ControlHint.Instance.UpdateWeaponHint();
            ControlHint.Instance.UpdateNumbersHint((int)weaponType);
            return;
        }

        WeaponSway();
        if (isSwinging)
        {
            float distanceMoved = Vector3.Distance(transform.position, lastEmitPosition);
            if (distanceMoved >= distanceThreshold)
            {
                EmitHurtBox();
                lastEmitPosition = transform.position;
            }
        }
        else
        {
            lastEmitPosition = transform.position;
        }
    }

    private void EmitHurtBox()
    {
        if (!isSwinging) return;
        if (Physics.BoxCast(owner.weaponHolder.position, hurtBoxSize / 2f, owner.weaponHolder.forward, out var hit, owner.weaponHolder.rotation, hurtBoxSize.z, ballisticLayersToHit))
        {
            Vector3 spawnPos = hit.point + hit.normal.normalized * 0.001f;
            SurfaceManager.Instance.SpawnEffect(spawnPos, Quaternion.FromToRotation(Vector3.forward, hit.normal), -hit.normal, hit.collider, surfaceImpact, hit.transform);

            if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
                damageable.TakeDamage(damage, hit.point, hit.point);
            else if (hit.collider.TryGetComponent<Rigidbody>(out var rb))
                rb.AddForceAtPosition(hitForce * hit.normal, hit.point, ForceMode.Impulse);
        }
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

    void WeaponSway()
    {
        if (!owner.ctx.IsControllingHand) 
        {
            Vector2 input = new Vector2(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));
            swingFactor = Mathf.Lerp(swingFactor, isSwinging ? 1f : 5f, Time.deltaTime * 5f);

            var cursorMovement = CursorController.MouseAxis * CursorController.aimSensitivity;

            if (CursorController.LMBdown)
            {
                swingParticle.SetActive(true);
                CursorController.EnableCursor(true);
            } 
            else if (CursorController.LMBup)
            {
                swingParticle.SetActive(false);
                CursorController.EnableCursor(false);
            } 

            if (CursorController.LMBhold)
            {
                if (cursorMovement.magnitude > 1) isSwinging = true;
                else isSwinging = false;

                owner.ctx.recoilAngle = input;
                float currentRotationAngle = isSwinging ? Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg : 0f;
                float bladeSide = input.y > 0 ? bladeSideRotation : -bladeSideRotation;
                float bladeRotation = isSwinging ? attackRot.eulerAngles.x : 0f;
                Vector3 weaponDirection = owner.cursor.position - owner.ctx.cameraPosition.position;
                Quaternion bladeLocalRotation = Quaternion.Euler(0f, 0f, currentRotationAngle);
                Quaternion targetRotation = Quaternion.LookRotation(weaponDirection, owner.ctx.cameraPosition.up);
                Quaternion localSpaceRotation = Quaternion.Inverse(owner.transform.rotation) * targetRotation;
                owner.weaponHolder.localRotation = Quaternion.Lerp(owner.weaponHolder.localRotation, localSpaceRotation * bladeLocalRotation, Time.deltaTime * 15f);
                transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(bladeRotation, bladeSide, 0f), Time.deltaTime * 15f);
                transform.localPosition = Vector3.Lerp(transform.localPosition, attackPos, Time.deltaTime * 15f);
            }
            else
            {
                Vector3 weaponDirection = owner.cursor.position + -owner.ctx.cameraPosition.position;
                Quaternion targetRotation = Quaternion.LookRotation(weaponDirection, owner.ctx.cameraPosition.up);

                owner.WpnManagerPosition = Mathf.Lerp(owner.WpnManagerPosition, 0f, Time.deltaTime * 5f);
                transform.localRotation = Quaternion.Lerp(transform.localRotation, handRot, Time.deltaTime * 15f);
                transform.localPosition = Vector3.Lerp(transform.localPosition, handPos, Time.deltaTime * 15f);
                owner.weaponHolder.rotation = Quaternion.Slerp(owner.weaponHolder.rotation, targetRotation, Time.deltaTime * 15f);

                isSwinging = false;
            }
        }
        else
        {
            owner.weaponHolder.localRotation = Quaternion.Lerp(owner.weaponHolder.localRotation, Quaternion.identity, Time.deltaTime * 15f);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, Time.deltaTime * 15f);
            transform.localPosition = Vector3.Lerp(transform.localPosition, handPos, Time.deltaTime * 5f);
            isSwinging = false;
        }
    }

    public void Unequip(Transform transformParent)
    {
        transform.parent = transformParent;
    }

    public void Reload(bool i)
    {
        
    }

    public void OnHandSwitchTrigger(bool isControllingHand)
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

    public IEnumerator BindWeapon(Transform slot, WeaponManagerFullBody wpnManager)
    {
        wpnManager.ctx.animator.SetBool("isArmed", false);
        wpnManager.ctx.animator.SetFloat("inverse", -1);
        wpnManager.ctx.animator.SetBool("isUnequiping", true);
        wpnManager.IsBusy = true;
        foreach (var col in interactableColliders)
        {
            col.enabled = false;
        }
        foreach (var col in colliders)
        {
            col.enabled = false;
        }
        owner.cursor.parent = owner.ctx.cameraPosition;
        CursorController.EnableCursor(wpnManager.ctx.IsControllingHand);
        transform.parent = wpnManager.ctx.animator.GetBoneTransform(HumanBodyBones.RightHand);
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
            this.transform.parent = invSlot.slotPath;
            transform.SetLocalPositionAndRotation(invPos, invRot);
            invSlot.OccupiedSlot = this.transform;
            gameObject.layer = LayerMask.NameToLayer("Melee");
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
        owner.playerIK.AcivateRightFingersIK = false;
        owner.weaponHolder.localRotation = Quaternion.identity;
        wpnManager.ctx.animator.SetBool("isUnequiping", false);
        wpnManager.IsBusy = false;
        ControlHint.Instance.UpdateWeaponHint();
        ControlHint.Instance.UpdateNumbersHint((int)weaponType);
    }

    public IEnumerator Equip(InventorySlot invSlot, WeaponManagerFullBody wpnOwner)
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
        //StartCoroutine(owner.playerIK.ChangeRightHandIKValue(1f, 0.5f));
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
        foreach (var col in colliders)
        {
            col.enabled = true;
        }
        owner.HeldWeapon = this;
        owner.ctx.animator.SetBool("isEquiping", false);
        owner.playerIK.AcivateLeftFingersIK = true;
        owner.playerIK.AcivateRightFingersIK = true;
        owner.transform.localRotation = Quaternion.identity;
        transform.localRotation = Quaternion.identity;
        owner.weaponHolder.localRotation = Quaternion.identity;
        owner.WpnManagerPosition = 0f;
        gameObject.layer = LayerMask.NameToLayer("Melee");
        _rb.useGravity = false;
        _rb.isKinematic = false;
        _rb.constraints = RigidbodyConstraints.FreezeAll;
        //owner.ctx.animator.SetBool("isArmed", true);
        invSlot.OccupiedSlot = null;
        owner.IsBusy = false;
        ControlHint.Instance.UpdateWeaponHint(false);
        ControlHint.Instance.UpdateNumbersHint((int)weaponType);
        //CursorController.EnableCursor(false);
    }

    public IEnumerator AssignWeaponToSlot(Transform slotTransform, WeaponManagerFullBody wpnManager)
    {
        wpnManager.ctx.animator.SetBool("isBinding", true);
        wpnManager.ctx.animator.SetFloat("inverse", 1);
        transform.parent = wpnManager.itemPivot;
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
        wpnManager.playerIK.ChangeLeftHandIKValue(1f, 0.5f);

        yield return new WaitForSeconds(0.5f);

        switch (weaponType)
        {
            case WeaponType.Primary: wpnManager.ctx.animator.SetBool("isMainWeapon", false); break;
            case WeaponType.Secondary: wpnManager.ctx.animator.SetBool("isSecondWeapon", false); break;
            case WeaponType.Melee: wpnManager.ctx.animator.SetBool("isMeleeWeapon", false); break;
        }
        wpnManager.ctx.animator.SetBool("isBinding", false);
        wpnManager.IsBusy = false;
        ControlHint.Instance.UpdateWeaponHint();
        ControlHint.Instance.UpdateNumbersHint((int)weaponType);
        CursorController.EnableCursor(wpnManager.ctx.IsControllingHand);
    }

    public void UpdateCursorPositionOverride(Transform cursor, Camera playerCamera, Vector3 mousePosition)
    {
        if (CursorController.LMBhold)
        {
            swingFactor = Mathf.Clamp(swingFactor, 0f, 0.5f);
            Vector3 position = new Vector3(mousePosition.x * swingSpeed * swingFactor, mousePosition.y * swingSpeed * swingFactor, 0.5f);
            cursor.position = Vector3.Lerp(cursor.position, playerCamera.ScreenToWorldPoint(position), 10 * Time.deltaTime);
        }
        else
        {
            float swayPositionX = Mathf.Clamp(CursorController.MouseAxis.x * CursorController.swaySensitivity,
            -CursorController.SwayBoundsX, CursorController.SwayBoundsX);
            float swayPositionY = Mathf.Clamp(CursorController.MouseAxis.y * CursorController.swaySensitivity,
                -CursorController.SwayBoundsX, CursorController.SwayBoundsX);
            Vector3 position = new Vector3(mousePosition.x + swayPositionX, mousePosition.y + swayPositionY, 1.5f);

            cursor.position = Vector3.Lerp(cursor.position, playerCamera.ScreenToWorldPoint(position), 1.0f - Mathf.Exp(-10f * Time.deltaTime));
        }
    }

    /*private void OnCollisionEnter(Collision collision)
    {
        if (owner == null) return;
        if (!isSwinging) return;
        RaycastHit hit;

        var contacts = collision.contacts[0];
        Vector3 direction = contacts.point - owner.transform.position;


        if (Physics.Raycast(owner.transform.position, direction, out hit, 2f, ballisticLayersToHit))
        {
            Vector3 spawnPos = contacts.point + contacts.normal.normalized * 0.001f;
            SurfaceManager.Instance.SpawnEffect(spawnPos, Quaternion.FromToRotation(Vector3.forward, hit.normal), -hit.normal, hit.collider, surfaceImpact, hit.transform);

            if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
                damageable.TakeDamage(damage, hit.point, hit.point);
            else if (hit.collider.TryGetComponent<Rigidbody>(out var rb))
                rb.AddForceAtPosition(hitForce * hit.normal, hit.point, ForceMode.Impulse);
        }
    }*/

    public void OnSprintingState(bool i)
    {
        
    }

    public override void DropWeapon()
    {
        CursorController.EnableCursor(owner.ctx.IsControllingHand);
        base.DropWeapon();
        ControlHint.Instance.UpdateWeaponHint();
        ControlHint.Instance.UpdateNumbersHint((int)weaponType);
    }

    private void OnDrawGizmosSelected()
    {
        if (owner == null) return;

        Gizmos.color = Color.red;

        // Устанавливаем матрицу трансформации, чтобы коробка учитывала поворот игрока
        Gizmos.matrix = Matrix4x4.TRS(owner.weaponHolder.position, owner.weaponHolder.rotation, Vector3.one);

        // Начальная коробка
        Gizmos.DrawWireCube(Vector3.zero, hurtBoxSize);

        // Конечная коробка после протяжки на дистанцию
        Vector3 endPoint = Vector3.forward * 1;
        Gizmos.DrawWireCube(endPoint, hurtBoxSize);

        // Соединяем их линиями для наглядности объема
        Gizmos.DrawLine(new Vector3(-hurtBoxSize.x / 2, 0, 0), new Vector3(-hurtBoxSize.x / 2, 0, 1));
        Gizmos.DrawLine(new Vector3(hurtBoxSize.x / 2, 0, 0), new Vector3(hurtBoxSize.x / 2, 0, 1));
    }
}
