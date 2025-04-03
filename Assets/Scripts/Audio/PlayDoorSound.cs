using System.Collections;
using UnityEngine;

/// <summary>
/// Handles playback of door and elevator-related sounds.
/// Includes support for elevator music and different door actions.
/// </summary>
public class PlayDoorSound : MonoBehaviour
{
    private AudioSource m_SoundSource;
    [SerializeField] private AudioClip m_door3_Opening;
    [SerializeField] private AudioClip m_door3_Closing;
    [SerializeField] private AudioClip elevatorMusic;

    private void Awake()
    {
        m_SoundSource = GetComponent<AudioSource>();
    }

    public void PlaySound()
    {
        m_SoundSource.Play();
    }   

    public void PlayDoor3OpenSound()
    {
        m_SoundSource.clip = m_door3_Opening;
        m_SoundSource.Play();
    }

    public void PlayDoor3CloseSound()
    {
        m_SoundSource.clip = m_door3_Closing;
        m_SoundSource.Play();
    }

    public void PlayElevatorMusic()
    {
       m_SoundSource.PlayOneShot(elevatorMusic);
    }

    private IEnumerator StopMusic()
    {
       yield return new WaitForSeconds(1.5f);
        m_SoundSource.Stop();
    }
}
