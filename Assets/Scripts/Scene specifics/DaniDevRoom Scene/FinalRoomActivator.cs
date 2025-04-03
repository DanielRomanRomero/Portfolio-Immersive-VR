using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Comfort;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

/// <summary>
/// Triggered when the player stops in front of the sofa during the final objective.
/// Disables player movement and starts the lamp cracking sequence.
/// </summary>
public class FinalRoomActivator : MonoBehaviour
{
    [Header("Player Control")]
    [SerializeField] private LocomotionProvider locomotionProvider; 
    [SerializeField] private CharacterController characterController;
    [SerializeField] private TunnelingVignetteController tunnelingVignetteController;

    [Header("Lamp Crack Sounds")]
    [SerializeField] private AudioSource lampSoundSource;
    [SerializeField] private AudioClip[] cracks;

    [Header("LampController")]
    [SerializeField] private LampController lampController;
    private bool lampIsFalling = false;
    public bool LampIsFalling { get => lampIsFalling; set => lampIsFalling = value; }

    [Header("Highlight")]
    [SerializeField] private GameObject highlightGameObject;

    private Coroutine coroutine;
    private bool cinematicActive = false;

    public void ActivateHighlight()
    {
        highlightGameObject.SetActive(true);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!cinematicActive)
            {
                // 1. Disable highlight
                highlightGameObject.SetActive(false);

                // 2. Hide objective text
                ObjectiveUI.Instance.HideObjectiveText();

                // 3. Disable player movement
                locomotionProvider.GetComponent<DynamicMoveProvider>().moveSpeed = 0f;

                // 4. Play first lamp crack sound
                lampSoundSource.Play();

                // 5. Allow the lamp to fall
                lampController.FinalCutsceneHasStarted = true;

                // 6. Start sound sequence
                StartFinal();

                cinematicActive = true;
            }

        }

    }

    private void StartFinal()
    {
        coroutine = StartCoroutine(FinalCutScene());
    }

    private IEnumerator FinalCutScene()
    {
        while(!LampIsFalling) // If player don't look up, lamp keeps cracking 
        {
            lampSoundSource.PlayOneShot(cracks[0]);
            yield return new WaitForSeconds(1.5f);

            lampSoundSource.PlayOneShot(cracks[1]);
            yield return new WaitForSeconds(1.5f);

            lampSoundSource.PlayOneShot(cracks[2]);
            yield return null;
        }
        
        coroutine = null;
    }
}
