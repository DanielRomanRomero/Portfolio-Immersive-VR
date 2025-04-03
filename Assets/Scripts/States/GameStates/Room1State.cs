using UnityEngine;

/// <summary>
/// State that handles Room 1 (puzzle section).
/// It starts the related robot voice line.
/// 
/// This state is part of a modular State Machine architecture used throughout the prototype.
/// Even though the project is small, this pattern was intentionally used to demonstrate solid and scalable architecture practices.
/// </summary>
public class Room1State : IState
{
    // The name of the state
    private string stateName;

    public Room1State()
    {
        // Set the state name to the name of the class
        stateName = typeof(Room1State).Name;
    }


    public void EnterState()
    {
        Debug.LogFormat("Entering {0} ", stateName);
        RobotSoundManager.Instance.SetRoom(1);
        RobotSoundManager.Instance.PlayNextClip(2); 
    }
    public void ExecuteState()
    {
        Debug.LogFormat("Executing {0} ", stateName);
    }
    public void ExitState()
    {
        Debug.LogFormat("Exiting {0} ", stateName);
    }
}
