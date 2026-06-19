using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkingState : PlayerState
{
    public PlayerWalkingState(EPlayerState stateId, IStateMachine<EPlayerState> stateMachine, PlayerController controller) : base(stateId, stateMachine, controller) { }

    public override void OnEnter()
    {
        _controller.animator.SetBool("isMoving", true);
    }

    public override void OnUpdate()
    {
        _controller.ProcessMovement(_controller.defaultSpeed.Value);
        if (CursorController.RMBhold && _controller.weaponManager.HeldWeapon != null)
        {
            _stateMachine.SetState(EPlayerState.Aiming);
        }
        if (_controller.InputDirection.magnitude == 0f && !_controller.IsMoving)
        {
            _stateMachine.SetState(EPlayerState.Idle);
        }
        else if (_controller.IsRunPressed)
        {
            if (_controller.weaponManager.IsBusy) return;
            if (_controller.InputDirection.y == 0) return;
            if (_controller.InputDirection.x != 0) return;
            if (_controller.weaponManager.Reloading) return;
            if (_controller.health.IsTired) return;
            _stateMachine.SetState(EPlayerState.Sprinting);
        }
    }

    public override void OnExit()
    {
        
    }
}

/*public class PlayerWalkingState : PlayerBaseState
{
    public PlayerWalkingState(PlayerController currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {

    }
    public override void EnterState()
    {
        Ctx.TargetSpeed = Ctx.defaultSpeed;
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        
    }
    public override void InitializeSubState()
    {
        
    }
    public override void CheckSwitchStates()
    {
        if (CursorController.RMBhold)
        {
            SwitchState(Factory.Aiming());
        }
        if (Ctx.InputDirection.magnitude == 0f)
        {
            SwitchState(Factory.Idle());
        }
        else if (Ctx.InputDirection.y != 0 && Ctx.IsRunPressed)
        {
            if (Ctx.weaponManager.IsBusy) return;
            if (Ctx.weaponManager.Reloading) return;
            SwitchState(Factory.Sprinting());
        }
        /*else if (Input.GetKeyDown(KeyCode.C))
        {
            SwitchState(Factory.Crouching());
        }*
    }
}*/
