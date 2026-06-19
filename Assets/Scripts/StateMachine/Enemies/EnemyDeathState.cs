using System;
using UnityEngine;

public class EnemyDeathState : EnemyState
{
    public EnemyDeathState(EEnemyState state, IStateMachine<EEnemyState> stateMachine, EnemyController controller) : base(state, stateMachine, controller)
    {
    }

    public override void OnEnter()
    {
        _controller.EnableSensors(false);
    }
}
