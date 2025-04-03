using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Detects when the notebook is placed on the table, triggers an event and advances the objective.
/// </summary>
public class TableController : MonoBehaviour
{

    private bool notebookIsOnTheTable = false;
    public UnityEvent OnNotebookPlaced;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Notebook_Brown" && !notebookIsOnTheTable)
        {
            OnNotebookPlaced.Invoke();
            notebookIsOnTheTable = true;
        }
    }

    public void ObjectiveTwoAcomplish()
    {
        GameManager.Objectives.ChangeObjective(new PlayAroundObjectiveState(GameManager.Objectives));
    }

}
