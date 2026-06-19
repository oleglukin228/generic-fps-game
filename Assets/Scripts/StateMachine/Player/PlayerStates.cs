using UnityEngine;

public class PlayerRootState : BaseHierarchicalState<EPlayerState, EPlayerState>
{
    protected PlayerController _controller;
    public PlayerRootState(EPlayerState stateId, IStateMachine<EPlayerState> stateMachine, PlayerController controller) : base(stateId, stateMachine)
    {
        _stateMachine = stateMachine;
        _controller = controller;
    }

    public override void InitializeSubStates() { }
}

public class PlayerState : BaseState<EPlayerState>
{
    protected PlayerController _controller;
    public PlayerState(EPlayerState stateId, IStateMachine<EPlayerState> stateMachine, PlayerController controller) : base(stateId, stateMachine)
    {
        _stateMachine = stateMachine;
        _controller = controller;
    }
}
