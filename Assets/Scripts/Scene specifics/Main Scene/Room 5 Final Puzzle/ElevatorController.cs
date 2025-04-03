using UnityEngine;

/// <summary>
/// Handles player parenting for elevators, to make them move with the platform.
/// Elevator is at room5 (the end of the game)
/// </summary>
public class ElevatorController : MonoBehaviour
{
    [SerializeField] private GameObject playerLocomotion;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.parent = null;
        }
    }

    public void ReturnControlToPlayer()
    {
        if(!playerLocomotion.activeInHierarchy)
            playerLocomotion.SetActive(true);
    }

}
