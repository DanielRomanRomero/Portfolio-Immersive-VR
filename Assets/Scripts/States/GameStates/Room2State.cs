using UnityEngine;
/// <summary>
/// State that handles Room 2 (shooter section).
/// Triggers the robot audio as the player enters.
/// 
/// This state is part of a modular State Machine architecture used throughout the prototype.
/// Even though the project is small, this pattern was intentionally used to demonstrate solid and scalable architecture practices.
/// </summary>
public class Room2State : IState
{
    // The name of the state
    private string stateName;


    public Room2State()
    {
        // Set the state name to the name of the class
        stateName = typeof(Room2State).Name;
    }


    public void EnterState()
    {
        Debug.LogFormat("Entering {0} ", stateName);
        RobotSoundManager.Instance.SetRoom(2);
        RobotSoundManager.Instance.PlayNextClip();
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
