using UnityEngine.EventSystems;
using UnityEngine;
using Unity.VisualScripting;

public class PlayerGroundedState : PlayerRootState
{
    public PlayerGroundedState(EPlayerState stateId, StateMachine<EPlayerState> stateMachine, PlayerController controller) : base(stateId, stateMachine, controller)
    {
        InitializeSubStates();
    }

    public override void InitializeSubStates()
    {
        AddSubState(new PlayerCrouchingState(EPlayerState.Crouching, this.subStateMachine, _controller));
        AddSubState(new PlayerStandingState(EPlayerState.Standing, this.subStateMachine, _controller));

        SetSubState(EPlayerState.Standing);
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        _controller.UpdateCameraPosition();

        if (Input.GetKeyDown(KeyCode.Space) && !_controller.IsTired && _controller.IsGrounded)
        {
            //Ctx.VelocityDirection = Ctx.animator.velocity;
            //SwitchState(Factory.Jumping());
        }
        else if (!_controller.IsGrounded)
        {
            _stateMachine.SetState(EPlayerState.Falling);
        }
    }

    public override void OnLateUpdate()
    {
        _controller.CameraControl();
    }
}

