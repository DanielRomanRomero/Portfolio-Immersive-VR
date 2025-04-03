using UnityEngine;

/// <summary>
/// Handles the forward movement of each cube in the Beat Saber-style minigame.
/// Each cube moves in its local forward direction until it either gets hit or reaches the finish line.
/// </summary>
public class CubeMovement : MonoBehaviour
{
    private Rigidbody rb;

    [Tooltip("Forward movement speed of the cube")]
    public float speed = 2f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Initialization method (currently not used but kept for potential future use).
    /// </summary>
    /// <param name="dspTime">Audio start time for synchronization (unused)</param>
    /// <param name="startPos">Starting position of the cube (unused)</param>
    public void InitializeCube(double dspTime, Vector3 startPos)
    {
        // Placeholder for any future setup
    }


    private void FixedUpdate()
    {
        // Constantly move the cube forward in its local direction
        rb.MovePosition(transform.position + transform.forward * Time.deltaTime * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the cube hits the finish trigger, deactivate it (simulate despawn)
        if (other.CompareTag("Finish"))
        {
           gameObject.SetActive(false);
           // In the future, this could return the cube to a pool instead

        }
    }
}
