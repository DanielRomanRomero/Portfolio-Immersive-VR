using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// UI class that displays and animates objective text in the game.
/// </summary>
public class ObjectiveUI : MonoBehaviour
{

    public static ObjectiveUI Instance;

    [SerializeField] private TMP_Text objectiveText;
    [SerializeField] private CanvasGroup canvasGroup;

    private Coroutine currentRoutine;

    private void Awake()
    {
        Instance = this;
        canvasGroup.alpha = 0f;
    }

    /// <summary>
    /// Updates the objective text
    /// </summary>
    public void UpdateObjectiveText(string newObjective)
    {
        objectiveText.text = newObjective;

        // reset any active coroutine
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(EnterTextObjective());
    }


    private IEnumerator EnterTextObjective()
    {
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0,1f,elapsed/duration);
            yield return null;
        }
        canvasGroup.alpha = 1;

    }

    /// <summary>
    /// Call this to hide the objective text.
    /// </summary>
    public void HideObjectiveText()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ExitTextObjective());
    }


    private IEnumerator ExitTextObjective()
    {
        float duration = 0.5f;
        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = 0;
    }

}
