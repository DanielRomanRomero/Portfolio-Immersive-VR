using UnityEngine;
/// <summary>
/// Second objective: place the notebook on the table.
/// 
/// This objective highlights the target and relies on external interaction handling.
/// It's part of a modular objective system based on the State pattern.
/// </summary>
public class NotebookObjectiveState : IObjectiveState
{
    private ObjectiveStateMachine objectiveSM;

    public string ObjectiveDescription => "Place the notebook on the table";


    public NotebookObjectiveState(ObjectiveStateMachine objectiveSM)
    {
        this.objectiveSM = objectiveSM;
    }

    public void EnterObjective()
    {
        Debug.Log("New Objective: Place the notebook on the table.");
        GameManager.Instance.ActivateSecondObjectiveHighlight();
        
    }

    public void ExecuteObjective()
    {
        // Handled externally – no real-time checking needed here
    }

    public void ExitObjective()
    {
        Debug.Log("Objective completed: Notebook placed. Exiting state.");
    }

   
}
