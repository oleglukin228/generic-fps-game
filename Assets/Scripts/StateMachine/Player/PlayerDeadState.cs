using AudioSystem;
using UnityEngine;

public class PlayerDeadState : PlayerState
{
    public PlayerDeadState(EPlayerState stateId, IStateMachine<EPlayerState> stateMachine, PlayerController controller) : base(stateId, stateMachine, controller)
    {
    }

    public override void OnEnter()
    {
        _controller.weaponManager.OnRapedMethod();
        _controller.animator.SetBool("isRaped", true);
        CursorController.SetState(ECursorState.OnKnockdown);
        SoundManager.Instance.CreateSoundBuilder().WithPosition(_controller.transform.position).Play(_controller.PlayerSFX.rapedSound);
    }

    public override void OnUpdate()
    {
        _controller.health.StopRegenerating();
        _controller.UpdateCameraPosition();
    }

    public override void OnLateUpdate()
    {
        _controller.OnKnockdownCameraControl();
    }

    public override void OnExit()
    {
        _controller.animator.SetBool("isRaped", false);
    }
}
