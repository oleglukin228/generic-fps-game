using System;
using UnityEngine;

public enum ECursorState
{
    OnLive,
    OnKnockdown,
    OnMenu
}

public class CursorStateMachine : StateMachine<ECursorState>
{
    protected override void InitializeStates()
    {
        CursorController _controller = GetComponent<CursorController>();

        AddState(new CursorOnLiveState(ECursorState.OnLive, this, _controller));
        AddState(new CursorOnKnockdownState(ECursorState.OnKnockdown, this, _controller));

        initialStateId = ECursorState.OnLive;
    }
}
