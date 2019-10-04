using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : MonoBehaviour
{
    public abstract void Enter();

    public virtual void Exit()
    {

    }

    protected virtual void OnDestory()
    {

    }

    /*
    private void RemoveListeners()
    {
       
    }

    private void AddListeners()
    {
        
    }
    */
}

public abstract class StateMachine
{

    GameObject owner;

    public StateMachine(GameObject owner)
    {
        this.owner = owner;
    }

    public virtual State CurrentState
    {
        get { return _currentState; }
        set { Transition(value); }
    }

    protected State _currentState;

    protected bool _inTransition;

    protected Dictionary<Type, State> stateList = new Dictionary<Type, State>();

    public virtual T GetState<T>() where T : State
    {
        Type key = typeof(T);
        T target;
        if (!stateList.ContainsKey(key))
        {
            target = owner.GetComponent<T>();

            if (target == null)
                target = owner.AddComponent<T>();

            stateList.Add(key, target);

            return target;
        }

        return (T)stateList[key]; 
        
    }

    public virtual void ChangeState<T>() where T : State
    {
        CurrentState = GetState<T>();
    }

    protected virtual void Transition(State value)
    {
        if (_currentState == value || _inTransition) return;
        _inTransition = true;

        if (_currentState != null) _currentState.Exit();

        _currentState = value;

        if (_currentState != null) _currentState.Enter();

        _inTransition = false;
    }

    private void Update()
    {
        Control();
    }

    abstract protected void Control();



}

public interface IStateController
{
    StateMachine controller { get;}
}
