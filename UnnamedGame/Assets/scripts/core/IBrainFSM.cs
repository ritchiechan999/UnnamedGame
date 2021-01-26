using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class IBrainFSM : MonoBehaviour
{
    Dictionary<Type, IState> _states = new Dictionary<Type, IState>();
    Type _currentState;
    IState _existing;
    public bool BrainEnabled = true;

    public void RegisterState(IState state)
    {
        _states[state.GetType()] = state;
    }

    public bool ChangeState(Type stateType, params object[] args)
    {
        //if same type
        if (_currentState == stateType)
            return false;

        //if not the one to be changing to
        if (!_states.TryGetValue(stateType, out IState newState))
            return false;

        //correct one
        if (_currentState != null && _states.TryGetValue(_currentState, out IState currentState))
            currentState.OnStateExit(args);

        _currentState = stateType;
        newState.OnStateEnter(args);
        _existing = newState;
        return true;
    }

    public void UpdateBrain()
    {
        if (!BrainEnabled)
            return;

        if (_existing == null) {
            throw new Exception(string.Format("Please used a correct state for entity"));
        }

        _existing.OnStateUpdate();
        print(_existing);
    }

    //still testing
    public bool HasState(Type key)
    {
        return _states.ContainsKey(key);
    }

    /// <summary>
    /// Will be used on the entity controller to separate inputs and behaviour.
    /// Can also be used as a reaction for AI
    /// </summary>
    /// <param name="msgtype">message type</param>
    /// <param name="args">anything to pass through</param>
    public void SendMessageToBrain(ugMessageType msgType, params object[] args)
    {
        _existing.OnReceiveMessage(msgType, args);
    }
}

public abstract class IState
{
    public IBrainFSM Brain;
    public IState(IBrainFSM brain)
    {
        Brain = brain;
    }
    public abstract void OnStateEnter(object[] args);
    public abstract void OnStateUpdate();
    public abstract void OnStateExit(object[] args);
    public abstract void OnReceiveMessage(ugMessageType msgType, object[] args);
}

public abstract class IBaseState<T> : IState where T : IBrainFSM
{
    protected T Entity;
    protected int InitConstruct;
    protected IBaseState(T brain, int initConstruct) : base(brain)
    {
        Entity = brain;
        InitConstruct = initConstruct; //to resolved default constructor issues
    }
    protected IBaseState(T brain) : base(brain)
    {
        Entity = brain;
    }
}

public enum ugMessageType
{
    None,
    Move,
    Jump,
    Attack,
    Flinch
}