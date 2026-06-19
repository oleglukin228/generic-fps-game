using UnityEngine;

public class PlayerStandingTrappedState : PlayerState
{
    GameObject _enemySensor;
    public PlayerStandingTrappedState(EPlayerState stateId, IStateMachine<EPlayerState> stateMachine, PlayerController controller, GameObject enemySensor) : base(stateId, stateMachine, controller) 
    {
        _enemySensor = enemySensor;
    }

    public override void OnEnter()
    {
        
    }

    public override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            _stateMachine.SetState(EPlayerState.Crouching);
        }
    }

    public override void OnLateUpdate()
    {
        _controller.CameraControl();
    }
}
