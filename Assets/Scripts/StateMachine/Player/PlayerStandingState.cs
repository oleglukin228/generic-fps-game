using UnityEngine;

public class PlayerStandingState : PlayerRootState
{
    public PlayerStandingState(EPlayerState stateId, IStateMachine<EPlayerState> stateMachine, PlayerController controller) : base(stateId, stateMachine, controller)
    {
        InitializeSubStates();
    }

    public override void InitializeSubStates()
    {
        AddSubState(new PlayerIdleState(EPlayerState.Idle, this.subStateMachine, _controller));
        AddSubState(new PlayerWalkingState(EPlayerState.Walking, this.subStateMachine, _controller));
        AddSubState(new PlayerSprintingState(EPlayerState.Sprinting, this.subStateMachine, _controller));
        AddSubState(new PlayerAimingState(EPlayerState.Aiming, this.subStateMachine, _controller));

        SetSubState(EPlayerState.Idle);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        //Debug.Log("standing");
        if (Input.GetKeyDown(KeyCode.Space) && !_controller.IsTired && _controller.IsGrounded)
        {
            //Ctx.VelocityDirection = Ctx.animator.velocity;
            //SwitchState(Factory.Jumping());
        }
        /*else if (!_controller.IsGrounded)
        {
            _controller.VelocityDirection = _controller.animator.angularVelocity;
            _stateMachine.SetState("Falling");
        }*/
        if (Input.GetKeyDown(KeyCode.C))
        {
            _stateMachine.SetState(EPlayerState.Crouching);
            SetSubState(EPlayerState.Idle);
        }
    }
}
