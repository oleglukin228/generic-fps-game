using UnityEngine;

public class PlayerTrappedState : PlayerRootState
{
    GameObject _enemySensor;

    public PlayerTrappedState(EPlayerState stateId, IStateMachine<EPlayerState> stateMachine, 
        PlayerController controller, GameObject enemySensor) : base(stateId, stateMachine, controller)
    {
        _enemySensor = enemySensor;
        InitializeSubStates();
    }

    public override void InitializeSubStates()
    {
        AddSubState(new PlayerStandingTrappedState(EPlayerState.Standing, this.subStateMachine, _controller, _enemySensor));
        AddSubState(new PlayerCrouchingTrappedState(EPlayerState.Crouching, this.subStateMachine, _controller, _enemySensor));
    }

    public override void OnEnter()
    {
        base.OnEnter();
        //SetSubState(_stateMachine.IsInPreviousState(EPlayerState.Standing) ? EPlayerState.Standing : EPlayerState.Crouching);
        SetSubState(EPlayerState.Standing);
        _controller.animator.SetBool("isMoving", false);
        _controller.ProcessMovement(0);
        //_controller.animator.applyRootMotion = false;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        _controller.UpdateCameraPosition();
    }

    public override void OnExit()
    {
        base.OnExit();
        //_controller.animator.applyRootMotion = true;
    }
}
