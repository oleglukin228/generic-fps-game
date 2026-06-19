using System;
using UnityEngine;

public class CursorOnLiveState : CursorState
{
    public CursorOnLiveState(ECursorState stateId, IStateMachine<ECursorState> stateMachine, CursorController controller) : base(stateId, stateMachine, controller)
    {
    }

    public override void OnLateUpdate()
    {
        _controller.OnLiveCursorUpdate();
    }
}
