using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

/// <summary>
/// Plays footstep sounds based on player distance travelled, only when smooth motion is enabled.
/// </summary>
public class FootstepsPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private float stepDistance = 0.75f;
    [SerializeField] private ControllerInputActionManager controllerInputActionManager;

    private bool smoothMotionIsEnabled = true;
    private Vector3 previousPosition;
    private float distanceTravelled;

    void Start()
    {
        previousPosition = transform.position;
    }

    void Update()
    {
        smoothMotionIsEnabled = controllerInputActionManager.smoothMotionEnabled;

        if (!smoothMotionIsEnabled) return;

        Vector3 currentPosition = transform.position;
        distanceTravelled += Vector3.Distance(currentPosition, previousPosition);

        if (distanceTravelled >= stepDistance)
        {
            PlayFootstep();
            distanceTravelled = 0f;
        }

        previousPosition = currentPosition;
    }

    private void PlayFootstep()
    {
        footstepAudioSource.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Length)]);
    }
}
