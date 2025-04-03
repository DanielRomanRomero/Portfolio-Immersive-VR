using System.Collections;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// Handles the logic of the falling lamp during the final cutscene.
/// Detects when the player looks up, triggers fall, and plays impact sounds.
/// </summary>
public class LampController : MonoBehaviour
{
    [SerializeField] private FinalRoomActivator finalRoomActivatorScript; // We use this to stop sound Cracking
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Rigidbody rbLamp;
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip crashAudioclip;
    [SerializeField] private AudioClip bodyImpactAudioclip;

    [SerializeField] private float lookUpThreshold = 0.8f; // Ajusta el ángulo necesario para que la lámpara caiga
    private bool lampHasFallen = false;

    private bool finalCutsceneHasStarted = false;
    public bool FinalCutsceneHasStarted { get => finalCutsceneHasStarted; set => finalCutsceneHasStarted = value; }

    private Coroutine lampCoroutine = null;


    private void Update()
    {
        if (!FinalCutsceneHasStarted) return;

        if (!lampHasFallen && IsLookingUp())
        {
            MakeLampFall();
            lampHasFallen |= true;
        }
    }

    private bool IsLookingUp()
    {
        // Calculate angle between direction look and up global axys 
        float angle = Vector3.Dot(playerCamera.forward, Vector3.up);

        // If angle is higher enough, means player is looking up
        return angle > lookUpThreshold;
    }


    public void MakeLampFall()
    {
        rbLamp.isKinematic = false;
        lampCoroutine = StartCoroutine(SoundFallingLamp());

        finalRoomActivatorScript.LampIsFalling = true;
        finalRoomActivatorScript.StopAllCoroutines();
    }


    private IEnumerator SoundFallingLamp()
    {
        audioSource.PlayOneShot(crashAudioclip);
        yield return new WaitForSeconds(1f);
        audioSource.PlayOneShot(bodyImpactAudioclip);
    }

}
