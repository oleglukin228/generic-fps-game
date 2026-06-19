using System;

// --- Базовые интерфейсы для обхода дерева состояний на любой глубине ---
public interface IStateNode
{
    Enum StateIdEnum { get; }
    IStateMachineNode SubStateMachineNode { get; }
}

public interface IStateMachineNode
{
    Enum CurrentStateIdEnum { get; }
    Enum PreviousStateIdEnum { get; }
    IStateNode CurrentStateNode { get; }
    IStateNode PreviousStateNode { get; }

    bool IsTransitioning { get; } // <--- НОВОЕ СВОЙСТВО

    event Action OnStateChanged;
    void NotifyStateChanged();
}
// ------------------------------------------------------------------------

public interface IState<TStateEnum> : IStateNode where TStateEnum : struct, Enum
{
    TStateEnum StateId { get; }
    void OnEnter();
    void OnUpdate();
    void OnLateUpdate();
    void OnExit();
    bool CanTransitionTo(TStateEnum stateId);
}

// Интерфейс для машины состояний
public interface IStateMachine<TStateEnum> : IStateMachineNode where TStateEnum : struct, Enum
{
    TStateEnum? CurrentStateId { get; }
    TStateEnum? PreviousStateId { get; }
    IState<TStateEnum> CurrentState { get; }
    IState<TStateEnum> PreviousState { get; }
    void AddState(IState<TStateEnum> state);
    bool SetState(TStateEnum? stateId);
    bool HasState(TStateEnum stateId);
    void ReturnToPreviousState();
    void Update();
    void LateUpdate();
    bool IsInCurrentState(TStateEnum stateId);
    bool IsInPreviousState(TStateEnum stateId);
}

// Базовый класс для обычных состояний
public abstract class BaseState<TStateEnum> : IState<TStateEnum> where TStateEnum : struct, Enum
{
    public TStateEnum StateId { get; protected set; }

    // Реализация IStateNode
    public Enum StateIdEnum => StateId;
    public virtual IStateMachineNode SubStateMachineNode => null;

    protected IStateMachine<TStateEnum> _stateMachine;

    public BaseState(TStateEnum stateId, IStateMachine<TStateEnum> stateMachine)
    {
        StateId = stateId;
        _stateMachine = stateMachine;
    }

    public virtual void OnEnter() { }
    public virtual void OnUpdate() { }
    public virtual void OnLateUpdate() { }
    public virtual void OnExit() { }
    public virtual bool CanTransitionTo(TStateEnum stateId) => true;
}

// Базовый класс для иерархических состояний
public abstract class BaseHierarchicalState<TStateEnum, TSubStateEnum> : IState<TStateEnum>
where TStateEnum : struct, Enum
where TSubStateEnum : struct, Enum
{
    public TStateEnum StateId { get; protected set; }

    // Реализация IStateNode
    public Enum StateIdEnum => StateId;
    public virtual IStateMachineNode SubStateMachineNode => subStateMachine;

    protected IStateMachine<TStateEnum> _stateMachine;
    protected IStateMachine<TSubStateEnum> subStateMachine;

    public BaseHierarchicalState(TStateEnum stateId, IStateMachine<TStateEnum> stateMachine)
    {
        StateId = stateId;
        _stateMachine = stateMachine;
        subStateMachine = new StateMachineBase<TSubStateEnum>();

        // Прокидываем событие наверх ТОЛЬКО если родитель прямо сейчас не меняет состояние
        subStateMachine.OnStateChanged += () =>
        {
            if (_stateMachine != null && !_stateMachine.IsTransitioning)
            {
                _stateMachine.NotifyStateChanged();
            }
        };
    }

    public virtual void OnEnter()
    {
        InitializeSubStates();
    }

    public virtual void OnUpdate()
    {
        subStateMachine?.Update();
    }

    public virtual void OnLateUpdate()
    {
        subStateMachine?.LateUpdate();
    }

    public virtual void OnExit()
    {
        subStateMachine?.SetState(null);
    }

    public virtual bool CanTransitionTo(TStateEnum stateId) => true;

    public virtual void InitializeSubStates() { }

    public bool SetSubState(TSubStateEnum? subStateId)
    {
        return subStateMachine?.SetState(subStateId) ?? false;
    }

    public TSubStateEnum? GetCurrentSubStateId()
    {
        return subStateMachine?.CurrentStateId;
    }

    public void AddSubState(IState<TSubStateEnum> subState)
    {
        subStateMachine?.AddState(subState);
    }
}