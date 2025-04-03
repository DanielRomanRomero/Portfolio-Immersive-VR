using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages the beat-based minigame in Room 3.
/// It controls the timing of the music and events that spawn cubes rhythmically using DSP time for precise audio sync.
/// </summary>
public class BeatController : MonoBehaviour
{
    [SerializeField] private float bpm;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Intervals[] intervals;
    [SerializeField] private SpawnCubes cubesSpawner;

    private bool eventRoom3IsActive = false;
    private double dspStartTime;


    /// <summary>
    /// Called from GameManager when player is at room 3 and robot-speech is over
    /// </summary>
    public void StartEventRoom3()
    {
        StartCoroutine(StartMusic());
        eventRoom3IsActive = true;
    }

    private IEnumerator StartMusic()
    {
        yield return new WaitForSeconds(2);

        dspStartTime = AudioSettings.dspTime + 0.1;
        audioSource.PlayScheduled(dspStartTime);
        StartCoroutine(CheckAudioCountdown());

        foreach (var interval in intervals)
        {
            interval.Initialize(dspStartTime, bpm);
            StartCoroutine(IntervalRoutine(interval));
        }
    }

    /// <summary>
    /// Triggers an event repeatedly according to its interval timing.
    /// </summary>
    private IEnumerator IntervalRoutine(Intervals interval)
    {
        while (eventRoom3IsActive)
        {
            double currentDspTime = AudioSettings.dspTime;
            double timeToWait = interval.NextTriggerTime - currentDspTime;

            if (timeToWait > 0)
                yield return new WaitUntil(() => AudioSettings.dspTime >= interval.NextTriggerTime);
            else
                yield return null;

            interval.TriggerEvent();
            interval.ScheduleNext();
        }
    }

    /// <summary>
    /// Monitors the music playback and triggers effects shortly before the clip ends.
    /// </summary>
    private IEnumerator CheckAudioCountdown()
    {
        
        double totalClipTime = audioSource.clip.length;// total clip duration
        
        double clipEndTime = dspStartTime + totalClipTime;// exactly moment that clips ends
        
        double triggerTime = clipEndTime - 2.0;// total clip duration -2 seconds

        // Wait untill dspTime reachs triggerTime 
        yield return new WaitUntil(() => AudioSettings.dspTime >= triggerTime);

        //event is done => it should stop intanciating
        eventRoom3IsActive = false;

        // here we trigger pitch down dramaticly
        StartCoroutine(LowerPitchEffect(1f, -01f));

        RobotSoundManager.Instance.PlayNextClip();

        // call to dissolve all active cubes
        cubesSpawner.DissolveAllActiveCubes();
        
        yield return new WaitUntil(() => AudioSettings.dspTime >= clipEndTime);

        audioSource.Stop();

        // call to dissolve all active cubes AGAIN in case more cube has been instanciate After call stop
        cubesSpawner.DissolveAllActiveCubes();
        //Debug.Log("audio is done and stop");

        GameManager.Instance.CheckRoom3Counters();

    }

    /// <summary>
    /// Dramatic pitch-down effect before music ends.
    /// </summary>
    private IEnumerator LowerPitchEffect(float duration, float targetPitch)
    {
        float startPitch = audioSource.pitch;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            audioSource.pitch = Mathf.Lerp(startPitch, targetPitch, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        audioSource.pitch = targetPitch;
    }
}


/// <summary>
/// Handles the scheduling of events based on BPM.
/// </summary>
[System.Serializable]
public class Intervals
{
    [SerializeField] private float _steps;
    [SerializeField] private UnityEvent _trigger;

    private double _intervalSeconds;
    public double NextTriggerTime { get; private set; }

    public void Initialize(double dspStartTime, float bpm)
    {
        _intervalSeconds = 60.0 / (bpm * _steps);
        NextTriggerTime = dspStartTime + _intervalSeconds;
    }

    public void TriggerEvent()
    {
        _trigger?.Invoke();
    }

    public void ScheduleNext()
    {
        NextTriggerTime += _intervalSeconds;
    }

}
