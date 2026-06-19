using UnityEngine;

public class PlayerFallingState : PlayerRootState
{
    CharacterController _cc;
    public PlayerFallingState(EPlayerState stateId, IStateMachine<EPlayerState> stateMachine, PlayerController controller, CharacterController cc) : base(stateId, stateMachine, controller) 
    {
        _cc = cc;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        _controller.VelocityDirection = _cc.velocity;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        _controller.ProcessFalling();

        if (_controller.IsGrounded)
        {
            _stateMachine.SetState(EPlayerState.Grounded);
        }
    }

    public override void OnLateUpdate()
    {
        _controller.CameraControl();
    }

    public override void OnExit()
    {
        _controller.animator.SetBool("isFalling", false);
        base.OnExit();
    }
}