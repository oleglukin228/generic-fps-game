using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(EPlayerState stateId, IStateMachine<EPlayerState> stateMachine, PlayerController controller) : base(stateId, stateMachine, controller) { }

    public override void OnEnter()
    {
        _controller.animator.SetBool("isMoving", false);
    }

    public override void OnUpdate()
    {
        _controller.ProcessMovement(0f);
        //Debug.Log("idling");
        if (_controller.InputDirection.y != 0 && _controller.IsRunPressed)
        {
            if (_controller.weaponManager.IsBusy) return;
            if (_controller.weaponManager.Reloading) return;
            _stateMachine.SetState(EPlayerState.Sprinting);
        }
        else if (_controller.InputDirection.magnitude != 0)
        {
            _stateMachine.SetState(EPlayerState.Walking);
        }
    }
}

/*public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerController currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {

    }
    public override void CheckSwitchStates()
    {
        if (Ctx.InputDirection.y != 0 && Ctx.IsRunPressed)
        {
            if (Ctx.weaponManager.IsBusy) return;
            if (Ctx.weaponManager.Reloading) return;
            SwitchState(Factory.Sprinting());
        }
        else if (Ctx.InputDirection.magnitude != 0)
        {
            SwitchState(Factory.Walking());
        }
        /*else if (Input.GetKeyDown(KeyCode.C))
        {
            SwitchState(Factory.Crouching());
        }*
    }

    public override void EnterState()
    {
        Ctx.animator.SetBool("isMoving", false);
    }

    public override void ExitState()
    {
        Ctx.animator.SetBool("isMoving", true);
    }

    public override void InitializeSubState()
    {
        
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }
}*/
