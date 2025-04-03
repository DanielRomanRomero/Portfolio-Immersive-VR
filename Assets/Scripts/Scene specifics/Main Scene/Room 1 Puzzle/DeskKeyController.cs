using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Puzzle logic for inserting desk keys in the right order at room 1
/// There are 3 deskKeys in the room
/// The correct order is: B > C > A.
/// The deskKeys are named DeskKeyB, DeskKeyC, DeskKeyA
/// </summary>
public class DeskKeyController : MonoBehaviour
{

    public UnityEvent OnPuzzleResolved;
    private int correctKeyIndex = 0;

    private void Start()
    {
        correctKeyIndex = 0;
    }
    public void CheckRightOrder(string deskKeyName)
    {
        //Everytime a key is introduced in a desk socket, we check if that desk socket is the correct one to sume correctKeyIndex

        if (correctKeyIndex == 0 && deskKeyName == "B")
        {
            correctKeyIndex++;
        }
        else if (correctKeyIndex == 1 && deskKeyName == "C")
        {
            correctKeyIndex++;
        }
        else if (correctKeyIndex == 2 && deskKeyName == "A")
        {
            correctKeyIndex++;
        }
        else
        {
            if(correctKeyIndex > 0)
                correctKeyIndex--;
        }

        if(correctKeyIndex == 3)
        {
            OnPuzzleResolved.AddListener(AddListenerToEvent);
            OnPuzzleResolved?.Invoke();
        }
    }

    private void AddListenerToEvent()
    {
        GameManager.Instance.Room1Finished();
    }

}
