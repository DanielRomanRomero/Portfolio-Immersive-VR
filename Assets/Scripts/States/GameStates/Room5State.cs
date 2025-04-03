using UnityEngine;
/// <summary>
/// Final room state. Starts a countdown and plays the final robot voice line when entered.
/// 
/// This state is part of a modular State Machine architecture used throughout the prototype.
/// Even though the project is small, this pattern was intentionally used to demonstrate solid and scalable architecture practices.
/// </summary>
public class Room5State : IState
{
    // The name of the state
    private string stateName;
    private DeathCountdown deathCountdown;

    public Room5State()
    {
        // Set the state name to the name of the class
        stateName = typeof(Room5State).Name;
    }


    public void EnterState()
    {
        // Get the DeathCountdown component from the scene
        deathCountdown = GameObject.FindFirstObjectByType<DeathCountdown>();

        // Play robot voice line
        RobotSoundManager.Instance.SetRoom(5);
        RobotSoundManager.Instance.PlayNextClip();
        Debug.LogFormat("Entering {0} ", stateName);

    }
    public void ExecuteState()
    {
        deathCountdown.StartCoundown();
        Debug.LogFormat("Executing {0} ", stateName);
    }
    public void ExitState()
    {
        Debug.LogFormat("Exiting {0} ", stateName);
    }



}
