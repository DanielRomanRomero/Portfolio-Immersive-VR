using ArmnomadsGames;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;


/// <summary>
/// Centralized manager for runtime scene interactions and transitions across rooms in the main scene.
/// Links XR interactions to GameManager and controls teleports, buttons, and game flow.
/// </summary>
public class MainSceneManager : MonoBehaviour
{

    [Header("Triggers and Interactions")]
    [SerializeField] private XRSimpleInteractable xRSimpleInteractable_Room0;
    [SerializeField] private TeleportationAnchor teleportMainPlatform_Room2;
    [SerializeField] private XRGrabInteractable grabInteractablePistols_Room2;
    [SerializeField] private TeleportationAnchor teleportMainPlatform_Room3;
    [SerializeField] private TeleportationAnchor teleportMainPlatform_Room4;
    [SerializeField] private Button button_Room5;
    [SerializeField] private DeathCountdown countdown_Room5;


    [Header("Main Room Objects for the Game Manager")]
    public GameObject player;
    public Transform startPosition;
    public GameObject room3;
    public GameObject falseFloor;
    public EnemiesManager enemiesManager;
    public GameObject lightExitDoorRoom2;
    public FogController fogController;
    public GameObject WeaponsEliminator;
    public MovingPlatform movingPlatformRoom2;
    public AudioSource audioSourceRoom2;
    public Animator animatorDoorFromTwoToThree;
    //Room 3
    public MovingPlatform movingPlatformRoom3;
    public GameObject extraColliderTeleporterRoom3;
    public BeatController beatControllerRoom3;
    //Rom 4
    public GameObject room4;
    //Others
    public GameObject[] weaponDissolveSpheres;
    public GameObject[] handsPlayer;
    public GameObject[] sabersPlayer;
    public LaserSword[] laserSwords;
    public TMP_Text pointsRedCubes;
    public TMP_Text pointsBlueCubes;
    public GameObject teleportColliderFinalRoom3;
    public GameObject finalUIMessage;
    public GameObject canvasUI;


    private void Start()
    {

        if (xRSimpleInteractable_Room0 != null)
        {
            xRSimpleInteractable_Room0.selectEntered.AddListener(OnSelectButtonRoom0);
        }

        if (teleportMainPlatform_Room2 != null)
        {
            teleportMainPlatform_Room2.teleporting.AddListener(OnEventRoom2);
        }

        if (grabInteractablePistols_Room2 != null)
        {
            grabInteractablePistols_Room2.selectEntered.AddListener(OnSelectePistolsRoom2);
        }

        if (teleportMainPlatform_Room3 != null)
        {
            teleportMainPlatform_Room3.teleporting.AddListener(OnEventRoom3);
        }

        if (teleportMainPlatform_Room4 != null)
        {
            teleportMainPlatform_Room4.teleporting.AddListener(OnEventRoom4);
        }

        if(button_Room5 != null)
        {
            button_Room5.onClick.AddListener(OnEventRoom5);
        }

        if(countdown_Room5 != null)
        {
            countdown_Room5.OnCountdownFinished.AddListener(GameOverManager);
        }


        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetSceneReferences(this);
        }
        else
        {
            Debug.Log("GameManager Instance doesn't exist");
        }
    }

    private void OnSelectButtonRoom0(SelectEnterEventArgs args)
    {
        GameManager.Instance.Room0Finished();
    }

    private void OnEventRoom2(TeleportingEventArgs args)
    {
        GameManager.Instance.ActivateRoom2Event();
    }

    private void OnSelectePistolsRoom2(SelectEnterEventArgs args)
    {
        GameManager.Instance.SetWeaponsAsTaken();
    }

    private void OnEventRoom3(TeleportingEventArgs args)
    {
        GameManager.Instance.PlayerIsInRoom3();
    }
    private void OnEventRoom4(TeleportingEventArgs args)
    {
        GameManager.Instance.PlayerIsInRoom4();
    }
    private void OnEventRoom5()
    {
        GameManager.Instance.PlayerIsInRoom5();
    }
    private void GameOverManager()
    {
        GameManager.Instance.GameOver();
    }
}
