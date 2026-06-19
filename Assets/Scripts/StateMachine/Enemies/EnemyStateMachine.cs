using UnityEngine;

public enum EEnemyState
{
    Idle,
    Wandering,
    Chasing,
    LostTarget,
    Attack,
    PullVictimToRape,
    Eating,
    IdleCrouch,
    Death
}

public class EnemyStateMachine : StateMachine<EEnemyState>
{
    
}
