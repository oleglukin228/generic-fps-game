using UnityEngine.AI;

public class EnemyRootState : BaseHierarchicalState<EEnemyState, EEnemyState>
{
    protected EnemyController _controller;
    protected readonly NavMeshAgent _agent;
    public EnemyRootState(EEnemyState state, IStateMachine<EEnemyState> stateMachine, EnemyController controller) : base(state, stateMachine)
    {
        _stateMachine = stateMachine;
        _controller = controller;
        _agent = _controller.GetComponent<NavMeshAgent>();
    }

    public override void InitializeSubStates() { }
}

public class EnemyState : BaseState<EEnemyState>
{
    protected EnemyController _controller;
    protected readonly NavMeshAgent _agent;

    public EnemyState(EEnemyState state, IStateMachine<EEnemyState> stateMachine, EnemyController controller) : base(state, stateMachine)
    {
        _stateMachine = stateMachine;
        _controller = controller;
        _agent = _controller.GetComponent<NavMeshAgent>();
    }
}
