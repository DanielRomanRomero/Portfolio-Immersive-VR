using UnityEngine;

/// <summary>
/// Third objective: interact with 3 nearby objects.
/// 
/// It checks progress via a counter stored in GameManager, and transitions once the goal is reached.
/// </summary>
public class PlayAroundObjectiveState : IObjectiveState
{

    private ObjectiveStateMachine ObjectiveSM;
    private const int requiredInteractions = 3;

    public string ObjectiveDescription => "Play or interact with 3 objects around you (0/3)";


    public PlayAroundObjectiveState(ObjectiveStateMachine ObjectiveSM)
    {
        this.ObjectiveSM = ObjectiveSM;
    }


    public void EnterObjective()
    {
        Debug.Log("New Objective: Interact with 3 objects nearby.");
    }

    public void ExecuteObjective()
    {
        if(GameManager.Instance.InteractedObjectsCount >= requiredInteractions)
        {
            ObjectiveUI.Instance.HideObjectiveText();

            // Transition to final objective
            GameManager.Objectives.ChangeObjective(new RestOnSofaObjectiveState(GameManager.Objectives));
        }
     
    }

    public void ExitObjective()
    {
        Debug.Log("Objective completed: 3 objects interacted. Exiting state.");
    }
}
