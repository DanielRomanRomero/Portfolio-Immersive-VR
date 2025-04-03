using System.Collections;
using UnityEngine;

/// <summary>
/// Plays a kind of dissolve effect on a visual sphere and switches the player's weapon set (hands and sabers/pistols).
/// </summary>
public class WeaponDissolveEffect : MonoBehaviour
{
    [SerializeField] private Renderer sphereRenderer; 
    [SerializeField] private float tilingIncreaseDuration = 1f; 
    [SerializeField] private float fadeOutDuration = 1f; 
    [SerializeField] private Material transparentMaterial;
    [SerializeField] private GameObject playerLocomotion;

    [Header("Controllers")]
    [SerializeField] private GameObject[] handPlayer;
    [SerializeField] private GameObject[] pistolsPlayer;
    [SerializeField] private GameObject[] laserSabersPlayer;

    [Header("Custom")]
    public bool arePistols = true;

    private Material instanceMaterial;


    /// <summary>
    /// Entry point to start the dissolve transition and swap weapons.
    /// </summary>
    public void StartEffect(bool arePistols, bool isForActivate)
    {
        instanceMaterial = new Material(transparentMaterial);
        sphereRenderer.material = instanceMaterial;
        StartCoroutine(DissolveEffectCoroutine(arePistols,isForActivate));
    }


    private IEnumerator DissolveEffectCoroutine(bool arePistols, bool isForActivate)
    {
        float elapsedTime = 0f;
        Vector2 tiling = instanceMaterial.mainTextureScale;
        Color materialColor = instanceMaterial.color;
        materialColor.a = 1;

        // Increase texture tiling (stretch visual)
        while (elapsedTime < tilingIncreaseDuration)
        {
            elapsedTime += Time.deltaTime;
            tiling.y = Mathf.Lerp(1, 6, elapsedTime / tilingIncreaseDuration);
            instanceMaterial.mainTextureScale = tiling;
            yield return null;
        }

        // Swap weapons
        if (arePistols)
        {
            foreach (GameObject pistols in pistolsPlayer)
            {
                pistols.SetActive(false);
            }

            foreach (GameObject hands in handPlayer)
            {
                hands.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject hands in handPlayer)
                hands.SetActive(!isForActivate);

            foreach (GameObject sabers in laserSabersPlayer)
                sabers.SetActive(isForActivate);
        }

        yield return new WaitForSeconds(0.1f); 

        // Fade out dissolve sphere
        elapsedTime = 0f;

        // Reduces opacity untill is transparent
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            materialColor.a = Mathf.Lerp(1, 0, elapsedTime / fadeOutDuration);
            instanceMaterial.color = materialColor;
            yield return null;
        }

        // Re-enable locomotion and hide this object
        if (!playerLocomotion.activeInHierarchy)
            playerLocomotion.SetActive(true);

        gameObject.SetActive(false);
    }
}
