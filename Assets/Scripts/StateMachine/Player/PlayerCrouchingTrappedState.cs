using UnityEngine;

public class PlayerCrouchingTrappedState : PlayerState
{
    GameObject _enemySensor;
    public PlayerCrouchingTrappedState(EPlayerState stateId, IStateMachine<EPlayerState> stateMachine, PlayerController controller, GameObject enemySensor) : base(stateId, stateMachine, controller) 
    { 
        _enemySensor = enemySensor;
    }

    public override void OnEnter()
    {
        _controller.animator.SetBool("isCrouching", true);
        //_controller.Hitbox.height = _controller.crouchHeight;
        //_controller.Hitbox.center /= 2f;
        //_enemySensor.SetActive(true);

        ControlHint.Instance.UpdateCKey(true);
        CursorController.SetState(ECursorState.OnKnockdown);
    }

    public override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            _stateMachine.SetState(EPlayerState.Standing);
        }
    }

    public override void OnLateUpdate()
    {
        _controller.OnKnockdownCameraControl();
    }

    public override void OnExit()
    {
        _controller.animator.SetBool("isCrouching", false);
        //_controller.Hitbox.height = _controller.StandingHeight;
        //_controller.Hitbox.center *= 2f;
        //_enemySensor.SetActive(false);

        ControlHint.Instance.UpdateCKey(false);
        CursorController.SetState(ECursorState.OnLive);
    }
}
