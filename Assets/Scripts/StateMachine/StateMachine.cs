using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineBase<TStateEnum> : IStateMachine<TStateEnum> where TStateEnum : struct, Enum
{
    protected Dictionary<TStateEnum, IState<TStateEnum>> states = new Dictionary<TStateEnum, IState<TStateEnum>>();
    protected IState<TStateEnum> currentState;
    protected IState<TStateEnum> previousState;

    public TStateEnum? CurrentStateId => currentState?.StateId;
    public TStateEnum? PreviousStateId => previousState?.StateId;
    public IState<TStateEnum> CurrentState => currentState;
    public IState<TStateEnum> PreviousState => previousState;

    // Реализация IStateMachineNode
    public Enum CurrentStateIdEnum => CurrentStateId;
    public Enum PreviousStateIdEnum => PreviousStateId;
    public IStateNode CurrentStateNode => currentState;
    public IStateNode PreviousStateNode => previousState;

    public event Action OnStateChanged;
    public void NotifyStateChanged() => OnStateChanged?.Invoke();

    public void AddState(IState<TStateEnum> state)
    {
        if (!states.ContainsKey(state.StateId))
            states.Add(state.StateId, state);
    }

    public bool IsTransitioning { get; private set; }

    public bool SetState(TStateEnum? stateId)
    {
        // Защита от рекурсивных вызовов в момент транзакции
        if (IsTransitioning) return false;

        if (stateId == null)
        {
            IsTransitioning = true; // Начали смену
            try
            {
                currentState?.OnExit();
                currentState = null;
            }
            finally
            {
                IsTransitioning = false; // Закончили смену
            }

            NotifyStateChanged();
            return true;
        }

        if (states.TryGetValue(stateId.Value, out IState<TStateEnum> newState))
        {
            if (currentState != null && !currentState.CanTransitionTo(stateId.Value))
                return false;

            IsTransitioning = true; // Начали смену
            try
            {
                previousState = currentState;
                currentState?.OnExit();
                currentState = newState;
                currentState.OnEnter();
            }
            finally
            {
                IsTransitioning = false; // Закончили смену
            }

            NotifyStateChanged(); // Уведомляем только один раз в самом конце
            return true;
        }
        return false;
    }

    public bool HasState(TStateEnum stateId) => states.ContainsKey(stateId);

    public void ReturnToPreviousState()
    {
        if (previousState != null)
            SetState(previousState.StateId);
    }

    public void Update() => currentState?.OnUpdate();
    public void LateUpdate() => currentState?.OnLateUpdate();

    public bool IsInCurrentState(TStateEnum stateId)
    {
        throw new NotImplementedException();
    }

    public bool IsInPreviousState(TStateEnum stateId)
    {
        throw new NotImplementedException();
    }
}

public class StateMachine<TStateEnum> : MonoBehaviour, IStateMachine<TStateEnum> where TStateEnum : struct, Enum
{
    [SerializeField] protected TStateEnum initialStateId;

    [Header("Deep State Tracking")]
    [SerializeField] private string currentStatePath;
    [SerializeField] private string previousStatePath;

    // Публичные свойства для доступа к цепочке состояний из кода
    public List<Enum> CurrentStateChain { get; private set; } = new List<Enum>();
    public List<Enum> PreviousStateChain { get; private set; } = new List<Enum>();

    private StateMachineBase<TStateEnum> smBase = new StateMachineBase<TStateEnum>();

    public TStateEnum? CurrentStateId => smBase.CurrentStateId;
    public TStateEnum? PreviousStateId => smBase.PreviousStateId;
    public IState<TStateEnum> CurrentState => smBase.CurrentState;
    public IState<TStateEnum> PreviousState => smBase.PreviousState;

    // Проброс IStateMachineNode
    public Enum CurrentStateIdEnum => smBase.CurrentStateIdEnum;
    public Enum PreviousStateIdEnum => smBase.PreviousStateIdEnum;
    public IStateNode CurrentStateNode => smBase.CurrentStateNode;
    public IStateNode PreviousStateNode => smBase.PreviousStateNode;
    public event Action OnStateChanged
    {
        add => smBase.OnStateChanged += value;
        remove => smBase.OnStateChanged -= value;
    }
    public void NotifyStateChanged() => smBase.NotifyStateChanged();
    public bool IsTransitioning => smBase.IsTransitioning;

    private void Awake()
    {
        smBase.OnStateChanged += HandleStateHierarchyChanged;
        InitializeStates();
    }

    private void Start()
    {
        SetState(initialStateId);
    }

    public virtual void Update() => smBase.Update();
    public void LateUpdate() => smBase.LateUpdate();

    public void AddState(IState<TStateEnum> state) => smBase.AddState(state);

    public bool SetState(TStateEnum? stateId)
    {
        var result = smBase.SetState(stateId);
        if (!result && stateId.HasValue)
        {
            Debug.LogWarning($"State {stateId} not found or transition rejected!");
        }
        return result;
    }

    public bool HasState(TStateEnum stateId) => smBase.HasState(stateId);
    public void ReturnToPreviousState() => smBase.ReturnToPreviousState();

    protected virtual void InitializeStates() { }
    public void InitState(TStateEnum state) => initialStateId = state;

    // --- Логика обхода дерева ---
    private void HandleStateHierarchyChanged()
    {
#if UNITY_EDITOR
        var newChain = GetCurrentStateChain(smBase);
        var newPath = string.Join(" -> ", newChain);

        // Игнорируем дублирующиеся события (чтобы не было спама при множественном OnEnter)
        if (newPath == currentStatePath) return;

        // Текущая конфигурация становится "предыдущей" конфигурацией системы
        PreviousStateChain = new List<Enum>(CurrentStateChain);
        previousStatePath = currentStatePath;

        // Обновляем текущую конфигурацию
        CurrentStateChain = newChain;
        currentStatePath = newPath;
#endif
    }

    private List<Enum> GetCurrentStateChain(IStateMachineNode node)
    {
        List<Enum> chain = new List<Enum>();
        var currentNode = node;

        while (currentNode != null)
        {
            if (currentNode.CurrentStateIdEnum != null)
            {
                chain.Add(currentNode.CurrentStateIdEnum);
            }

            // Переходим вглубь к подсостоянию
            var state = currentNode.CurrentStateNode;
            currentNode = state?.SubStateMachineNode;
        }

        return chain;
    }

    public bool IsInCurrentState(TStateEnum stateId) { return CurrentStateChain.Contains(stateId); }
    public bool IsInPreviousState(TStateEnum stateId) { return PreviousStateChain.Contains(stateId); }
}