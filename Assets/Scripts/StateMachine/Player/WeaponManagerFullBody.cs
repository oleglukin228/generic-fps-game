using AudioSystem;
using LlamAcademy.Guns.Demo;
using System.Collections;
using UnityEngine;

public class WeaponManagerFullBody : MonoBehaviour
{
    public PlayerHandScript handScript;
    public PlayerController ctx;
    public Transform cursor;
    public Transform weaponHolder;
    public WeaponInventorySlot[] weaponSlots;
    public InventorySlot[] inventorySlots;

    [Header("Animation Rigs")]
    public bool debugWeaponPosing;
    public Transform itemPivot;
    public PlayerIK playerIK;
    public AnimatorOverrideController armedOverride;
    public Transform bothHandWeaponHolder;

    [Header("Weapon Pickup Settings")]
    public float pickupRange;
    public float pickupRadius;
    public int weaponLayer;

    [Header("Weapon sway and bobbing")]
    public bool sway = true;
    public bool bobRotaion = true;
    public bool bobOffset = true;
    public bool bobSway = true;
    public float bobExaggeration;
    public float speedCurve;
    public Vector3 multiplier;
    float curveSin { get => Mathf.Sin(speedCurve); }
    float curveCos { get => Mathf.Cos(speedCurve); }
    public Vector3 travelLimit = Vector3.one * 0.025f;
    public Vector3 bobLimit = Vector3.one * 0.01f;
    Vector3 bobEulerRot;
    Vector3 swayEulerRot;
    Vector3 bobPosition;
    Vector3 swayPos;

    Transform holdingItem;
    float wpnManagerPosition;
    float sphereCastRadius = 0.025f;
    int selectedWeapon = -1;
    int previousWeapon;
    bool isBusy = false;
    bool reloading;
    Ray ray;
    RaycastHit hit;
    IPickableWeaponInterface _heldWeapon;
    IPickableInterface targetItem;
    float distanceToSlot;
    Transform targetedInvSlot;
    Transform prevTargetedInvSlot;
    //LayerMask mask;
    private int _isEquipingTrigger = Animator.StringToHash("isEquiping");
    private int _isUnequipingTrigger = Animator.StringToHash("isUnequiping");
    private int _isControllingHandTrigger = Animator.StringToHash("isControllingHand");
    private int _isMovingTrigger = Animator.StringToHash("isMoving");
    public float WpnManagerPosition { get { return wpnManagerPosition; } set { wpnManagerPosition = value; } }
    public Transform HoldingItem { get { return holdingItem; } set { holdingItem = value; } }
    public IPickableWeaponInterface HeldWeapon { get { return _heldWeapon; } set { _heldWeapon = value; } }
    public bool IsBusy { get { return isBusy; } set { isBusy = value; } }
    public bool Reloading { get { return reloading; } set { reloading = value; } }
    public IPickableInterface TargetItem { get { return targetItem; } set {  targetItem = value; } }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        WeaponControl();
        SwitchWeaponInput();
        HandControl();
        transform.position = ctx.headPivot.position + (ctx.transform.up * wpnManagerPosition);
    }

    private void LateUpdate()
    {
        if (ctx.IsControllingHand)
        {
            if (holdingItem != null)
            {
                targetItem.MoveItem(cursor);
                if (targetedInvSlot != null)
                    distanceToSlot = Vector3.Distance(holdingItem.position, targetedInvSlot.position);
            }
        }
    }

    private void HandHelper()
    {
        var t = 1.0f - Mathf.Exp(-10f * Time.deltaTime);
        LayerMask mask = holdingItem == null ? LayerMask.GetMask("UI", "Pickable") : LayerMask.GetMask("UI", "Insertable");
        if (Physics.SphereCast(ctx.cameraPosition.position, sphereCastRadius, ray.direction, out hit, 1f, mask))
        {
            if (hit.rigidbody.gameObject.layer == LayerMask.NameToLayer("Pickable") && holdingItem == null)
            {
                cursor.position = Vector3.Lerp(cursor.position, hit.rigidbody.position, t);
            }
            else if (hit.rigidbody.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                cursor.position = Vector3.Lerp(cursor.position, hit.rigidbody.position, t);
            }
            else if (hit.rigidbody.gameObject.layer == LayerMask.NameToLayer("Insertable"))
            {
                cursor.position = Vector3.Lerp(cursor.position, hit.rigidbody.position, t);
            }
            else
                cursor.position = Vector3.Lerp(cursor.position, ctx.PlayerCamera.ScreenToWorldPoint(new Vector3(CursorController.MousePosition.x, CursorController.MousePosition.y, 0.4f)), t);
        }
        else
        {
            cursor.position = Vector3.Lerp(cursor.position, ctx.PlayerCamera.ScreenToWorldPoint(new Vector3(CursorController.MousePosition.x, CursorController.MousePosition.y, 0.4f)), t);
        }
    }

    private void OnDrawGizmos()
    {
        if (hit.rigidbody == null)
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.green;
        Gizmos.DrawSphere(cursor.position, sphereCastRadius);
    }

    public void HandControl()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (holdingItem == null && !isBusy)
            {
                ctx.IsControllingHand = !ctx.IsControllingHand;
                ctx.animator.SetBool(_isControllingHandTrigger, ctx.IsControllingHand);
                int leftIKValue = ctx.IsControllingHand && !playerIK.ControlRightHand ? 1 : _heldWeapon != null ? 1 : 0;
                int rightIKValue = ctx.IsControllingHand && playerIK.ControlRightHand ? 1 : _heldWeapon != null ? 1 : 0;
                if (_heldWeapon == null)
                {
                    CursorController.EnableCursor(ctx.IsControllingHand);
                    if (ctx.IsControllingHand)
                    {
                        if (playerIK.ControlRightHand) playerIK.SetupRig(cursor, 1);
                        else playerIK.SetupRig(cursor, 0);
                        playerIK.AcivateLeftFingersIK = true;
                        cursor.parent = ctx.cameraPosition;
                        playerIK.ChangeLeftHandIKValue(leftIKValue, 1f);
                        playerIK.ChangeRightHandIKValue(rightIKValue, 0.5f);
                    }
                    else
                    {
                        playerIK.AcivateLeftFingersIK = false;
                        playerIK.ChangeLeftHandIKValue(leftIKValue, 1f);
                        playerIK.ChangeRightHandIKValue(rightIKValue, 0.5f);
                    }
                }
                else
                {
                    _heldWeapon.OnHandSwitchTrigger(ctx.IsControllingHand);
                }
                cursor.rotation = cursor.parent.rotation;
                ControlHint.Instance.UpdateEKeyHint();
            }
        }

        ray = ctx.PlayerCamera.ScreenPointToRay(CursorController.MousePosition);
        if (ctx.IsControllingHand)
        {
            HandHelper();

            float normalizedX = CursorController.MousePosition.x / Screen.width;
            playerIK.ElbowHintPosition = normalizedX * 0.5f - 0.125f;

            targetedInvSlot = hit.collider == null ? null : hit.collider.GetComponent<InventorySlot>() && !CursorController.IsInteracting ? hit.collider.transform : null;
            //targetedInvSlot = hit.collider == null ? null : hit.rigidbody.gameObject.layer == LayerMask.GetMask("UI") ? hit.collider.transform : null;

            if (isBusy) return;
            if (Input.GetMouseButton(0) && hit.collider != null && holdingItem == null)
            {
                StartCoroutine(PickupItem(hit));
            }
            else if (!debugWeaponPosing && !Input.GetMouseButton(0) && targetedInvSlot != null && holdingItem != null && distanceToSlot <= 0.05f)
            {
                if (targetedInvSlot.gameObject.layer == LayerMask.NameToLayer("UI"))
                {
                    targetedInvSlot.GetComponent<InventorySlot>().PutDownToSlot(holdingItem, this);
                }
            }
            else if (!debugWeaponPosing && !Input.GetMouseButton(0) && holdingItem != null)
            {
                holdingItem.GetComponent<IPickableInterface>().DropItem(playerIK, this);
            }

            if (targetedInvSlot != prevTargetedInvSlot)
            {
                InventorySlot slot = null;

                if (targetedInvSlot != null)
                    slot = targetedInvSlot.GetComponent<InventorySlot>();

                if (holdingItem != null)
                {
                    var item = holdingItem.GetComponent<PickableStuff>();
                    if (slot == null || item == null) return;
                    //StopCoroutine(nameof(ApplyItemRotation));
                    /*if (targetedInvSlot.CompareTag("WeaponMagazineSlot") && item.TryGetComponent<WeaponMagazine>(out var mag))
                        StartCoroutine(mag.ApplyItemRotation(this, targetedInvSlot, mag.magRot));
                    else
                        StartCoroutine(item.ApplyItemRotation(this, targetedInvSlot, item.invRot));*/
                    //if (slot.OccupiedSlot == null && !holdingItem.GetComponent<InteractableEnvironment>()) holdingItem.parent = targetedInvSlot.transform;
                }
                else
                {
                    if (slot != null && slot.OccupiedSlot != null) playerIK.SetupRig(slot.OccupiedSlot.GetComponent<PickableStuff>().PickupPoseParent, 0);
                    else if (slot == null && targetItem == null) playerIK.SetupRig(cursor, 0);
                }
                prevTargetedInvSlot = targetedInvSlot;
            }
        }
        else
        {
            if (_heldWeapon != null)
            {
                _heldWeapon.UpdateCursorPositionOverride(cursor, ctx.PlayerCamera, CursorController.MousePosition);
            }
            else
            {
                cursor.position = Vector3.Lerp(cursor.position,
                    ctx.PlayerCamera.ScreenToWorldPoint(new Vector3(CursorController.MousePosition.x, CursorController.MousePosition.y, 1.5f)),
                    1.0f - Mathf.Exp(-10f * Time.deltaTime));
                //cursor.position = ctx.PlayerCamera.ScreenToWorldPoint(new Vector3(CursorController.MousePosition.x, CursorController.MousePosition.y, 1.5f));
            }
        }
    }

    IEnumerator PickupItem(RaycastHit hit)
    {
        var itemPosition = hit.collider.transform;

        if (!ctx.IsControllingHand) yield break;
        if (itemPosition.gameObject.layer == LayerMask.NameToLayer("Pickable"))
        {
            isBusy = true;
            var distance = Vector3.Distance(hit.point, cursor.position);
            distance = Mathf.Clamp(distance, 0, 0.2f);
            float timeElapsed = 0;
            while (timeElapsed < distance)
            {
                float t = timeElapsed / distance;
                cursor.position = Vector3.Lerp(cursor.position, itemPosition.position, t);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            isBusy = false;
            itemPosition.GetComponent<IPickableInterface>().Pickup(playerIK, this);
        }
        else if (itemPosition.gameObject.layer == LayerMask.NameToLayer("UI"))
        {
            itemPosition.TryGetComponent<InventorySlot>(out var slot);
            if (slot.OccupiedSlot != null)
            {
                isBusy = true;
                var distance = Vector3.Distance(hit.point, cursor.position);
                distance = Mathf.Clamp(distance, 0, 0.2f);
                float timeElapsed = 0;
                while (timeElapsed < distance)
                {
                    float t = timeElapsed / distance;
                    cursor.position = Vector3.Lerp(cursor.position, itemPosition.position, t);
                    timeElapsed += Time.deltaTime;
                    yield return null;
                }
                isBusy = false;
                slot.PickupItemFromSlot(this);
            }
        }
    }

    void WeaponControl()
    {
        if (CursorController.RMBup)
        { 
            CursorController.ResetSensitivity(); 
            PostProcessingController.ChangeADSVolumeValue(0f);
        }
        if (_heldWeapon != null)
        {
            if (CursorController.RMBdown)
            {
                CursorController.MultiplySensitivity(_heldWeapon.CurrentStats.aimSensitivityMultiplier);
                PostProcessingController.ChangeADSVolumeValue(1f);
            } 
            _heldWeapon.UpdateWeapon();
            //if (_heldWeapon.Reloading) return;
            WeaponSway();
            WeaponBobOffset();
            WeaponBobRotation();
            CompositePositionRotation();
        }
    }

    void WeaponSway()
    {
    }

    void WeaponBobRotation()
    {
        if (bobSway == false) { bobEulerRot = Vector3.zero; return; }

        bobEulerRot.x = (ctx.InputDirection != Vector2.zero ? multiplier.x * (Mathf.Sin(2 * speedCurve)) :
                                                                   multiplier.x * (Mathf.Sin(2 * speedCurve) / 2));
        bobEulerRot.y = (ctx.InputDirection != Vector2.zero ? multiplier.y * curveCos : 0);
        bobEulerRot.z = (ctx.InputDirection != Vector2.zero ? multiplier.z * curveCos * ctx.InputDirection.x : 0);
    }

    void WeaponBobOffset()
    {
        //speedCurve += Time.deltaTime * (ctx.animator.GetBool("isMoving") ? ctx.CurrentSpeed : 1f);
        speedCurve += ctx.animator.GetBool(_isMovingTrigger) ? Time.deltaTime * bobExaggeration * ctx.animator.velocity.magnitude : Time.deltaTime * 1f;

        if (CursorController.RMBhold || bobOffset == false) { bobPosition = Vector3.zero; return; }

        //bobPosition.x = (curveCos * bobLimit.x * (ctx.IsGrounded ? 1 : 0) - (ctx.InputDirection.x * travelLimit.x));
        //bobPosition.y = (curveSin * bobLimit.y) - (ctx.animator.velocity.y * travelLimit.y);
        //bobPosition.z = -(ctx.InputDirection.y * travelLimit.z);

        bobPosition.x = (Mathf.Cos(speedCurve) * bobLimit.x) - (ctx.animator.angularVelocity.x * travelLimit.x);
        bobPosition.y = (Mathf.Sin(speedCurve * 2) * bobLimit.y) - (ctx.animator.angularVelocity.y * travelLimit.y);
        bobPosition.z = -(ctx.animator.angularVelocity.y * travelLimit.z);
    }

    float smooth = 10f;
    float smoothRot = 12f;
    void CompositePositionRotation()
    {
        weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, swayPos + bobPosition, Time.deltaTime * smooth);
        //weaponHolder.localRotation = Quaternion.Slerp(weaponHolder.localRotation, Quaternion.Euler(swayEulerRot) * Quaternion.Euler(bobEulerRot), Time.deltaTime * smoothRot);
    }

    void SwitchWeaponFunction(int i)
    {
        if (ctx.animator.GetBool(_isEquipingTrigger)) return;
        if (ctx.animator.GetBool(_isUnequipingTrigger)) return;
        
        if (weaponSlots[i].OccupiedSlot == null)
        {
            if (_heldWeapon == null)
            {
                if (holdingItem.TryGetComponent<IPickableWeaponInterface>(out var weapon))
                    if (weaponSlots[i].weaponType == weapon.GetWeaponType)
                        StartCoroutine(weapon.AssignWeaponToSlot(weaponSlots[i].transform, this));
            }
            else
            {
                if (weaponSlots[i].weaponType == _heldWeapon.GetWeaponType)
                {
                    StartCoroutine(_heldWeapon.BindWeapon(weaponSlots[i].transform, this));
                    _heldWeapon = null;
                }
            }
        }
        else if (weaponSlots[i].OccupiedSlot != null)
        {
            if (_heldWeapon == null)
            {
                if (weaponSlots[i].OccupiedSlot.TryGetComponent<IPickableWeaponInterface>(out var weapon))
                    StartCoroutine(weapon.Equip(weaponSlots[i], this));
            }
        }
    }

    void SwitchWeaponInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ///primaty weapon
        {
            if (!isBusy)
            {
                SwitchWeaponFunction(0);
                previousWeapon = selectedWeapon;
            } 
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) ///secondary weapon
        {
            if (!isBusy)
            {
                SwitchWeaponFunction(1);
                previousWeapon = selectedWeapon;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) ///melee weapon
        {
            if (!isBusy)
            {
                SwitchWeaponFunction(2);
                previousWeapon = selectedWeapon;
            }
        }
    }

    public void ChangeCursorPosition(Vector3 position)
    {
        //mousePosition = ctx.PlayerCamera.WorldToScreenPoint(position);
        cursor.position = position;
    }

    public void OnRapedMethod()
    {
        isBusy = true;
        ctx.IsControllingHand = false;
        ctx.animator.SetBool(_isControllingHandTrigger, ctx.IsControllingHand);
        foreach (var weaponSlot in weaponSlots)
        {
            if (weaponSlot.OccupiedSlot == null) continue;
            weaponSlot.OccupiedSlot.GetComponent<IPickableInterface>().DropItem(playerIK, this);
        }
        foreach (var slot in inventorySlots)
        {
            if (slot.OccupiedSlot == null) continue;
            slot.OccupiedSlot.GetComponent<IPickableInterface>().DropItem(playerIK, this);
        }
        targetItem?.DropItem(playerIK, this);
        _heldWeapon?.DropWeapon();
    }
}
