using AudioSystem;
using Kryz.CharacterStats;
using System.Collections;
using UnityEngine;

public class BearTrap : InteractableEnvironment
{
    [SerializeField] private AnimatorOverrideController onTrappedAnimator;
    [SerializeField] private GameObject _sensors;
    [SerializeField] private HealthComponent _health;
    [SerializeField] private PlayerSensor _playerSensor;
    [SerializeField] private EnemySensor _enemySensor;
    [SerializeField] private RigidbodySensor _rigidbodySensor;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private SoundData _beartrapSound;
    private CharacterStat _characterStat;
    private PlayerController _playerController;

    protected override void Start()
    {
        base.Start();
        _playerSensor.OnPlayerEnter += PlayerSensor_OnPlayerEnter;
        _playerSensor.OnPlayerExit += PLayerSensor_OnPlayerExit;
        _enemySensor.OnEnemyEnter += EnemySensor_OnEnemyEnter;
        _enemySensor.OnEnemyExit += EnemySensor_OnEnemyExit;
        _rigidbodySensor.OnHitboxEnter += RigidbodySensor_OnHitboxEnter;
        _rigidbodySensor.OnHitboxExit += RigidbodySensor_OnHitboxExit;
        _health.OnDamageEvent += Health_OnDamageEvent;
        _health.OnDeathEvent += Health_OnDeathEvent;
    }

    private void EnemySensor_OnEnemyExit(EnemyController enemy)
    {
        
    }

    private void EnemySensor_OnEnemyEnter(EnemyController enemy)
    {
        _characterStat = enemy.Speed;
        _characterStat.AddModifier(new StatModifier(-1f, StatModType.PercentMult, this));

        var randomFoot = enemy.Animator.GetBoneTransform(Random.Range(0, 2) == 0 ? HumanBodyBones.LeftFoot : HumanBodyBones.RightFoot);
        _rigidbody.useGravity = false;
        transform.SetParent(randomFoot, true);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(-45, 0, -180));
        interactableActions.PlayAnimations(0.1f);
        _sensors.SetActive(false);

        SoundManager.Instance.CreateSoundBuilder().WithPosition(transform.position).Play(_beartrapSound);
    }

    private void RigidbodySensor_OnHitboxExit(Rigidbody rb)
    {
        
    }

    private void RigidbodySensor_OnHitboxEnter(Rigidbody rb)
    {
        Health_OnDeathEvent();
        SoundManager.Instance.CreateSoundBuilder().WithPosition(transform.position).Play(_beartrapSound);
    }

    private void Health_OnDeathEvent()
    {
        _sensors.SetActive(false);
        this.transform.SetParent(null);
        _rigidbody.useGravity = true;
        _rigidbody.isKinematic = false;
        interactableActions.PlayAnimations(0.05f);
        _characterStat?.RemoveAllModifiersFromSource(this);
    }

    private void OnTrapDeathTriggerForPlayer()
    {
        StartCoroutine(nameof(WaitUntilIsNotOnKnokdown));
    }

    private IEnumerator WaitUntilIsNotOnKnokdown()
    {
        yield return new WaitUntil(() => !_playerController.StateMachine.IsInCurrentState(EPlayerState.Knockdown));
        _playerController.animator.runtimeAnimatorController = _playerController.initialController;
        if (!_playerController.StateMachine.IsInCurrentState(EPlayerState.Dead)) _playerController.StateMachine.SetState(EPlayerState.Grounded);
        _playerController = null;
        _characterStat = null;

        _health.OnDeathEvent -= OnTrapDeathTriggerForPlayer;
    }

    private void Health_OnDamageEvent()
    {
        
    }

    private void PlayerSensor_OnPlayerEnter(PlayerController player)
    {
        _health.OnDeathEvent += OnTrapDeathTriggerForPlayer;

        _playerController = player;
        _playerController.SetCameraAngleLimit(-90f, 45f);
        _playerController.animator.runtimeAnimatorController = onTrappedAnimator;
        _playerController.StateMachine.SetState(EPlayerState.Trapped);
        _characterStat = player.defaultSpeed;
        _characterStat.AddModifier(new StatModifier(-1f, StatModType.PercentMult, this));
        
        var randomFoot = player.animator.GetBoneTransform(Random.Range(0, 2) == 0 ? HumanBodyBones.LeftFoot : HumanBodyBones.RightFoot);
        transform.SetParent(randomFoot, true);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(-45, 0, -180));
        interactableActions.PlayAnimations(0.1f);
        _sensors.SetActive(false);

        SoundManager.Instance.CreateSoundBuilder().WithPosition(transform.position).Play(_beartrapSound);
    }

    private void PLayerSensor_OnPlayerExit(Vector3 lastKnownPosition)
    {
        
    }
}
