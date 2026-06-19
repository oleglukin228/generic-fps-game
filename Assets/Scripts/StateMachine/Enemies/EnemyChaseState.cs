using System;
using UnityEngine;

public class EnemyChaseState : EnemyState
{
    private PlayerController _target;

    public EnemyChaseState(EEnemyState state, IStateMachine<EEnemyState> stateMachine, EnemyController controller, PlayerController target) : base(state, stateMachine, controller)
    {
        _target = target;
    }

    public override void OnEnter()
    {
        _agent.SetDestination(_target.transform.position);
    }

    public override void OnUpdate()
    {
        _agent.SetDestination(_target.transform.position);
        _controller.SynchronizeAnimatorAndAgent(_agent.steeringTarget, _controller.Speed.Value);
    }
}

/*public class AverageFagChaseState : AiState
{
    float timer = 0.0f;
    float dodgeTimer = 0.0f;

    public AiStateId GetId()
    {
        return AiStateId.ChasePlayer;
    }

    public void Enter(AiAgent agent)
    {

    }
    public void Update(AiAgent agent)
    {
        if (!agent.enabled)
        {
            return;
        }
        timer -= Time.deltaTime;
        dodgeTimer -= Time.deltaTime;
        if (!agent.navMeshAgent.hasPath)
        {
            agent.navMeshAgent.destination = agent.target.position;
        }
        if (timer < 0.0f)
        {
            Vector3 direction = (agent.target.position - agent.navMeshAgent.destination);
            direction.y = 0;
            if (direction.sqrMagnitude > agent.config.maxDistance * agent.config.maxDistance)
            {
                if (agent.navMeshAgent.pathStatus != NavMeshPathStatus.PathPartial)
                {
                    agent.navMeshAgent.destination = agent.target.position;
                }
            }

            timer = agent.config.maxTime;
        }

        if (dodgeTimer < 0.0f)
        {
            float dot = Vector3.Dot(agent.transform.forward, agent.target.transform.forward);
            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
            var halfHeightDifference = Vector3.up * (agent.navMeshAgent.height / 2);
            //Debug.DrawLine(agent.transform.localPosition + halfHeightDifference, agent.target.localPosition, Color.red, 1f);
            if (angle > 179f && angle < 181f)
            {
                if (Physics.Raycast(agent.transform.localPosition + halfHeightDifference, agent.transform.forward, out var hit, agent.config.dodgeDistance, ~LayerMask.GetMask("Enemy", "Hitbox")))
                {
                    PlayerMovement target = hit.collider.gameObject.GetComponent<PlayerMovement>();
                    if (target != null && target.weaponManager.HeldWeapon != null)
                    {
                        dodgeTimer = agent.config.dodgeCooldown;
                        agent.stateMachine.ChangeState(AiStateId.Dodge);
                    }
                }
            }
        }

        float distanceToPlayer = Vector3.Distance(agent.transform.position, agent.target.position);
        if (distanceToPlayer < agent.config.attackRange)
        {
            agent.stateMachine.ChangeState(AiStateId.Attack);
        }
    }
    public void Exit(AiAgent agent)
    {

    }
}
*/