using UnityEngine;
/// <summary>
/// State that handles Room 3 (Beat Saber-style event).
/// Plays the introductory robot voice line for the room.
/// 
/// This state is part of a modular State Machine architecture used throughout the prototype.
/// Even though the project is small, this pattern was intentionally used to demonstrate solid and scalable architecture practices.
/// </summary>
public class Room3State : IState
{
    // The name of the state
    private string stateName;


    public Room3State()
    {
        // Set the state name to the name of the class
        stateName = typeof(Room3State).Name;
    }


    public void EnterState()
    {
        RobotSoundManager.Instance.SetRoom(3);
        RobotSoundManager.Instance.PlayNextClip();
        Debug.LogFormat("Entering {0} ", stateName);

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
