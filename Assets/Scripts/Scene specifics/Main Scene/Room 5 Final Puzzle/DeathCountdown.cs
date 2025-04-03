using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// Countdown used in Room 5. Displays time and invokes an event when reaching 0.
/// </summary>
public class DeathCountdown : MonoBehaviour
{
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private float countdownTime = 5.0f;

    public UnityEvent OnCountdownFinished;

    private AudioSource audioSource;
    private float secondsLeft;
    private Coroutine myCoroutine;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        secondsLeft = countdownTime;
    }

    private IEnumerator Countdown(float time)
    {
        float minutesLeft;

        while (secondsLeft > 0)
        {
            secondsLeft --;

            //The last 5 seconds the pitch of the audio will increase
            if (secondsLeft <= 5)
            {
                audioSource.pitch = 1.5f;
            }

            audioSource.Play();
            minutesLeft = Mathf.FloorToInt(secondsLeft / 60);
            countdownText.text = string.Format("{0:00}:{1:00}", minutesLeft, secondsLeft % 60);

            yield return new WaitForSeconds(1f);
        }

        OnCountdownFinished?.Invoke();

        StopCoroutine(myCoroutine);
        myCoroutine = null;
    }


    public void StartCoundown()
    {
        if (myCoroutine == null)
            myCoroutine = StartCoroutine(Countdown(countdownTime));
    }
}
