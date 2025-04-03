/// <summary>
/// Interface used for handling individual objectives inside the ObjectiveStateMachine.
/// Used only for objectives in DaniDevRoom State (and scene)
/// </summary>
public interface IObjectiveState 
{
    string ObjectiveDescription { get; }

    void EnterObjective();
    void ExecuteObjective();
    void ExitObjective();

}
