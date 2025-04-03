using UnityEngine;
/// <summary>
/// Final objective in the DaniDevRoom scene: rest on the sofa.
/// 
/// Triggers the transition to the final part of the tutorial sequence.
/// </summary>
public class RestOnSofaObjectiveState : IObjectiveState
{
    private ObjectiveStateMachine ObjectiveSM;

    public string ObjectiveDescription => "Rest on the sofa";

    public RestOnSofaObjectiveState (ObjectiveStateMachine ObjectiveSM)
    {
        this.ObjectiveSM = ObjectiveSM;
    }

    public void EnterObjective()
    {
        Debug.Log("Final Objective: Rest on the sofa.");
        GameManager.Instance.ActiveFinalRoomSequence(); // Triggers final room events
    }

    public void ExecuteObjective()
    {
        // No real-time logic required; handled externally when completed
    }

    public void ExitObjective()
    {
        Debug.Log("Objective completed: End of DaniDevRoom (Level 0 or 1).");
    }
}
