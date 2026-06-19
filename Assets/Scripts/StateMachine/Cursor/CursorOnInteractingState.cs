using UnityEngine;

public class CursorOnInteractingState : CursorState
{
    public CursorOnInteractingState(ECursorState stateId, IStateMachine<ECursorState> stateMachine, CursorController cursorController) : base(stateId, stateMachine, cursorController)
    {
    }

    public override void OnEnter()
    {
        
    }
}
