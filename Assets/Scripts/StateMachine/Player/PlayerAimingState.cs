using UnityEngine;

public class PlayerAimingState : PlayerState
{
    public PlayerAimingState(EPlayerState stateId, IStateMachine<EPlayerState> stateMachine, PlayerController controller) : base(stateId, stateMachine, controller) { }

    public override void OnEnter()
    {

    }

    public override void OnUpdate()
    {
        _controller.ProcessMovement(_controller.defaultSpeed.Value * _controller.crouchSpeedMultiplier.Value);

        if (_controller.InputDirection.magnitude == 0)
            _controller.animator.SetBool("isMoving", false);
        else
            _controller.animator.SetBool("isMoving", true);

        if (!CursorController.RMBhold && _controller.InputDirection.magnitude != 0 && _controller.IsRunPressed)
        {
            _stateMachine.SetState(EPlayerState.Sprinting);
        }
        else if (!CursorController.RMBhold && _controller.InputDirection.magnitude != 0)
        {
            _stateMachine.SetState(EPlayerState.Walking);
        }
    }
    public override void OnExit() 
    { 
        
    }
}

/*public class PlayerAimingState : PlayerBaseState
{
    public PlayerAimingState(PlayerController currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {

    }
    public override void CheckSwitchStates()
    {
        if (!CursorController.RMBhold && Ctx.InputDirection.magnitude != 0 && Ctx.IsRunPressed)
        {
            SwitchState(Factory.Sprinting());
        }
        else if (!CursorController.RMBhold && Ctx.InputDirection.magnitude != 0)
        {
            SwitchState(Factory.Walking());
        }
    }

    public override void EnterState()
    {
        Ctx.TargetSpeed = Ctx.crouchSpeed;
    }

    public override void ExitState()
    {
        
    }

    public override void InitializeSubState()
    {
        
    }

    public override void UpdateState()
    {
        if (Ctx.InputDirection.magnitude == 0)
            Ctx.animator.SetBool("isMoving", false);
        else
            Ctx.animator.SetBool("isMoving", true);
        CheckSwitchStates();
    }
}*/
