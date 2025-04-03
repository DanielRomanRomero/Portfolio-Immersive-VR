using ArmnomadsGames;
using System.Collections;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;


/// <summary>
/// Central controller of the game logic. Handles the main flow of the experience using state machines,
/// managing scene transitions, player progression, and triggering events in the right sequence.
/// 
/// It also communicates with auxiliary state machines (e.g., ObjectiveStateMachine) to coordinate goals,
/// while scene-specific components and UnityEvents handle interactions at runtime.
/// </summary>
public class GameManager : MonoBehaviour
{
    // Singleton instance to ensure only one GameManager exists during runtime
    public static GameManager Instance;

    // Main State machine for game states
    private Statemachine gameStateMachine;

    // Reference to the objective tracking (sub)state machine used in the DaniDevRoom scene
    public ObjectiveStateMachine ObjectiveSM { get; private set; }

    // Shortcut accessor to easily reference ObjectiveSM from anywhere via GameManager.Objectives
    public static ObjectiveStateMachine Objectives => Instance.ObjectiveSM;

    // UnityEvent triggered when the main game starts. 
    // Could be used to initialize systems or start background elements.
    public UnityEvent OnMainGameStarted;

    // UnityEvent triggered on Game Over. Typically used to reset systems or return to the main menu.
    public UnityEvent OnGameOver;


    /// <summary>
    /// This prototype is divided into two main gameplay areas:
    /// 
    /// • Dani Dev Room: A warm-up/tutorial scene where the player arrives home and interacts with several objects.
    ///    It uses an ObjectiveStateMachine to track simple goals (e.g., interact, rest).
    /// 
    /// • Main Scene: A sci-fi environment resembling a spaceship or lab. 
    ///    The player progresses through rooms featuring distinct gameplay mechanics (puzzle, shooter, rhythm),
    ///    guided by a robot voice-over and transitions managed via a central state machine.
    /// </summary>

    [Header("Objectives Dev Room")]
    [SerializeField] private GameObject tableHighlight;                          // A highlight for the table (objective 2)
    [SerializeField] private int interactedObjectsCount = 0;                     // A counter for interacted objects (objective 3)
    public int InteractedObjectsCount { get { return interactedObjectsCount; } } // Exposing the counter to check from ObjectiveStateMachine (PlayAroundObjectiveState)
    private const int requiredInteractions = 3;                                  // The amount of interactions we need to acomplish objective 3
    
    // GO of the controller for the last part of Dani Dev Room. See more at FinalRoomActivator Class.
    [SerializeField] private GameObject finalRoomActivator; 

    [Header("Player Main Scene")]
    [SerializeField] private GameObject player;       // Player's GO at Main scene
    [SerializeField] private Transform startPosition; // Start position for player at Main scene


    [Header("Shared for rooms at Main scene")]
    [SerializeField] private FogController fogController;  // Fog controller class (used in room 2 and room 3)
    [SerializeField] private GameObject WeaponsEliminator; // Effect for eliminate current Controllers (uses WeaponsEliminator class)

    [Header("Objectives Main Room 2")]
    [SerializeField] private GameObject falseFloor;                   // False floor used while event 2 (room 2). It provides more aesthetic while fog and enemies are running
    [SerializeField] private EnemiesManager enemiesManager;           // Enemy manager class
    [SerializeField] private GameObject lightExitDoorRoom2;           // Light that highlitgh exit door at the end of event 2
    [SerializeField] private AudioSource audioSourceRoom2;            // Audio source that controls music while event 2
    [SerializeField] private float fadeMusicDuration = 3f;            // Duration for the music fade at event 2
    [SerializeField] private bool roomTwoCompleted = false;           // Controls if event 2 has been complete
    [SerializeField] private GameObject extraColliderTeleporterRoom3; // Extented collider beyond the teleporter surface (exactly is on the entrance door of room 3)
    [SerializeField] private MovingPlatform movingPlatformRoom2;      // Acces to room 2 moving main platform (after completing we indicate it to start moving near to exit door)
    private bool playerHasTakenWeapons = false;                       // Has the player change its hands controllers for pistol controllers?
    private int requiredDeathEnemies;                                 // Required death enemies for completing event/room 2
    private int deathEnemiesCounter;                                  // Enemies death counter
    public Animator animatorDoorFrom2To3;                             // Animator component for opening exit room 2 door
    private bool eventRoom2Completed = false;                         // Event 2 - Room 2 has been completed?
    

    [Header("Objectives Main Room 3")]
    [SerializeField] private GameObject room3;                      // Reference to full Room3 GO, so we can activate it before entering and deactivating after completing (Optimization).
    [SerializeField] private MovingPlatform movingPlatformRoom3;    // Acces to room 3 moving main platform (at entering room3 we indicate it to start moving near room3 -beatSaber- event)
    [SerializeField] private BeatController beatControllerRoom3;    // Acces to BeatController at room 3 => this class controls main event room 3
    [SerializeField] private GameObject[] weaponDissolveSpheres;    // Direct acces to activate Weapon Dissolve Spheres => Used indepently at room3 for change hands controllers to saber controllers (1/each hand)
    [SerializeField] private LaserSword[] laserSwords;              // Debemos acceder para activar/desactivarlos
    [SerializeField] private TMP_Text pointsRedCubesText;           // Acces to red cubes counter Text at event/room 3 (could be managed by Room3State, but it would be redundand due the size of the prototype)
    [SerializeField] private TMP_Text pointsBlueCubesText;          // Acces to blue cubes counter Text at event/room 3 (could be managed by Room3State, but it would be redundand due the size of the prototype)
    [SerializeField] private GameObject teleportColliderFinalRoom3; // Extented collider beyond the teleporter surface (exactly is around all surface of room3 after completing, it teleports your directly at room 4)

    // Private counters for red and blue cubes (could be managed by Room3State, but it would be redundand due the size of the prototype)
    private int redCubesPoints = 0;    
    private int blueCubesPoints = 0;

    // security bool to call room3 event just onces
    private bool playerHasEnterRoom3 = false;  

    [Header("Room 4")]
    [SerializeField] private GameObject room4;          // Reference to full Room4 GO, so we can activate it before entering and deactivating after completing (Optimization).

    [Header("Final Message")]
    [SerializeField] private GameObject canvasUI;       // Reference to activate Canvas before finalUI message
    [SerializeField] private GameObject finalUIMessage; // Reference to activate final game message


    private void Awake()
    {
        /// <summary>
        /// Sets up the singleton instance and prevents this GameObject from being destroyed between scene loads.
        /// </summary>
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); 
    }

    void Start()
    {
        // Create a new instance of the statemachine
        gameStateMachine = new Statemachine();

        // We create another instance of ObjectiveStateMachine, the only one that will exist throughout the entire gameState of DaniDevRoom, and it will be updated through all the objectives.
        ObjectiveSM = new ObjectiveStateMachine(); 
    }



    // Public method to activate the GameObject that manages the final part of the DaniDevRoom scene.
    // It's called from RestOnSofaObjectiveState (the last objective of DaniDevRoom).
    public void ActiveFinalRoomSequence()
    {
        finalRoomActivator.SetActive(true);
    }


    /// <summary>
    /// State tracking and transitions for each major gameplay phase (scene).
    /// These methods are responsible for changing state, executing setup logic for each room,
    /// and triggering the correct gameplay flow in sequence.
    /// </summary>


    // FIRST GAME CINEMATIC

    // Called by the CinematicController once the initial cutscene ends
    public void CreateFirstGameStateMachine()
    {
        // We start the state for the initial scene (DaniDevRoom)
        gameStateMachine.ChangeState(new DaniDevRoomState());

        // Execute the current state
        gameStateMachine.ExecuteStateUpdate();
    }

    //FIRST SCENE GAME 'DaniDevRoom' STUFF

    /// <summary>
    /// Public method to activate the table highlight during objective 2.
    /// Called from the ObjectiveStateMachine.
    /// </summary>
    public void ActivateSecondObjectiveHighlight()
    {
        tableHighlight.SetActive(true);
    }

    /// <summary>
    /// Objective 3 of the first scene: method to be called externally to increase the count of interacted objects.
    /// </summary>
    public void RegisterObjectInteraction()
    {
        if (interactedObjectsCount < requiredInteractions)
        {
            interactedObjectsCount++;
            ObjectiveUI.Instance.UpdateObjectiveText($"Play or interact with 3 objects around you ({interactedObjectsCount}/{requiredInteractions})");

            ObjectiveSM.ExecuteObjective();
        }

    }


    /// <summary>
    /// END OF SCENE 1: DaniDevRoom is completed (GAME STATE 1)
    /// </summary>
    public void DaniDevRoomCompleted()
    {
        // Finish objective state machine and clear references (Garbage Collector should clean it, no longer needed)
        ObjectiveSM.FinishObjectiveMachine();

        // Transition to Room0State, which is responsible for loading the next scene
        gameStateMachine.ChangeState(new Room0State());

        //Execute the current state (It would be the normal thing in a 'full and big game'. In this case, it does nothing, just checks everything is working correctly)
        gameStateMachine.ExecuteStateUpdate();
    }

    // ROOM 0 (New Scene): START OF MAIN SCENE)

    /// <summary>
    /// Method for GameManager to receive and store all necessary references from the MainSceneManager (scene 2).
    /// Called during scene initialization.
    /// </summary>
    /// <param name="sceneManager">It contains all data (currently in scene)</param>
    public void SetSceneReferences(MainSceneManager sceneManager)
    {
        // Store references from the MainSceneManager for later use
        player = sceneManager.player;
        startPosition = sceneManager.startPosition;
        room3 = sceneManager.room3;
        falseFloor = sceneManager.falseFloor;
        enemiesManager = sceneManager.enemiesManager;
        lightExitDoorRoom2 = sceneManager.lightExitDoorRoom2;
        fogController = sceneManager.fogController;
        WeaponsEliminator = sceneManager.WeaponsEliminator;
        movingPlatformRoom2 = sceneManager.movingPlatformRoom2;
        audioSourceRoom2 = sceneManager.audioSourceRoom2;
        animatorDoorFrom2To3 = sceneManager.animatorDoorFromTwoToThree;
        extraColliderTeleporterRoom3 = sceneManager.extraColliderTeleporterRoom3;
        movingPlatformRoom3 = sceneManager.movingPlatformRoom3;
        beatControllerRoom3 = sceneManager.beatControllerRoom3;
        weaponDissolveSpheres = sceneManager.weaponDissolveSpheres;
        laserSwords = sceneManager.laserSwords;
        pointsRedCubesText = sceneManager.pointsRedCubes;
        pointsBlueCubesText = sceneManager.pointsBlueCubes;
        teleportColliderFinalRoom3 = sceneManager.teleportColliderFinalRoom3;
        finalUIMessage = sceneManager.finalUIMessage;
        room4 = sceneManager.room4;
        canvasUI = sceneManager.canvasUI;
    }

    /// <summary>
    /// Initializes player and camera settings for the Main Scene.
    /// Called after entering the scene to ensure proper starting point and visuals.
    /// </summary>
    public void SetMainSceneSettings()
    {
        // Set XR Rig transform
        player.transform.SetPositionAndRotation(startPosition.position, startPosition.rotation);

        // Reposition camera in world space
        player.GetComponent<XROrigin>().MoveCameraToWorldLocation(startPosition.position);
       
        // Disable anti-aliasing for performance
        QualitySettings.antiAliasing = 0;

        // Enable post-processing (soft) on main camera
        Camera mainCamera = Camera.main;
        mainCamera.gameObject.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;

    }

    /// <summary>
    /// Called when the player finishes Room 0 (Main Scene intro). It's called through Unity Event when player press the button
    /// </summary>
    public void Room0Finished()
    { 
        // play 2ond clip of room 0
        RobotSoundManager.Instance.PlayNextClip();

        //Wait to previous clip ends to change state and start new clip (Room 1 clip 1.1)
        StartCoroutine(ChangeToStateRoom1());
    }

    /// <summary>
    /// Coroutine that waits for the voice line to end, then transitions to Room 1.
    /// </summary>
    private IEnumerator ChangeToStateRoom1()
    {
        yield return new WaitForSeconds(5.6f); // Wait for clip duration

        // Changes the state to next room
        gameStateMachine.ChangeState(new Room1State());

        //Execute the current state (It would be the normal thing in a 'full and big game'. In this case, it does nothing, just checks everything is working correctly)
        gameStateMachine.ExecuteStateUpdate();
    }

    // ROOM 1 (PUZZLE)

    /// <summary>
    /// Called when the puzzle in Room 1 is solved. Transitions to Room 2 (shooter section).
    /// </summary>
    public void Room1Finished()
    {
        //When Room0 is finished, change the state to next room
        gameStateMachine.ChangeState(new Room2State());

        //Execute the current state (It would be the normal thing in a 'full and big game'. In this case, it does nothing, just checks everything is working correctly)
        gameStateMachine.ExecuteStateUpdate();
    }

    // ROOM 2 (SHOOTER)

    /// <summary>
    /// Confirms the player has picked up the pistols. Required to activate Room 2 event. Called from Unity Event in scene
    /// </summary>
    public void SetWeaponsAsTaken()
    {
        playerHasTakenWeapons = true;
    }

    /// <summary>
    /// Method to activate the room 2 event. Its called when player teleports to main teleport transform at room 2.      
    /// Triggers fog (which is already warm up), music, and delayed enemy spawning.
    /// </summary>
    public void ActivateRoom2Event()
    {
        if (eventRoom2Completed) return;

        requiredDeathEnemies = enemiesManager.ReturnEnemyPool();    

        if (!roomTwoCompleted && playerHasTakenWeapons)
        {
            fogController.StartFogFadeIn(); 

            Invoke(nameof(ActivateFalseFloorRoom2),4);

            Invoke(nameof(ActivateMusicRoom2), 11);
            
            Invoke(nameof(DelayedEnemyActivation), 12f); // extra delay for enemies activation
        }

    }

    
    private void ActivateFalseFloorRoom2()
    {
        falseFloor.SetActive(true); // Activate false floor => Used for more aestethic porpuses
    }

    private void ActivateMusicRoom2()
    {
        audioSourceRoom2.Play();
    }

    /// <summary>
    /// Coroutine to smoothly fade out the room music.
    /// </summary>
    private IEnumerator FadeOutRoutine()
    {
        float startVolume = audioSourceRoom2.volume;

        while (audioSourceRoom2.volume > 0f)
        {
            audioSourceRoom2.volume -= startVolume * Time.deltaTime / fadeMusicDuration;
            yield return null;
        }

        audioSourceRoom2.volume = 0f;
        audioSourceRoom2.Stop(); 
    }
    /// <summary>
    /// Extra step to delay a bit enemies activation (this is for not activating fog and enemies at same time)
    /// </summary>
    private void DelayedEnemyActivation()
    {
        // Call event to start enemies coming out 
        enemiesManager.ActivateEnemiesEvent();
    }

    /// <summary>
    /// Called when an enemy dies. Updates kill counter and checks for room completion (depends on enemyList.Count).
    /// </summary>
    public void CountDeathEnemiesRoom2()
    {
        if (deathEnemiesCounter < requiredDeathEnemies)
        {
            deathEnemiesCounter++;
        }

        if(deathEnemiesCounter >= requiredDeathEnemies)
        {
            Room2Finished();
            StartCoroutine(FadeOutRoutine());
        }
    }

    /// <summary>
    /// Called when Room 2 is completed (all enemies defeated). Triggers scene changes and prepares Room 3.
    /// </summary>
    public void Room2Finished()
    {
        RobotSoundManager.Instance.PlayNextClip();

        roomTwoCompleted = true;

        fogController.StartFogFadeOut(); // fog fade out

        falseFloor.SetActive(false); // Deactivate false floor

        lightExitDoorRoom2.SetActive(true); // activate exit door light

        //Activate Weapons eliminator above to player
        WeaponsEliminator.SetActive(true);
        WeaponsEliminator.GetComponent<WeaponEliminator>().ActivateWeaponsEliminatorNow(true, false);

        //Activate Room 3
        room3.SetActive(true);

        animatorDoorFrom2To3.SetTrigger("TriggerDoor"); // open the exir door

        //Activate teleport extra collider so player can enter to room 3
        extraColliderTeleporterRoom3.SetActive(true);

        Invoke(nameof(MoveRoom2MainPlatformToExit), 4);

        eventRoom2Completed = true;
        //Debug.Log("Room 2 event completed.");
    }

    /// <summary>
    /// Moves Room 2's main platform toward the exit door.
    /// </summary>
    private void MoveRoom2MainPlatformToExit()
    {
        movingPlatformRoom2.SetStartMoving();
    }

    // ROOM 3 (BEAT SABER-LIKE)

    /// <summary>
    ///  Called when the player enters Room 3. Triggers state change, platform movement, and event preparation.
    /// </summary>
    public void PlayerIsInRoom3()
    {

        if (playerHasEnterRoom3) return;

        //Change state to the new room 3
        gameStateMachine.ChangeState(new Room3State());

        //Execute the current state. (not actually needed but in a full/bigger game would be the normal thing)
        gameStateMachine.ExecuteStateUpdate();

        //Start moving platform and activate room 3 event
        movingPlatformRoom3.SetStartMoving();
        StartCoroutine(ActivateEventRoom3());

        //Debug.Log("Preparing Room 3 event.");
        playerHasEnterRoom3 = true;
    }

    /// <summary>
    /// Coroutine that prepares and activates Room 3 event (Beat Saber style).
    /// Handles fog, weapon switching, saber activation, and music start.
    /// </summary>
    private IEnumerator ActivateEventRoom3()
    {
        yield return new WaitForSeconds(4); // Time for platform movement and voice-over intro
                                            
        fogController.isRoom2 = false;
        fogController.StartFogFadeIn();


        foreach (var spheres in weaponDissolveSpheres)
        {
            spheres.SetActive(true);
            spheres.GetComponent<WeaponDissolveEffect>().StartEffect(false,true);
        }


        yield return new WaitForSeconds(12f);// Delay before saber activation (robot keeps talking)

        foreach (var scriptSaber in laserSwords)
        {
            scriptSaber.Enable();// Enable saber lights
        }
        

        yield return new WaitForSeconds(5);// Wait for final robot voice line

        // Reset counters
        redCubesPoints = 0;
        blueCubesPoints = 0;
        pointsRedCubesText.SetText(redCubesPoints.ToString());
        pointsBlueCubesText.SetText(blueCubesPoints.ToString());

        //Call beat Controller to start the beat-saber like event
        beatControllerRoom3.StartEventRoom3();

        //Debug.Log("Room 3 event started.");
    }


    /// <summary>
    /// Adds a point to the red cube counter and updates the UI.
    /// </summary>
    public void Room3RedCubesCounter()
    {
        redCubesPoints++;
        pointsRedCubesText.SetText(redCubesPoints.ToString());
    }

    /// <summary>
    /// Adds a point to the blue cube counter and updates the UI.
    /// </summary>
    public void Room3BlueCubesCounter()
    {
        blueCubesPoints++;
        pointsBlueCubesText.SetText(blueCubesPoints.ToString());
    }



    /// <summary>
    /// Called by the BeatController when the music ends. Checks if enough cubes were sliced.
    /// </summary>
    public void CheckRoom3Counters()
    {
        if(redCubesPoints >= 10 && blueCubesPoints >= 10)
        {
            Room3Finished();//Succes
        }
        else 
        {
            // This project is for portfolio use only – no retry logic needed.
            // Land me a job and I'll complete this function :)

            Room3Finished();// Fails also move forward
        }
    }


    /// <summary>
    /// Handles the cleanup and transition after Room 3 is completed.
    /// </summary>
    public void Room3Finished()
    {
        room4.SetActive(true); //Activate next Room content

        foreach (var scriptSaber in laserSwords)// deactivate sabers
        {
            scriptSaber.Disable();// Turn off sabers
        }

        StartCoroutine(SetUpFinalRoom3()); // Start hand reset
    }

    /// <summary>
    /// Coroutine to reset hands while robot is talking
    /// </summary>
    private IEnumerator SetUpFinalRoom3()
    {
        yield return new WaitForSeconds(2f);

        //Activate Weapons eliminator above to player
        WeaponsEliminator.SetActive(true);
        WeaponsEliminator.GetComponent<WeaponEliminator>().ActivateWeaponsEliminatorNow(false, false);

        // activate alternative collider final room 3 to be able to reach room 4
        teleportColliderFinalRoom3.SetActive(true);
    }


    /// <summary>
    /// Called when the player teleports into Room 4. Triggers state change.
    /// </summary>
    public void PlayerIsInRoom4()
    {
        fogController.StartFogFadeOut();

        gameStateMachine.ChangeState(new Room4State());
        gameStateMachine.ExecuteStateUpdate();
    }


    /// <summary>
    /// Method called when we enter at room 5 since we dont need to do anything special here. It's just for keep the full structure and preserve flow consistency
    /// </summary>
    public void Room4Finished()
    {
        //When we press all buttons in room, the last button opens the door and play next robot sound => Then when we teleportate to room 5, playerIsInRoom5 starts, so, we dont need to do nothing special here
    }

    /// <summary>
    /// Called when the player teleports into Room 5.
    /// </summary>
    public void PlayerIsInRoom5()
    {
        Room4Finished();
       
        gameStateMachine.ChangeState(new Room5State());

        //Execute the current state (It would be the normal thing in a 'full and big game'. In this case, it does nothing, just checks everything is working correctly)
        gameStateMachine.ExecuteStateUpdate();
    }

    /// <summary>
    /// Called after Room 5 is completed. Starts final sequence.
    /// </summary>
    public void Room5Finished()
    {
        StartCoroutine(EndsTheGameAfterAWhile());
    }

    /// <summary>
    /// Coroutine to delay final message activation and end the game after robot finishes speaking.
    /// </summary>
    private IEnumerator EndsTheGameAfterAWhile()
    {
        RobotSoundManager.Instance.PlayNextClip();

        yield return new WaitForSeconds(26f); // Last speech duration

        gameStateMachine.ExitStateMachine(); // Ends gameStateMachine

        // Activate Canvas and special final message
        canvasUI.SetActive(true);
        finalUIMessage.SetActive(true);

        //Debug.Log("Game officially ended.");

    }


    /// <summary>
    /// Global Game Over method. Called when the player fails or chooses to end.
    /// Since this is for portfolio porpuses, nothing is set up in scenes 
    /// </summary>
    public void GameOver()
    {
        Debug.Log("Game Over!");

        OnGameOver?.Invoke();
        SceneManager.LoadScene(0); // Reload initial scene
    }


}
