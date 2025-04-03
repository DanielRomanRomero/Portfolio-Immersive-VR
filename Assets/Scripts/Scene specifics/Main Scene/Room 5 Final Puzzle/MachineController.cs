using System.Collections;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// Controls a sci-fi machine that is activated when enough batteries are inserted at room 5
/// Plays sounds and triggers animations once all batteries are present.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class MachineController : MonoBehaviour
{
    public UnityEvent OnMachineStarted;

    [SerializeField] private Animator elevatorDoorsAnimator;
    [SerializeField] private AudioClip engineComputerSound_1;
    [SerializeField] private AudioClip engineComputerSound_2;
    [SerializeField] private AudioClip engineComputerSound_3;
    [SerializeField] private AudioClip engineMachineSound;
    [SerializeField] private Material glow_yellow;
    [SerializeField] private MeshRenderer[] batteryRenderers;

    private AudioSource audioSource;
    private int batteryLevel;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void ChangeBattery(int amount)
    {
        batteryLevel += amount;

        if ((batteryLevel >= 3))
        {
            StartMachine();
        }
    }

    private void StartMachine()
    {
        OnMachineStarted?.Invoke();
        StartCoroutine(MachineProcess());
    }

    private IEnumerator OpenElevatorDoors()
    {
        yield return new WaitForSeconds(2f);
        elevatorDoorsAnimator.SetTrigger("ToggleDoor");
        
    }
    private IEnumerator MachineProcess()
    {
        audioSource.PlayOneShot(engineComputerSound_1);
        yield return new WaitForSeconds(0.9f);
        audioSource.PlayOneShot(engineComputerSound_2);
        yield return new WaitForSeconds(0.8f);
        audioSource.PlayOneShot(engineComputerSound_3);
        yield return new WaitForSeconds(3f);
        audioSource.PlayOneShot(engineMachineSound);
        
        foreach (var batteryRenderer in batteryRenderers)
        {
            batteryRenderer.materials[1] = glow_yellow;
        }
        
        yield return new WaitForSeconds(2f);
        
        StartCoroutine(OpenElevatorDoors());
    }

}
