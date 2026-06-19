using AudioSystem;
using Kryz.CharacterStats;
using Kryz.CharacterStats.Examples;
using System;
using System.Collections;
using TeoGames.Mesh_Combiner.Scripts.Extension;
using UnityEngine;

public class SnareTrap : MonoBehaviour
{
    [SerializeField] private AnimatorOverrideController onTrappedAnimator;
    [SerializeField] private GameObject _1halfRope;
    [SerializeField] private GameObject _2halfRope;
    [SerializeField] private GameObject _sensors;
    [SerializeField] private HealthComponent _health;
    [SerializeField] private PlayerSensor _playerSensor;
    [SerializeField] private EnemySensor _enemySensor;
    [SerializeField] SoundData _onActivateSound;
    private CharacterStat _characterStat;
    private PlayerController _playerController;

    private void Start()
    {
        _playerSensor.OnPlayerEnter += PlayerSensor_OnPlayerEnter;
        _playerSensor.OnPlayerExit += PLayerSensor_OnPlayerExit;
        //_enemySensor.OnEnemyEnter += EnemySensor_OnEnemyEnter;
        //_enemySensor.OnEnemyExit += EnemySensor_OnEnemyExit;
        _health.OnDamageEvent += Health_OnDamageEvent;
        _health.OnDeathEvent += Health_OnDeathEvent;
    }

    private void Health_OnDamageEvent()
    {
        
    }

    private void Health_OnDeathEvent()
    {
        _1halfRope.SetActive(false);
        _sensors.SetActive(false);
    }

    private void EnemySensor_OnEnemyEnter(EnemyController enemy)
    {
        throw new NotImplementedException();
    }

    private void EnemySensor_OnEnemyExit(Vector3 lastKnownPosition)
    {
        throw new NotImplementedException();
    }

    private void PlayerSensor_OnPlayerEnter(PlayerController player)
    {
        _health.OnDeathEvent += OnTrapDeathTriggerForPlayer;
        _2halfRope.SetActive(false);
        
        _playerController = player;
        _playerController.health.OnDeathEvent += OnTrapDeathTriggerForPlayerOnKO;
        _playerController.Hitbox.enabled = false;
        _playerController.SetCameraAngleLimit(0f, 100f);
        _playerController.animator.runtimeAnimatorController = onTrappedAnimator;
        _playerController.StateMachine.SetState(EPlayerState.Trapped);
        
        _characterStat = player.defaultSpeed;
        _characterStat.AddModifier(new StatModifier(-1f, StatModType.PercentMult, this));

        StartCoroutine(LerpEnumerator.OnUpdate(
            (t) => _playerController.transform.SetLocalPositionAndRotation(
                Vector3.Lerp(_playerController.transform.localPosition, _1halfRope.transform.position, t),
                Quaternion.Lerp(_playerController.transform.localRotation, _1halfRope.transform.rotation, t)), () => _playerController.Hitbox.enabled = true,
        0.5f));
        SoundManager.Instance.CreateSoundBuilder().WithPosition(transform.position).Play(_onActivateSound);
    }

    private void OnTrapDeathTriggerForPlayer()
    {
        _playerController.health.OnDeathEvent -= OnTrapDeathTriggerForPlayerOnKO;
        _playerController.transform.SetParent(null);
        _playerController.animator.runtimeAnimatorController = _playerController.initialController;
        _playerController.animator.SetBool("isKnockedDown", true);
        _playerController.StateMachine.SetState(EPlayerState.Falling);

        StartCoroutine(LerpEnumerator.OnUpdate(
            (t) => _playerController.transform.localRotation = Quaternion.Lerp(_playerController.transform.localRotation, Quaternion.identity, t), 
            () => 
            { 
                _playerController.animator.SetBool("isKnockedDown", false); 
                _playerController = null; 
            },
        0.5f));

        _characterStat?.RemoveAllModifiersFromSource(this);
        _characterStat = null;

        _health.OnDeathEvent -= OnTrapDeathTriggerForPlayer;
    }

    private void OnTrapDeathTriggerForPlayerOnKO()
    {
        StartCoroutine(nameof(WaitUntilIsNotOnKnokdown));
    }

    private IEnumerator WaitUntilIsNotOnKnokdown()
    {

        _playerController.health.OnDeathEvent -= OnTrapDeathTriggerForPlayerOnKO;
        _playerController.transform.SetParent(null);
        _playerController.animator.runtimeAnimatorController = _playerController.initialController;

        StartCoroutine(LerpEnumerator.OnUpdate(
            (t) => _playerController.transform.localRotation = Quaternion.Lerp(_playerController.transform.localRotation, Quaternion.identity, t),
        0.5f));

        yield return new WaitUntil(() => !_playerController.StateMachine.IsInCurrentState(EPlayerState.Knockdown));
        if (!_playerController.StateMachine.IsInCurrentState(EPlayerState.Dead)) _playerController.StateMachine.SetState(EPlayerState.Grounded);
        _playerController.SetCameraAngleLimit(-90f, 45f);
        _playerController = null;
        _characterStat?.RemoveAllModifiersFromSource(this);
        _characterStat = null;

        _health.OnDeathEvent -= OnTrapDeathTriggerForPlayer;
    }

    private void PLayerSensor_OnPlayerExit(Vector3 lastKnownPosition)
    {
        
    }
}
