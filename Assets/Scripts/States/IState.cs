/// <summary>
/// Interface for general game states.
/// </summary>
public interface IState
{
    void EnterState();
    void ExecuteState();
    void ExitState();
}



