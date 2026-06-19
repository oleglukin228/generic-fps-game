using TMPro;
using UnityEngine;

public class PlayerCrouchingState : PlayerRootState
{
    public PlayerCrouchingState(EPlayerState stateId, IStateMachine<EPlayerState> stateMachine, PlayerController controller) : base(stateId, stateMachine, controller)
    {
        InitializeSubStates();
    }

    public override void InitializeSubStates()
    {
        AddSubState(new PlayerIdleState(EPlayerState.Idle, this.subStateMachine, _controller));
        AddSubState(new PlayerWalkingState(EPlayerState.Walking, this.subStateMachine, _controller));

        SetSubState(EPlayerState.Idle);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        _controller.animator.SetBool("isCrouching", true);
        _controller.Hitbox.height = _controller.crouchHeight;
        _controller.Hitbox.center /= 2f;
        ControlHint.Instance.UpdateCKey(true);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (Input.GetKeyDown(KeyCode.C))
        {
            _stateMachine.SetState(EPlayerState.Standing);
            SetSubState(EPlayerState.Idle);
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        _controller.animator.SetBool("isCrouching", false);
        _controller.Hitbox.height = _controller.StandingHeight;
        _controller.Hitbox.center *= 2f;
        ControlHint.Instance.UpdateCKey(false);
    }
}

/*public class PlayerCrouchingState : PlayerBaseState
{
    public PlayerCrouchingState(PlayerController currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
        InitializeSubState();
    }
    public override void CheckSwitchStates()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            SwitchState(Factory.Grounded());
        }
        else if (!Ctx.IsGrounded)
        {
            SwitchState(Factory.Falling());
        }
    }

    public override void EnterState()
    {
        Ctx.animator.SetBool("isCrouching", true);
        Ctx.controller.height = Ctx.crouchHeight;
        Ctx.controller.center /= 2f;
        ControlHint.Instance.UpdateCKey(true);
    }

    public override void ExitState()
    {
        Ctx.animator.SetBool("isCrouching", false);
        Ctx.controller.height = Ctx.StandingHeight;
        Ctx.controller.center *= 2f;
        ControlHint.Instance.UpdateCKey(false);
    }

    public override void InitializeSubState()
    {
        if (Ctx.InputDirection.y == 0)
        {
            SetSubState(Factory.Idle());
        }
        else
        {
            SetSubState(Factory.Walking());
        }
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }
}*/
