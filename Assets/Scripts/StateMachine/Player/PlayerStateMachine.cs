using System;
using UnityEngine;

public enum EPlayerState
{
    Grounded,
    Falling,
    Knockdown,
    Standing,
    Crouching,
    Idle,
    Walking,
    Sprinting,
    Aiming,
    Trapped,
    Dead
}

public class PlayerStateMachine : StateMachine<EPlayerState>
{
    /*protected override void InitializeStates()
    {
        PlayerController player = GetComponent<PlayerController>();

        AddState(new PlayerGroundedState(EPlayerState.Grounded, this, player));
        AddState(new PlayerFallingState(EPlayerState.Falling, this, player));
        AddState(new PlayerKnockdownState(EPlayerState.Knockdown, this, player, null));

        initialStateId = EPlayerState.Grounded;
    }*/
}