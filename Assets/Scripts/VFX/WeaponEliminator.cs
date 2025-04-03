using System.Collections;
using UnityEngine;

/// <summary>
/// Floating object that descends above the player and triggers the dissolve spheres.
/// Used to visually transition between controller sets (hands/pistols/sabers).
/// </summary>
public class WeaponEliminator : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform playerPosition;
    [SerializeField] private float offsetEliminator = 4f; 

    [SerializeField] private GameObject[] spheresEliminators;
    [SerializeField] private GameObject[] handPlayer;
    [SerializeField] private GameObject[] pistolsPlayer;

    [SerializeField] private float timeMoving = 3f;

    private bool isActive = false;
    private bool arePistols;
    private bool isForActivate;

    /// <summary>
    /// Called by GameManager to initiate the effect.
    /// </summary>
    public void ActivateWeaponsEliminatorNow(bool arePistols, bool isForActivate)
    {
        isActive = false;
        timeMoving = 3f;
        Vector3 offset = new Vector3(0,offsetEliminator,-0.5f);
        transform.position = playerPosition.position + offset;
        this.arePistols = arePistols;
        this.isForActivate = isForActivate;

        isActive = true;

    }

    private void Update()
    {
        if (isActive)
        {
            if (timeMoving > 0)
            {
                timeMoving -= Time.deltaTime;
                transform.position += Vector3.down * 0.8f * Time.deltaTime;
            }
            else
            {
                isActive = false;
                this.gameObject.SetActive(false);
            }
        }
    }


    private IEnumerator EliminateWeapons()
    {
        foreach (GameObject spheres in spheresEliminators)
        {
            spheres.SetActive(true);

            spheres.GetComponent<WeaponDissolveEffect>().StartEffect(arePistols, isForActivate);
        }

        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            StartCoroutine(EliminateWeapons());
    }

}
