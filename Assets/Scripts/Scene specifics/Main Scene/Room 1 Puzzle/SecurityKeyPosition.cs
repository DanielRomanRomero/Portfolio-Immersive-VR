using UnityEngine;

/// <summary>
/// Automatically resets the key's position if it moves too far from its spawn location.
/// Useful for cases where the key is accidentally thrown or lost.
/// </summary>
public class SecurityKeyPosition : MonoBehaviour
{
    [SerializeField] private Transform respawnTransform;
    [SerializeField] private float maxDistance = 2f;

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, respawnTransform.position);

        if(distance > maxDistance)
        {
            transform.position = respawnTransform.position;
        }
    }
}
