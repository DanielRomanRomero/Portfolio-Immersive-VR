
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// Handles a 3-digit numeric lock puzzle. Players can increment or decrement each digit, and the lock checks for the correct combination.
/// </summary>
public class LockController : MonoBehaviour
{
    // Called when the correct password is entered
    public UnityEvent OnPasswordUnlocked;

    public List<TMP_Text> numbersText = new List<TMP_Text>();
    public List<int> password = new List<int>();
    private List<int> currentCombination = new List<int>() { 0, 0, 0 };

    [SerializeField] private float numberSpeed;
    private AudioSource audioSource;
    [SerializeField] private AudioClip gazeLockSound;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void StartAdding(int index)
    {
        StartCoroutine(StartCountingRoutine(index, 1));
    }

    public void StartSubstracting(int index) 
    {
        StartCoroutine(StartCountingRoutine(index, -1));
    }

    public void StopCounting()
    {
        StopAllCoroutines();
        //print("Stop Counting");
    }


    private IEnumerator StartCountingRoutine(int index, int amount)
    {
        while (true)
        {
            //print("Start Counting");
            yield return new WaitForSeconds(numberSpeed);
           
            int newNumber = amount + int.Parse(numbersText[index].text);

            newNumber = newNumber < 0 ? 9 : newNumber > 9 ? 0 : newNumber;

            numbersText[index].text = newNumber.ToString();
            currentCombination[index] = newNumber;
            audioSource.PlayOneShot(gazeLockSound);


            if (CheckIfValidPassword(currentCombination))
                OnPasswordUnlocked.Invoke();
        }
    }


    private bool CheckIfValidPassword(List<int> currentCombination)
    {
        for (int i = 0; i < currentCombination.Count; i++)
        {
            if (currentCombination[i] != password[i])
            {
                return false;
            }
        }

        return true;
    }
}
