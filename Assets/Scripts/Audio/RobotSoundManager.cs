using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;

/// <summary>
/// Handles robot voice audio and subtitle playback throughout the Main scene.
/// Loads voice lines from RobotSoundModule ScriptableObjects and plays them with subtitles.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class RobotSoundManager : MonoBehaviour
{

    public static RobotSoundManager Instance;

    [Header("Robot Audio Settings")]
    [SerializeField] private List<RobotSoundModule> robotSounds = new List<RobotSoundModule>();
    [SerializeField] private TextMeshProUGUI subtitleText; 
    [SerializeField] private GameObject canvasSubtitles;

    [Header("Subtitle Canvas Group")]
    [SerializeField] private CanvasGroup subtitleCanvasGroup;
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("Subtitles Obstruction Handler")]
    [SerializeField] private SubtitleAvoidObstruction subtitlesScript;

    private RobotSoundModule currentRoom;
    private AudioSource audioSource;
    private int currentClip;
    private Coroutine subtitleRoutine;
    private Coroutine fadeRoutine;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        subtitleCanvasGroup.alpha = 0; // Hide subtitles initially
    }

    /// <summary>
    /// Set current room index to determine which voice lines to use.
    /// </summary>
    /// <param name="roomID">Index of the room (corresponds to robotSounds list)</param>
    public void SetRoom(int roomID)
    {
        currentRoom = robotSounds[roomID];
        currentClip = 0;
    }

    /// <summary>
    /// Play the next clip and its subtitles from the current room.
    /// </summary>
    public void PlayNextClip()
    {
        if (currentClip >= currentRoom.sounds.Count)
            return;

        // Get the data asociate to this module (clip + subtitles)
        RobotAudioData audioData = currentRoom.sounds[currentClip++];

      
        audioSource.clip = audioData.clip;  // Take the clip from data
        audioSource.Play();  // Play the audio


        // Stop current subtitle coroutine if active
        if (subtitleRoutine != null)
            StopCoroutine(subtitleRoutine);

        subtitleRoutine = StartCoroutine(PlaySubtitleSegments(audioData.subtitles));

    }

    /// <summary>
    /// Delayed playback of the next clip.
    /// </summary>
    public void PlayNextClip(float delay)
    {
       Invoke(nameof(PlayNextClip), delay);
    }

    /// <summary>
    /// Coroutine that plays subtitle segments in sequence with delays.
    /// </summary>
    private IEnumerator PlaySubtitleSegments(List<SubtitleSegment> segments)
    {

        canvasSubtitles.SetActive(true);

        // Reset subtitle canvas position to avoid visual obstruction
        if (canvasSubtitles.TryGetComponent(out SubtitleAvoidObstruction avoid))
            avoid.ResetPositionSubs();

        // Fade in
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeCanvasGroup(1));

        foreach (var segment in segments)
        {
            subtitleText.text = segment.text;
            yield return new WaitForSeconds(segment.delay);
        }

        // Fade out
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(FadeCanvasGroup(0));

        
    }

    /// <summary>
    /// Coroutine to fade subtitle canvas in or out.
    /// </summary>
    private IEnumerator FadeCanvasGroup(float targetAlpha)
    {
        float startAlpha = subtitleCanvasGroup.alpha;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            subtitleCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeDuration);
            yield return null;
        }
        if(targetAlpha == 0f)
            canvasSubtitles.SetActive(false);

        subtitleCanvasGroup.alpha = targetAlpha;
    }

}
