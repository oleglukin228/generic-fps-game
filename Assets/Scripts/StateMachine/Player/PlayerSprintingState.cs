using UnityEngine;

public class PlayerSprintingState : PlayerState
{
    public PlayerSprintingState(EPlayerState stateId, IStateMachine<EPlayerState> stateMachine, PlayerController controller) : base(stateId, stateMachine, controller) { }

    public override void OnEnter()
    {
        _controller.animator.SetBool("isMoving", true);
        _controller.ProcessMovement(_controller.defaultSpeed.Value * _controller.sprintSpeedMultiplier.Value);
        if (_controller.weaponManager.HeldWeapon != null)
        {
            _controller.weaponManager.HeldWeapon.OnSprintingState(true);
            _controller.weaponManager.HeldWeapon.Reload(false);
        }
    }

    public override void OnUpdate()
    {
        _controller.ProcessMovement(_controller.defaultSpeed.Value * _controller.sprintSpeedMultiplier.Value);
        if (_controller.InputDirection.y == 0 && !_controller.IsMoving)
        {
            //Debug.Log("CURRENT SUBSTATE: WALKINg");
            _stateMachine.SetState(EPlayerState.Idle);
        }
        else if (_controller.InputDirection.y != 0 && !_controller.IsRunPressed || _controller.InputDirection.x != 0)
        {
            //Debug.Log("CURRENT SUBSTATE: SPRINTING");
            _stateMachine.SetState(EPlayerState.Walking);
        }

        if (!_controller.health.IsTired) _controller.health.DecreaseHealth();
        else _stateMachine.SetState(EPlayerState.Walking);
    }
    public override void OnExit() 
    {
        _controller.weaponManager.HeldWeapon?.OnSprintingState(false);
    }
}

/*public class PlayerSprintingState : PlayerBaseState
{
    public PlayerSprintingState(PlayerController currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {

    }
    public override void CheckSwitchStates()
    {
        if (Ctx.InputDirection.y == 0)
        {
            //Debug.Log("CURRENT SUBSTATE: WALKINg");
            SwitchState(Factory.Idle());
        }
        else if (Ctx.InputDirection.y != 0 && !Ctx.IsRunPressed)
        {
            //Debug.Log("CURRENT SUBSTATE: SPRINTING");
            SwitchState(Factory.Walking());
        }
        /*else if (Input.GetKeyDown(KeyCode.C))
        {
            SwitchState(Factory.Crouching());
        }*
    }

    public override void EnterState()
    {
        Ctx.TargetSpeed = Ctx.sprintSpeed;
        if (Ctx.weaponManager.HeldWeapon != null)
        {
            Ctx.weaponManager.HeldWeapon.OnSprintingState(true);
            Ctx.weaponManager.HeldWeapon.Reload(false);
        }
    }

    public override void ExitState()
    {
        Ctx.weaponManager.HeldWeapon?.OnSprintingState(false);
    }

    public override void InitializeSubState()
    {
        
    }

    public override void UpdateState()
    {
        //Ctx.controller.Move((Ctx.CurrentSpeed * Ctx.moveDirection) * Time.deltaTime); // ??????????? ????????? ? ??????? CharacterController
        CheckSwitchStates();
    }
}*/
