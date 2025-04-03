using System.Collections;
using UnityEngine;

/// <summary>
/// Moves a platform back and forth between two points using Rigidbody.MovePosition.
/// Optionally allows permanent movement or movement triggered via an event.
/// </summary>
public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 1f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private bool permanentMovement = false;

    private Rigidbody rb;

    private Vector3 targetPosition;
    private bool isWaiting = false;
    private bool doItNow = false;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        targetPosition = pointB.position;
    }


    private void FixedUpdate()
    {
        if (doItNow)
        {
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                if (!isWaiting)
                {
                    if(audioSource.isPlaying)
                        audioSource.Stop();
                    
                    StartCoroutine(Wait());
                }
            }
            else
            {
                if(!audioSource.isPlaying)
                    audioSource.Play();

                Vector3 moveDirection = targetPosition - transform.position;
                rb.MovePosition(transform.position + moveDirection.normalized * speed * Time.fixedDeltaTime);
            }
        }
    }

    private IEnumerator Wait()
    {
        isWaiting = true;
        yield return new WaitForSeconds(2f);
        if (permanentMovement)
        {
            targetPosition = targetPosition == pointA.position ? pointB.position : pointA.position;
            isWaiting = false;
        }
      
    }

    public void SetStartMoving()
    {
        doItNow = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            other.transform.SetParent(transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            other.transform.parent = null;
    }

}
