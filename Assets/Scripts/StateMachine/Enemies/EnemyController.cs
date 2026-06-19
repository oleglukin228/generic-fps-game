using Kryz.CharacterStats;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private PlayerController _player;
    [SerializeField] private Transform _sensors;
    [SerializeField] private PlayerSensor _chaseSensor;
    [SerializeField] private PlayerSensor _attackSensor;
    [SerializeField] private PlayerSensor _hurtBoxSensor;
    [SerializeField] private LookAt _lookAt;
    public CharacterStat Damage;
    public CharacterStat WalkSpeed;
    public CharacterStat Speed;
    [SerializeField] Transform _playerLeftLegIKTarget;
    [SerializeField] Transform _playerRightLegIKTarget;
    [SerializeField] SurfaceImpact _onAttackSurfaceImpact;
    [SerializeField] EEnemyState initialState;

    private Ragdoll _ragdoll;
    private Animator _animator;
    private Vector2 _smoothDeltaPosition;
    private Vector2 _velocity;

    public HealthComponent Health { get; private set; }
    public NavMeshAgent Agent { get; private set; }
    public StateMachine<EEnemyState> StateMachine { get; private set; }
    public Animator Animator => _animator;

    private void Awake()
    {
        _ragdoll = GetComponent<Ragdoll>();
        _animator = GetComponent<Animator>();
        Agent = GetComponent<NavMeshAgent>();
        Health = GetComponent<HealthComponent>();
        _player = FindAnyObjectByType<PlayerController>();

        _animator.applyRootMotion = true;
        _animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        _animator.keepAnimatorStateOnDisable = true;
        Agent.updatePosition = false;
        Agent.updateRotation = true;
        Agent.speed = Speed.Value;

        StateMachine = gameObject.AddComponent<EnemyStateMachine>();
        StateMachine.AddState(new EnemyIdleState(EEnemyState.Idle, StateMachine, this));
        StateMachine.AddState(new EnemyIdleCrouchState(EEnemyState.IdleCrouch, StateMachine, this, _chaseSensor));
        StateMachine.AddState(new EnemyWanderingState(EEnemyState.Wandering, StateMachine, this));
        StateMachine.AddState(new EnemyChaseState(EEnemyState.Chasing, StateMachine, this, _player));
        StateMachine.AddState(new EnemyLostTargetState(EEnemyState.LostTarget, StateMachine, this, _player));
        StateMachine.AddState(new EnemyAttackState(EEnemyState.Attack, StateMachine, this, _player, _chaseSensor));
        StateMachine.AddState(new EnemyPullingVictimState(EEnemyState.PullVictimToRape, StateMachine, this, _player, _playerLeftLegIKTarget, _playerRightLegIKTarget));
        StateMachine.AddState(new EnemyEatState(EEnemyState.Eating, StateMachine, this, _player, _ragdoll));
        StateMachine.AddState(new EnemyDeathState(EEnemyState.Death, StateMachine, this));
        StateMachine.InitState(initialState);
    }

    private void Start()
    {
        _hurtBoxSensor.OnPlayerEnter += HurtBoxSensor_OnPlayerEnter;
        _chaseSensor.OnPlayerEnter += ChaseSensor_OnPlayerEnter;
        _chaseSensor.OnPlayerExit += ChaseSensor_OnPlayerExit;
        _attackSensor.OnPlayerEnter += AttackSensor_OnPlayerEnter;
        _attackSensor.OnPlayerExit += AttackSensor_OnPlayerExit;
        Health.OnDamageEvent += Health_OnDamageEvent;
        Health.OnDeathEvent += Health_OnDeathEvent;

        Invoke(nameof(DisableStateMachine), 1f);
        EnableSensors(false);
    }

    private void HurtBoxSensor_OnPlayerEnter(PlayerController player)
    {
        if (player.TryGetComponent<IDamageable>(out var damageable))
        {
            SurfaceManager.Instance.SpawnEffect(player.transform.position, Quaternion.identity, Vector3.zero, player.Hitbox, _onAttackSurfaceImpact, null);
            damageable.TakeDamage(Damage.Value, Vector3.zero, Vector3.zero);
        }
    }

    private void ChaseSensor_OnPlayerEnter(PlayerController player)
    {
        CancelInvoke(nameof(DisableStateMachine));
        _player = player;
        _animator.enabled = true;
        _animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
        StateMachine.enabled = true;
        StateMachine.SetState(EEnemyState.Chasing);
    }

    private void ChaseSensor_OnPlayerExit(Vector3 position)
    {
        StateMachine.SetState(EEnemyState.LostTarget);
        Invoke(nameof(DisableStateMachine), 10f);
    }

    private void AttackSensor_OnPlayerEnter(PlayerController player)
    {
        StateMachine.SetState(EEnemyState.Attack);
    }

    private void AttackSensor_OnPlayerExit(Vector3 position)
    {
        StateMachine.SetState(EEnemyState.Chasing);
    }

    public void Health_OnDamageEvent() 
    {
        StateMachine.SetState(EEnemyState.Chasing);
    }

    public void Health_OnDeathEvent()
    {
        StateMachine.SetState(EEnemyState.Death);
        _ragdoll.ActivateRagdoll();
        _chaseSensor.gameObject.SetActive(false);
        _attackSensor.gameObject.SetActive(false);
        Agent.enabled = false;
    }

    public void SynchronizeAnimatorAndAgent(Vector3 position, float speed)
    {
        Vector3 worldDeltaPosition = position - transform.position;
        worldDeltaPosition.y = 0;
        // Map 'worldDeltaPosition' to local space
        float dx = Vector3.Dot(transform.right, worldDeltaPosition);
        float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(dx, dy);

        // Low-pass filter the deltaMove
        float smooth = Mathf.Min(1, Time.deltaTime / 0.1f);
        _smoothDeltaPosition = Vector2.Lerp(_smoothDeltaPosition, deltaPosition, smooth);

        _velocity = _smoothDeltaPosition / Time.deltaTime;
        _velocity = Vector2.ClampMagnitude(_velocity, Speed.Value);
        if (Agent.remainingDistance <= Agent.stoppingDistance)
        {
            _velocity = Vector2.Lerp(Vector2.zero, _velocity, Agent.remainingDistance);
        }

        bool shouldMove = _velocity.magnitude > 0.5f && Agent.remainingDistance > Agent.stoppingDistance;

        Agent.speed = speed;
        Animator.SetBool("isMoving", shouldMove);
        Animator.SetFloat("X_Velocity", _velocity.x);
        Animator.SetFloat("Y_Velocity", _velocity.y);

        _lookAt.lookAtTargetPosition = Agent.steeringTarget + Agent.transform.forward;

        float deltaMagnitude = worldDeltaPosition.magnitude;
        if (deltaMagnitude > Agent.radius / 2)
        {
            Agent.transform.position = Vector3.Lerp(Animator.rootPosition, Agent.nextPosition, smooth);
        }
    }

    public void SynchronizeAnimatorAndAgent(Vector3 position, float speed, Transform target)
    {
        SynchronizeAnimatorAndAgent(position, speed);
        transform.LookAt(target);
    }

    private void OnAnimatorMove()
    {
        Vector3 rootPosition = _animator.rootPosition;
        rootPosition.y = Agent.nextPosition.y;
        transform.position = rootPosition;
        Agent.nextPosition = rootPosition;
    }
    public void EnableSensors(bool b)
    {
        _lookAt.enabled = b;
        _sensors.gameObject.SetActive(b);
    }

    public void DisableStateMachine()
    {
        _animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
        _animator.enabled = false;
        StateMachine.enabled = false;
    }

    public void EnableStateMachine()
    {
        _animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
        _animator.enabled = true;
        StateMachine.enabled = true;
    }

    public void StartAttack() => _hurtBoxSensor.gameObject.SetActive(true);
    public void EndAttack() => _hurtBoxSensor.gameObject.SetActive(false);
    public void SetStoppingDistance(float value) => Agent.stoppingDistance = value;
}

