using System;
using UnityEngine;
using static PlayerStateMachine;

public class CursorState : BaseState<ECursorState>
{
    protected CursorController _controller;
    public CursorState(ECursorState stateId, IStateMachine<ECursorState> stateMachine, CursorController cursorController) : base(stateId, stateMachine)
    {
        _stateMachine = stateMachine;
        _controller = cursorController;
    }
}

