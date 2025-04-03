using UnityEngine;
/// <summary>
/// Manages a sequence of objective states. Used primarily during tutorial or objective-based sequences.
///  It's a Data container that will be fill with every needed state, for example: LightsOnObjectiveState, NotebookObjectiveState, etc.
/// </summary>
public class ObjectiveStateMachine 
{
   
    private IObjectiveState currentObjective;

    public ObjectiveStateMachine()
    {
        Debug.Log("ObjectiveStateMachine created");
    }

    /// <summary>
    /// Replaces the current objective with a new one. Also updates the UI.
    /// </summary>
    public void ChangeObjective(IObjectiveState newObjective)
    {
        currentObjective?.ExitObjective();

        currentObjective = newObjective;

        currentObjective.EnterObjective();

        ObjectiveUI.Instance.UpdateObjectiveText(currentObjective.ObjectiveDescription);

    }

    /// <summary>
    /// Executes the currently active objective.
    /// </summary>
    public void ExecuteObjective()
    {
        currentObjective?.ExecuteObjective();
    }

    /// <summary>
    /// Clears the current objective. Called when the objective state machine ends.
    /// </summary>
    public void FinishObjectiveMachine()
    {
        currentObjective?.ExitObjective();
        currentObjective = null;
    }

}
