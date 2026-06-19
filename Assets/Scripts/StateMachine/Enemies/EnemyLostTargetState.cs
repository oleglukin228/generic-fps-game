using System;
using UnityEngine;

public class EnemyLostTargetState : EnemyState
{
    private PlayerController _player;
    private Vector3 _lastPosition;

    public EnemyLostTargetState(EEnemyState state, IStateMachine<EEnemyState> stateMachine, EnemyController controller, PlayerController player) : base(state, stateMachine, controller)
    {
        _player = player;
    }

    public override void OnEnter()
    {
        _lastPosition = _player.transform.position;
        _agent.SetDestination(_lastPosition);
    }

    public override void OnUpdate()
    {
        _agent.SetDestination(_lastPosition);
        _controller.SynchronizeAnimatorAndAgent(_agent.steeringTarget, _controller.Speed.Value);
        if (_agent.remainingDistance <= _agent.stoppingDistance) _stateMachine.SetState(EEnemyState.Wandering);
    }
}
