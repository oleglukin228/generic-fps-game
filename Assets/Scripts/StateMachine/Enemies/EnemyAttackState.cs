using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttackState : EnemyState
{
    private PlayerController _target;
    private PlayerSensor _chaseSensor;

    public EnemyAttackState(EEnemyState state, IStateMachine<EEnemyState> stateMachine, EnemyController controller, PlayerController target, PlayerSensor chaseSensor) : base(state, stateMachine, controller)
    {
        this._target = target;
        _chaseSensor = chaseSensor;
    }

    public override void OnEnter()
    {
        _controller.Animator.SetBool("Punch", true);
        _chaseSensor.gameObject.SetActive(false);
    }

    public override void OnUpdate()
    {
        _agent.SetDestination(_target.transform.position);
        _controller.SynchronizeAnimatorAndAgent(_target.transform.position, _controller.Speed.Value);
    }

    public override void OnExit()
    {
        _controller.Animator.SetBool("Punch", false);
        _chaseSensor.gameObject.SetActive(true);
    }
}
