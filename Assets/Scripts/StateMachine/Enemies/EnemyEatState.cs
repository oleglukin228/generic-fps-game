using UnityEngine;

public class EnemyEatState : EnemyState
{
    PlayerController _player;
    Ragdoll _ragdoll;
    public EnemyEatState(EEnemyState state, IStateMachine<EEnemyState> stateMachine, EnemyController controller, PlayerController player, Ragdoll ragdoll) : base(state, stateMachine, controller)
    {
        _player = player;
        _ragdoll = ragdoll;
    }

    public override void OnEnter()
    {
        _ragdoll.EnableBoxCollider(false);
        _controller.EnableSensors(false);
        _controller.Animator.SetBool("isRaping", true);
        _player.animator.applyRootMotion = false;
        _player.StateMachine.SetState(EPlayerState.Dead);
        _player.transform.SetParent(_controller.transform);
        _player.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0f, 180f, 0f));
    }

    public override void OnUpdate()
    {
        //_player.transform.position = Vector3.Lerp(_player.transform.position, _controller.transform.position, 10f * Time.deltaTime);
    }

    public override void OnExit()
    {
        
    }
}
