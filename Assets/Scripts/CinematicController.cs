using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


/// <summary>
/// Controls the initial cinematic sequence at the beginning of the game.
/// It disables interaction and movement, plays a video, and re-enables systems once finished.
/// </summary>
public class CinematicController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playerLocomotion;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform startPoint;
    [SerializeField] private SkinnedMeshRenderer[] controllersRenderers;
    [SerializeField] private GameObject[] nearFarInteractorsPlayer;
    [SerializeField] private Transform startPlayerPosition;
    [SerializeField] private Camera mainCamera;

    [Header("Others")]
    [SerializeField] private MeshRenderer[] notebookRenderers;
    [SerializeField] private GameObject gameContainer;
    [SerializeField] private XRSocketInteractor socketInteractorNotebook;
    [SerializeField] private XRGrabInteractable grabInteractableNotebook;

    public XRInteractionManager interactionManager;

    [SerializeField] private MeshRenderer[] meshTransparentRenderers;
    [SerializeField] private GameObject fadeImage;
    [SerializeField] private GameManager gameManager;

    [Header("VideoPlayer")]
    [SerializeField] private GameObject canvasVideoPlayer;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private float videoDuration = 11f;
    [SerializeField] private float finalFadeDuration = 1.0f;

    private bool videoHasStarted = false;
    private bool gameSetUpHasInvoke = false;
    private float time;

    /// <summary>
    /// Event triggered when the video finishes.
    /// Used to start the main game sequence.
    /// </summary>
    public UnityEvent OnVideoFinished;

    private void Awake()
    {
        // Disable movement and collision before the game starts
        playerLocomotion.SetActive(false);
        characterController.enabled = false;
    }

    private void Start()
    {
        // Hide player controllers (gloves)
        foreach (var gloves in controllersRenderers)
        {
            gloves.enabled = false;
        }

        // Hide notebook mesh renderers
        foreach (var renderers in notebookRenderers)
        {
            renderers.enabled = false;
        }

        // Disable player interactors
        foreach (var interactors in nearFarInteractorsPlayer)
        {
            interactors.SetActive(false);
        }

        time = 0f;
        videoPlayer.Play();
        videoHasStarted = true;
    }


    private void Update()
    {
        // Track video duration
        if (videoHasStarted && !gameSetUpHasInvoke)
        {
            time += Time.deltaTime;

            if(time >= videoDuration)
            {
                OnVideoFinished.Invoke();
                gameSetUpHasInvoke = true;
            }
        }

        // Rotate the canvas to always face the camera
        if (canvasVideoPlayer.activeSelf)
        {
            Vector3 cameraEulerAngles = mainCamera.transform.rotation.eulerAngles;
            canvasVideoPlayer.transform.rotation = Quaternion.Euler(cameraEulerAngles.x, cameraEulerAngles.y, 0f);
        }
    }

    /// <summary>
    /// Called after the video ends. Activates gameplay and disables the video canvas.
    /// </summary>
    public void GameSetUp() 
    {
        gameContainer.SetActive(true);

        foreach (MeshRenderer meshRenderer in meshTransparentRenderers)
        {
            meshRenderer.enabled = false;
        }

        if (interactionManager == null)
            interactionManager = FindFirstObjectByType<XRInteractionManager>();

        // Force attach notebook to the socket after cutscene
        interactionManager.SelectEnter(socketInteractorNotebook, (IXRSelectInteractable)grabInteractableNotebook);
       
        canvasVideoPlayer.SetActive(false);
        StartCoroutine(TakeOffFadeScreen());
    }

    /// <summary>
    /// Coroutine to fade out the screen, reset player position and enable gameplay elements.
    /// </summary>
    private IEnumerator TakeOffFadeScreen()
    {
        mainCamera.clearFlags = CameraClearFlags.Skybox;

        yield return new WaitForSeconds(finalFadeDuration);

        // Reset player position
        player.transform.SetPositionAndRotation(startPoint.position, startPoint.rotation);
        player.GetComponent<XROrigin>().MoveCameraToWorldLocation(startPoint.position);
       
        fadeImage.SetActive(false);

        // Enable transparent objects again (they were visible through canvas)
        foreach (MeshRenderer meshRenderer in meshTransparentRenderers)
        {
            meshRenderer.enabled = true;
        }

        // Enable locomotion and controllers
        playerLocomotion.SetActive(true);
        characterController.enabled = true;

        foreach (var gloves in controllersRenderers)
        {
            gloves.enabled = true;
        }

        foreach (var renderers in notebookRenderers)
        {
            renderers.enabled = true;
        }
        foreach (var interactors in nearFarInteractorsPlayer)
        {
            interactors.SetActive(true);
        }


        // Start game state machine
        gameManager.CreateFirstGameStateMachine();

        // Deactivate this object
        this.gameObject.SetActive(false);
    }

}
