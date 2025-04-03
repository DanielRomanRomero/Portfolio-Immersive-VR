using UnityEngine;

/// <summary>
/// Simple script to change hand gloves controllers for pistols at room2
/// Its activated trhough XR Interact event
/// </summary>
public class PistolsHands : MonoBehaviour
{

    //Ocultar manos del player
    [SerializeField] private GameObject[] handsPlayer;
    //Activar pistolas en player
    [SerializeField] private GameObject[] pistolsPlayer;


    public void ChangesHandsForPistols()
    {
        foreach (GameObject hands in handsPlayer)
        {
            hands.SetActive(false);
        }

        foreach (GameObject pistols in pistolsPlayer)
        {
            pistols.SetActive(true);
        }

    }

}
