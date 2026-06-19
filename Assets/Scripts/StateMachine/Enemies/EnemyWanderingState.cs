using Kryz.CharacterStats;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EnemyWanderingState : EnemyState
{
    float timer;

    public EnemyWanderingState(EEnemyState state, IStateMachine<EEnemyState> stateMachine, EnemyController controller) : base(state, stateMachine, controller)
    {
    }

    public override void OnEnter()
    {
        _controller.Speed.AddModifier(new StatModifier(-0.5f, StatModType.PercentMult, this));
    }

    public override void OnUpdate()
    {
        timer += Time.deltaTime;

        if (timer >= 5F)
        {
            Vector3 newPos = RandomNavSphere(_controller.transform.position, 5, -1);
            _agent.SetDestination(newPos);
            timer = 0;
        }

        _controller.SynchronizeAnimatorAndAgent(_agent.steeringTarget, _controller.WalkSpeed.Value);
    }

    public override void OnExit()
    {
        _controller.Speed.RemoveAllModifiersFromSource(this);
    }

    private Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;

        randomDirection += origin;

        NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, distance, layermask);

        return navHit.position;
    }
}
