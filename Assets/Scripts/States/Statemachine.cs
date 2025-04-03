using UnityEngine;

/// <summary>
/// Main class for handling game state transitions using the State pattern.
/// Stores the current, previous, and next state, and allows transitions between them.
/// </summary>
public class Statemachine 
{
    // The name of the current state
    private string currentStateName;
    // The current state
    private IState currentState;
    // The previous state
    private IState previousState;
    // The next state
    private IState nextState;

    // Properties
    public IState CurrentState { get => currentState; }
    public IState Previoustate { get => previousState; }
    public IState NextState { get => nextState; }
    public string CurrentStateName { get => currentStateName;}

    // Initial constructor
    public Statemachine()
    {
        Debug.Log("StateMachine created");
    }

    /// <summary>
    /// Transitions to a new state, calling exit/enter accordingly.
    /// </summary>
    public void ChangeState(IState newState)
    {
        currentState?.ExitState();
        previousState = currentState;

        SetNextState(newState);

        currentState = newState;
        currentStateName = currentState.GetType().Name;
        currentState.EnterState();
    }

    /// <summary>
    /// Calls the execution logic of the current state.
    /// </summary>
    public void ExecuteStateUpdate()
    {
        currentState?.ExecuteState();
    }

    /// <summary>
    /// Returns to the previous state, if any.
    /// </summary>
    public void ChangeToPreviousState()
    {
        if (previousState != null)
        {
            currentState.ExitState();
            currentState = previousState;
            currentStateName = currentState.GetType().Name;
            currentState.EnterState();
        }
    }

    /// <summary>
    /// Advances to the predefined next state, if available.
    /// </summary>
    public void ChangeToNextState()
    {
        if (nextState != null)
        {
            currentState.ExitState();
            currentState = nextState;
            currentStateName = currentState.GetType().Name;
            currentState.EnterState();
        }
    }

    /// <summary>
    /// Sets the next state that will be used when calling ChangeToNextState.
    /// </summary>
    public void SetNextState(IState state)
    {
        nextState = state;
    }

    /// <summary>
    /// Exits the current state and clears it. Called when the game ends.
    /// </summary>
    public void ExitStateMachine()
    {
        currentState.ExitState();
        currentState = null;
        Debug.Log("State machine is Over. Game Has Finished");
    }


}
