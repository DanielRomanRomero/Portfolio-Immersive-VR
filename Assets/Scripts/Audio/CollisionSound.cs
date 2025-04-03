using System.Collections;
using UnityEngine;

/// <summary>
/// Plays a sound when the object collides with specified layers.
/// Volume is scaled based on impact velocity using a curve.
/// </summary>
public class CollisionSound : MonoBehaviour
{
    [Tooltip("The layers that cause the sound to play")]
    public LayerMask collisionTriggers = ~0;

    [Tooltip("Source to play sound from")]
    public AudioSource source;

    [Tooltip("Clip to play on collision")]
    public AudioClip clip;

    [Tooltip("Velocity to volume curve")]
    public AnimationCurve velocityVolumeCurve = AnimationCurve.Linear(0, 0, 1, 1);
    
    [Space]

    public float volumeAmp = 0.8f;
    public float velocityAmp = 0.5f;
    public float soundRepeatDelay = 0.2f;

    Rigidbody rb;
    bool canPlaySound = true;
    Coroutine playSoundRoutine;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(SoundPlayBuffer(1f)); // Delay to prevent triggering on spawn
    }

    private void OnDisable()
    {
        if (playSoundRoutine != null)
            StopCoroutine(playSoundRoutine);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (rb == null && !gameObject.TryGetComponent(out rb))
            return;

        if (canPlaySound && collisionTriggers == (collisionTriggers | (1 << collision.gameObject.layer)))
        {
            if (source != null && source.enabled)
            {
                if (collision.collider.attachedRigidbody == null || collision.collider.attachedRigidbody.mass > 0.0000001f)
                {
                    if (clip != null || source.clip != null)
                        source.PlayOneShot(clip == null ? source.clip : clip, velocityVolumeCurve.Evaluate(collision.relativeVelocity.magnitude * velocityAmp) * volumeAmp);
                 
                    if (playSoundRoutine != null)
                        StopCoroutine(playSoundRoutine);

                    playSoundRoutine = StartCoroutine(SoundPlayBuffer());
                }
            }
        }
    }

    IEnumerator SoundPlayBuffer()
    {
        canPlaySound = false;
        yield return new WaitForSeconds(soundRepeatDelay);
        canPlaySound = true;
        playSoundRoutine = null;
    }

    IEnumerator SoundPlayBuffer(float time)
    {
        canPlaySound = false;
        yield return new WaitForSeconds(time);
        canPlaySound = true;
        playSoundRoutine = null;
    }
}
