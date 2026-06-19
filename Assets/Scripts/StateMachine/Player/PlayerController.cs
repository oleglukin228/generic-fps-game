using EZCameraShake;
using Kryz.CharacterStats;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerController : MonoBehaviour
{
    [Header("Camera parameters")]
    public Transform cameraRoot;
    public Transform cameraPosition;
    public Transform headPivot;
    public WeaponManagerFullBody weaponManager;
    public CameraShakeInstance screenInstance;

    [Header("Movement parameters")]
    float targetSpeed = 5f;
    float currentSpeed;
    public float characterWeight = 2.0f;
    public CharacterStat acceleration;
    public CharacterStat defaultSpeed;
    public CharacterStat sprintSpeedMultiplier;
    public CharacterStat crouchSpeedMultiplier;
    public float stepHeight = 0.2f;
    public float stepCheckDistance = 1.5f;

    [Header("Jumping parameters")]
    public float gravity = -9.8f;
    public float jumpHeight = 3f;

    [Header("Crouch parameters")]
    public float crouchHeight = 0.75f;
    private float currentHeight;
    private float standingHeight;

    [Header("Stamina parameters")]
    [SerializeField] private int lifes = 3;
    private bool isTired = false;

    [Header("Data parameters")]
    public Animator animator;
    public AnimatorOverrideController initialController;
    public PlayerStaminaComponent health;
    [SerializeField] private CharacterController _hitbox;
    [SerializeField] private EnemySensor _onKnockdownSensor;
    public PlayerSFXController PlayerSFX;

    [Header("Debug")]
    public TMP_Text fpsText;
    public TMP_Text staminaText;
    private float deltaTime;
    [Range(0, 1f)] public float timeSpeed;
    public int targetFPS = 60;
    public bool invulnerable;

    float xRotation = 0f;
    float yRotation = 0f;
    float minCameraAngle = -90f;
    float maxCameraAngle = 45f;
    private float smoothXRotation = 1f;
    private float smoothYRotation = 1f;
    private float smoothDampVelocity = 0.5f;
    private float smoothDampVelocity1 = 0.5f;
    bool isRunPressed => Input.GetKey(KeyCode.LeftShift);
    bool isControllingHand = false;
    bool isGrounded = true;
    public Vector2 recoilAngle;
    public float XRotaion { get { return xRotation; } set { xRotation = value; } }
    public float SmoothXRotation { get { return smoothXRotation; } set { smoothXRotation = value; } }
    public bool isJumped { get {  return isJumped; } }
    public bool IsControllingHand {  get { return isControllingHand; } set { isControllingHand = value; } }
    Vector3 velocityDirection;
    public Vector2 moveDirection;
    [SerializeField] Camera playerCamera;
    Vector2 inputDirection;
    public CharacterController Hitbox => _hitbox;
    public PlayerStateMachine StateMachine { get; private set; }
    public Camera PlayerCamera { get { return playerCamera; } }
    public Vector2 InputDirection { get { return inputDirection; } set { inputDirection = value; } }
    public Vector3 VelocityDirection { get { return velocityDirection; } set { velocityDirection = value; } }
    public float GravityVelocity { get { return velocityDirection.y; } set { velocityDirection.y = value; } }
    public float TargetSpeed { get; private set; }
    public float CurrentSpeed { get { return currentSpeed; } set { currentSpeed = value; } }
    public float StandingHeight { get { return standingHeight; } }
    public bool IsRunPressed { get { return isRunPressed; } }
    public bool IsMoving => _hitbox.velocity.magnitude > 0.1f;
    public bool IsRunning => TargetSpeed == defaultSpeed.Value * sprintSpeedMultiplier.Value && IsMoving;
    public bool IsGrounded { get { return isGrounded; } }
    public bool IsTired { get { return isTired; } }
    // Hashing string variables
    int isJumping, X_Velocity, Y_Velocity;

    // Update is called once per frame
    void Awake()
    {
        isJumping = Animator.StringToHash("isJumping");
        X_Velocity = Animator.StringToHash("X_Velocity");
        Y_Velocity = Animator.StringToHash("Y_Velocity");

        standingHeight = Hitbox.height;
        health = GetComponent<PlayerStaminaComponent>();
        playerCamera = cameraPosition.GetComponent<Camera>();
        Cursor.visible = false;
        velocityDirection.y -= 1f;

        StateMachine = gameObject.AddComponent<PlayerStateMachine>();
        StateMachine.AddState(new PlayerGroundedState(EPlayerState.Grounded, StateMachine, this));
        StateMachine.AddState(new PlayerFallingState(EPlayerState.Falling, StateMachine, this, _hitbox));
        StateMachine.AddState(new PlayerKnockdownState(EPlayerState.Knockdown, StateMachine, this, _onKnockdownSensor.gameObject));
        StateMachine.AddState(new PlayerTrappedState(EPlayerState.Trapped, StateMachine, this, _onKnockdownSensor.gameObject));
        StateMachine.AddState(new PlayerDeadState(EPlayerState.Dead, StateMachine, this));
        StateMachine.InitState(EPlayerState.Grounded);
    }

    private void Start()
    {
        health.OnDamageEvent += Health_OnDamageEvent;
        health.OnDeathEvent += Health_OnDeathEvent;
        _onKnockdownSensor.OnEnemyEnter += OnKnockdownSensor_OnEnemyEnter;
        _onKnockdownSensor.OnEnemyExit += OnKnockdownSensor_OnEnemyExit;
    }

    private void Update()
    {
        Application.targetFrameRate = targetFPS;
        Time.timeScale = timeSpeed;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        //ProcessMovement();
    }

    /*private void LateUpdate()
    {
        CameraControl();
    }*/

    public void ProcessMovement(float targetSpeed)
    {
        TargetSpeed = targetSpeed;
        var castOrigin = transform.position + new Vector3(0, Hitbox.radius, 0);
        isGrounded = Physics.SphereCast(castOrigin, Hitbox.radius - .01f, Vector3.down,
                out var hit, .1f, ~LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore);

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        inputDirection = new Vector2(horizontal, vertical);
        inputDirection = inputDirection.normalized;
        moveDirection = Vector2.Lerp(moveDirection, inputDirection, Time.deltaTime * acceleration.Value);

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration.Value * Time.deltaTime);

        var finalVelocity = currentSpeed * moveDirection;
        animator.SetFloat(X_Velocity, finalVelocity.x);
        animator.SetFloat(Y_Velocity, finalVelocity.y);
    }

    public void ProcessFalling()
    {
        var castOrigin = transform.position + new Vector3(0, Hitbox.radius, 0);
        isGrounded = Physics.SphereCast(castOrigin, Hitbox.radius - .01f, Vector3.down,
                out var hit, .1f, ~LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore);

        if (_hitbox.velocity.y < -5f) animator.SetBool("isFalling", true);
        GravityVelocity += gravity * Time.deltaTime;
        _hitbox.Move(velocityDirection * Time.deltaTime);
    }

    public void UpdateCameraPosition()
    {
        //cameraPivot.localRotation = Quaternion.Euler(smoothXRotation + recoilAngle.x, smoothYRotation, recoilAngle.y);
        //cameraPivot.localRotation = Quaternion.Euler(xRotation + recoilAngle.x, 0f, recoilAngle.y);
        //cameraPivot.position = headPivot.position;

        cameraRoot.position = Vector3.MoveTowards(cameraPosition.position, cameraPosition.position + cameraPosition.forward, 2f);
        cameraPosition.position = headPivot.position;
        cameraPosition.localRotation = Quaternion.Euler(smoothXRotation + recoilAngle.x, 0f, recoilAngle.y);
    }

    void CameraControlVariant2()
    {
        //xRotation -= isControllingHand ? Input.GetAxis("Mouse ScrollWheel") * 150f : Input.GetAxisRaw("Mouse Y");
        xRotation -= Input.GetAxisRaw("Mouse Y") * CursorController.aimSensitivity * 0.1f;
        yRotation += Input.GetAxisRaw("Mouse X") * CursorController.aimSensitivity * 0.1f;
        //transform.Rotate(Vector3.up, yRotation, Space.Self);

        if (CursorController.RMBhold)
            xRotation = Mathf.Clamp(xRotation, -45f, 45f);
        else
            xRotation = Mathf.Clamp(xRotation, -90f, animator.GetBool("isMoving") ? 105f : 90f);

        //yRotation = Mathf.Clamp(yRotation, -90, 90f);

        smoothXRotation = Mathf.SmoothDamp(smoothXRotation, xRotation, ref smoothDampVelocity, 0.1f);
        smoothYRotation = Mathf.SmoothDamp(smoothYRotation, yRotation, ref smoothDampVelocity1, 0.1f);
        
        transform.localRotation = Quaternion.Euler(0, yRotation, 0);
        
        recoilAngle.x = Mathf.Lerp(recoilAngle.x, 0f, Time.deltaTime * 15f);
        recoilAngle.y = Mathf.Lerp(recoilAngle.y, moveDirection.x * -5f, Time.deltaTime * 15f);
    }

    public void CameraControl()
    {
        //xRotation -= Input.GetAxis("Mouse ScrollWheel") * 150f;

        if (CursorController.RMBhold)
            xRotation = Mathf.Clamp(xRotation, -50f, 50f);
        else
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        smoothXRotation = Mathf.SmoothDamp(smoothXRotation, xRotation, ref smoothDampVelocity, 0.1f);

        recoilAngle.x = Mathf.Lerp(recoilAngle.x, 0f, Time.deltaTime * 15f);
        recoilAngle.y = Mathf.Lerp(recoilAngle.y, moveDirection.x * -5f, Time.deltaTime * 15f);
    }

    public void OnKnockdownCameraControl()
    {
        xRotation = Mathf.Clamp(xRotation, minCameraAngle, maxCameraAngle);
        smoothXRotation = Mathf.SmoothDamp(smoothXRotation, xRotation, ref smoothDampVelocity, 0.1f);
        recoilAngle.x = Mathf.Lerp(recoilAngle.x, 0f, Time.deltaTime * 15f);
        recoilAngle.y = Mathf.Lerp(recoilAngle.y, moveDirection.x * -5f, Time.deltaTime * 15f);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        // no rigidbody
        if (body == null || body.isKinematic)
            return;

        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3f)
            return;

        // Calculate push direction from move direction,
        // we only push objects to the sides never up and down
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        // If you know how fast your character is trying to move,
        // then you can also multiply the push velocity by that.

        // Apply the push
        body.linearVelocity = pushDir * 0.8f;
    }

    public void SetAnimatorBool(string boolName)
    {
        animator.SetBool(boolName, !true);
    }

    private void Health_OnDamageEvent()
    {
        var random = new Vector2(
            Random.Range(-45f, 45f),
            Random.Range(-45f, 45f));
        recoilAngle = random;
        weaponManager.transform.localRotation = Quaternion.Euler(random);
    }

    private void Health_OnDeathEvent()
    {
        lifes--;
        var random = new Vector2(
            Random.Range(-45f, 45f),
            Random.Range(-45f, 45f));
        recoilAngle = random;
        PlayerSFX.ChangeStaminaSound(PlayerSFX.knockdownBreathSound);
        weaponManager.transform.localRotation = Quaternion.Euler(random);
        StateMachine.SetState(EPlayerState.Knockdown);
    }

    public float GetRaycastDistanceFromCamera(float distance)
    {
        bool isCollided = Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out var hit, distance, ~LayerMask.GetMask("Pickable", "Player", "Insertable", "Trigger", "UI"));
        if (!isCollided) return distance;
        return hit.distance;
    }

    private void OnKnockdownSensor_OnEnemyEnter(EnemyController enemy)
    {
        if (!_onKnockdownSensor.gameObject.activeSelf) return;
        _onKnockdownSensor.gameObject.SetActive(false);
        enemy.StateMachine.SetState(EEnemyState.PullVictimToRape);
    }

    private void OnKnockdownSensor_OnEnemyExit(EnemyController enemy)
    {
        
    }

    public void SetCameraAngleLimit(float minAngle, float maxAngle)
    {
        minCameraAngle = minAngle;
        maxCameraAngle = maxAngle;
    }
}
