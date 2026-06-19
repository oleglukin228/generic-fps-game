using AudioSystem;
using UnityEngine;
using static PlayerStateMachine;

public class PlayerKnockdownState : PlayerState
{
    GameObject _enemySensor;
    public PlayerKnockdownState(EPlayerState stateId, IStateMachine<EPlayerState> stateMachine, 
        PlayerController controller, GameObject enemySensor) : base(stateId, stateMachine, controller) 
    {
        _enemySensor = enemySensor;
    }

    public override void OnEnter()
    {
        _controller.PlayerSFX.heartBeatSound.pitch = 3f;
        _controller.animator.SetBool("isKnockedDown", true);
        _enemySensor.SetActive(true);
        CursorController.SetState(ECursorState.OnKnockdown);
        SoundManager.Instance.CreateSoundBuilder()
            .WithPosition(_controller.transform.position)
            .Play(_controller.PlayerSFX.knockdownSound);
    }

    public override void OnUpdate()
    {
        //_controller.ProcessFalling();
        _controller.UpdateCameraPosition();
        if (_controller.health.IsFullHealth)
        {
            _stateMachine.SetState(_stateMachine.PreviousStateId);
        }
    }

    public override void OnLateUpdate()
    {
        _controller.OnKnockdownCameraControl();
    }

    public override void OnExit()
    {
        _controller.PlayerSFX.heartBeatSound.pitch = 1f;
        _controller.PlayerSFX.ChangeStaminaSound(_controller.PlayerSFX.calmBreathSound);
        _controller.animator.SetBool("isKnockedDown", false);
        _enemySensor.SetActive(false);
        CursorController.SetState(CursorController.PreviousState);
    }
}
