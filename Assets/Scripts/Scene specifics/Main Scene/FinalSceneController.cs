using UnityEngine;

/// <summary>
/// Simple script that checks if the player has reached the final area (roof).
/// Triggers final logic through GameManager only once.
/// </summary>
public class FinalSceneController : MonoBehaviour
{
    private bool playerIsOnTheRoof = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerIsOnTheRoof) return;
            GameManager.Instance.Room5Finished();

            playerIsOnTheRoof = true;
        }
    }

}
