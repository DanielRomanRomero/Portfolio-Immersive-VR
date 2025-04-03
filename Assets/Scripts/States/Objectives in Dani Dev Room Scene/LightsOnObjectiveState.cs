using UnityEngine;

/// <summary>
/// First objective in the DaniDevRoom tutorial scene: turn on the lights.
/// 
/// This class is part of the Objective State Machine architecture, allowing scalable and modular goal logic.
/// Even though the prototype is small, this design was used intentionally to demonstrate the State pattern.
/// </summary>
public class LightsOnObjectiveState : IObjectiveState
{
    private ObjectiveStateMachine objectiveSM;
    public string ObjectiveDescription => "Turn on the lights.";


    public LightsOnObjectiveState(ObjectiveStateMachine objectiveSM)
    {
        this.objectiveSM = objectiveSM;
    }

    public void EnterObjective()
    {
        Debug.Log("New Objective: Turn on the lights.");
        // Logic could be triggered externally from XRSliderController
    }

    public void ExecuteObjective()
    {
        // Example logic could be placed here if the system actively checks light status
        // Not used in this prototype, as the objective is handled externally by Unity Events
        // in the class XRSliderController, attached to the light
    }

    public void ExitObjective()
    {
        Debug.Log("Objective completed: Lights turned on. Exiting state.");
        ObjectiveUI.Instance.HideObjectiveText();
                                                                                                            
    }

    /*
    // Example method if you wanted to check lights from here
    private bool LightsAreOn()
    {
        return false;
    }
    */
}
