using UnityEngine;

/// <summary>
/// Simple interaction tracker. Increments interaction count when the racket hits the ball for the first time.
/// </summary>
public class RacketController : MonoBehaviour
{
    private bool firstTimeInteractingObject = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball") && !firstTimeInteractingObject)
        {
            GameManager.Instance.RegisterObjectInteraction();
            firstTimeInteractingObject = true;
        }
    }

}
