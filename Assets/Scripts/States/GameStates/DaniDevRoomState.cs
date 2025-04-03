using UnityEngine;

/// <summary>
/// Main game state for managing the entire DaniDevRoom scene.
/// When entered, it starts the objective state machine for the tutorial room.
/// </summary>
public class DaniDevRoomState : IState
{
    public void EnterState()
    {
        Debug.Log("Entering DaniDevRoomState");

        // Start the first objective inside this scene's objective state machine
        GameManager.Objectives.ChangeObjective(new LightsOnObjectiveState(GameManager.Instance.ObjectiveSM));
    }

    public void ExecuteState()
    {
        GameManager.Objectives.ExecuteObjective(); 
    }

    public void ExitState()
    {
        Debug.Log("Exiting DaniDevRoomState");
    }
}
