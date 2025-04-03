using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Moves the player along a path (like a conveyor belt) when entering the trigger.
/// Used in Room 1 to simulate being transported.
/// </summary>
public class BeltMovement : MonoBehaviour
{
    [SerializeField] private CharacterController playerTransform; 
    [SerializeField] private Transform targetPosition = null;

    public UnityEvent PlayerEntering;
    public UnityEvent OnTargetReached;

    private Coroutine movePlayerCoroutine;
    private readonly float extraSpeed = 1.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "XR Origin (XR Rig)")
        {
            //We detect player and triger the event to take out the movement control from the player and start moving the belt
            PlayerEntering.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.name == "XR Origin (XR Rig)")
        {
          
            //We detect player leaving and triger the event to stop the belt and return to the player the movement control
            OnTargetReached.Invoke();

            //Security check to stop the coroutine if it is running
            if(movePlayerCoroutine != null)
                StopCoroutine(movePlayerCoroutine);
        }
    }

    //The event call this function to move the player to the target position
    public void MovePlayerToTarget()
    {
        //If the coroutine is not running, we start it
        movePlayerCoroutine ??= StartCoroutine(MovePlayer());
       
    }

    /// <summary>
    /// Starts moving the player to the target using a coroutine.
    /// </summary>
    private IEnumerator MovePlayer()
    {
        while (Vector3.Distance(playerTransform.transform.position, targetPosition.position) > 0.8f)
        {
            //Move the player towards to the target position with characterController.Move function ( just in Z direction)
            Vector3 moveDirection = targetPosition.position - transform.position;
            moveDirection.y = 0; // force y axis to not move
            playerTransform.Move(moveDirection.normalized * Time.deltaTime * extraSpeed);

            yield return null;
        }
        //When the player reaches the target position, we stop the coroutine
        StopCoroutine(movePlayerCoroutine);
        movePlayerCoroutine = null;
        Destroy(this);

    }
}
