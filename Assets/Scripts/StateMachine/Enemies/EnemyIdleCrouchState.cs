using UnityEngine;

public class EnemyIdleCrouchState : EnemyState
{
    PlayerSensor _playerSensor;
    public EnemyIdleCrouchState(EEnemyState state, IStateMachine<EEnemyState> stateMachine, EnemyController controller, PlayerSensor playerSensor) : base(state, stateMachine, controller)
    {
        _playerSensor = playerSensor;
    }

    public override void OnEnter()
    {
        _controller.Animator.SetBool("isCrouching", true);
    }

    public override void OnExit()
    {
        _playerSensor.GetComponent<SphereCollider>().radius = 10f;
        _controller.Animator.SetBool("isCrouching", false);
    }
}
