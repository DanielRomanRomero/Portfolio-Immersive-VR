using UnityEngine;


/// <summary>
/// Singleton class used to play sounds at specific positions in 3D space.
/// </summary>
public class PlaySoundAt : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    public static PlaySoundAt Instance { get; private set; }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Play a clip at a specific world position.
    /// </summary>
    public void PlayAtPosition(AudioClip clip, Vector3 position, float volume = 1f)
    {
        // move audiosource to the position
        audioSource.transform.position = position;

        // play one shot
        audioSource.PlayOneShot(clip, volume);
    }


    public void PlayOneShot(AudioClip clip, float volume = 1f)
    {
        audioSource.PlayOneShot(clip, volume);
    }
}
