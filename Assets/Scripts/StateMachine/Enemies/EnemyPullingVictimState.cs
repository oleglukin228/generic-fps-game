using UnityEngine;
using UnityEngine.UIElements;

public class EnemyPullingVictimState : EnemyState
{
    private PlayerController _target;
    private Vector3 _destination;
    private Transform _leftLegIKTarget;
    private Transform _rightLegIKTarget;
    public EnemyPullingVictimState(EEnemyState state, IStateMachine<EEnemyState> stateMachine, EnemyController controller, PlayerController target, Transform leftLegIKTarget,
        Transform rightLegIKTarget) : base(state, stateMachine, controller)
    {
        _target = target;
        _leftLegIKTarget = leftLegIKTarget;
        _rightLegIKTarget = rightLegIKTarget;
    }

    public override void OnEnter()
    {
        _controller.SetStoppingDistance(0f);
        _controller.EnableSensors(false);
        _controller.Animator.SetBool("isPulling", true);
        _target.animator.applyRootMotion = false;
        _target.weaponManager.playerIK.SetupLeftLeg(_leftLegIKTarget);
        _target.weaponManager.playerIK.SetupRightLeg(_rightLegIKTarget);
        _target.weaponManager.playerIK.ChangeLeftFootIKValue(1f, 0.5f);
        _target.weaponManager.playerIK.ChangeRightFootIKValue(1f, 0.5f);
        _destination = GameEnvironment.GetClosestBush(_agent.steeringTarget);
    }

    public override void OnUpdate()
    {
        _agent.SetDestination(_destination);
        _controller.SynchronizeAnimatorAndAgent(_agent.steeringTarget, _controller.Speed.Value, _target.transform);
        
        _target.health.StopRegenerating();
        _target.transform.SetPositionAndRotation(Vector3.Lerp(_target.transform.position, _controller.transform.position, 2.5f * Time.deltaTime), 
            Quaternion.Lerp(_target.transform.rotation, Quaternion.LookRotation(-_controller.transform.forward), 2.5f * Time.deltaTime));

        if (Vector3.Distance(_destination, _controller.transform.position) < 0.1f) _stateMachine.SetState(EEnemyState.Eating);
    }

    public override void OnExit()
    {
        _controller.SetStoppingDistance(1f);
        _controller.EnableSensors(true);
        _controller.Animator.SetBool("isPulling", false);
        _target.animator.applyRootMotion = true;
        _target.weaponManager.playerIK.ChangeLeftFootIKValue(0f, 0.5f);
        _target.weaponManager.playerIK.ChangeRightFootIKValue(0f, 0.5f);
    }
}
