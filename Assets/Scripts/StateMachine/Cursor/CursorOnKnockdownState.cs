using System;
using UnityEngine;

public class CursorOnKnockdownState : CursorState
{
    public CursorOnKnockdownState(ECursorState stateId, IStateMachine<ECursorState> stateMachine, CursorController controller) : base(stateId, stateMachine, controller)
    {
    }

    public override void OnLateUpdate()
    {
        _controller.OnKnockdownCursorUpdate();
    }
}
