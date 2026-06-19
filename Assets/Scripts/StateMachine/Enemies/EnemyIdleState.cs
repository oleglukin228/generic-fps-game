using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(EEnemyState state, IStateMachine<EEnemyState> stateMachine, EnemyController controller) : base(state, stateMachine, controller)
    {
        
    }

    public override void OnEnter()
    {
        _controller.Animator.SetBool("isMoving", false);
    }

    /*public override void OnExit()
    {
        _controller.Animator.enabled = true;
    }*/
}

/*public class AverageFagIdleState : AiIdleState
{
    public override void Update(AiAgent agent)
    {
        Vector3 playerDirection = agent.target.position - agent.transform.position;
        if (playerDirection.magnitude > agent.config.maxSightDistance)
        {
            return;
        }

        Vector3 agentDirection = agent.transform.forward;

        playerDirection.Normalize();

        float dotProduct = Vector3.Dot(playerDirection, agentDirection);
        if (dotProduct > 0.0f)
        {
            agent.stateMachine.ChangeState(AiStateId.ChasePlayer);
        }
    }
}*/
