using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

/// <summary>
/// State that manages Room 0 (intro of the Main scene).
/// Responsible for loading the scene, initializing settings, and playing the first robot voice line.
/// 
/// This state is part of a modular State Machine architecture used throughout the prototype.
/// Even though the project is small, this pattern was intentionally used to demonstrate solid and scalable architecture practices.
/// </summary>
public class Room0State : IState
{
    // The name of the state
    private string stateName;

    public Room0State()
    {
        // Set the state name to the name of the class
        stateName = typeof(Room0State).Name;
    }


    public void EnterState()
    {
        Debug.LogFormat("Entering {0} ", stateName);

        // Start coroutine from GameManager to load the Main scene
        GameManager.Instance.StartCoroutine(LoadSceneAndInitialize());
    }


    private IEnumerator LoadSceneAndInitialize()
    {
        // Load scene with async operation
        var asyncOperation = SceneManager.LoadSceneAsync("Main");

        //dont move untill scene is loaded (assets & scripts)
        while (!asyncOperation.isDone)
            yield return null;

        // Set up audio for Room 0
        RobotSoundManager.Instance.SetRoom(0);
        RobotSoundManager.Instance.PlayNextClip(3);

        // Enable post-processing and XR setup
        Camera mainCamera = Camera.main;
        mainCamera.gameObject.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
        
        GameManager.Instance.SetMainSceneSettings();
    }


    public void ExecuteState()
    {
        Debug.LogFormat("Executing {0} ", stateName);
    }
    public void ExitState()
    {
        Debug.LogFormat("Exiting {0} ", stateName);
    }   

}
