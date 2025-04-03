using System.Collections;
using System.Drawing;
using UnityEngine;

/// <summary>
/// Manages environmental fog settings, fading in and out based on the room context.
/// Used to enhance atmosphere, especially in Room 2 and Room 3. (at 4 starts going out for ever)
/// </summary>
public class FogController : MonoBehaviour
{
    [SerializeField] private float fogFadeDuration = 3f; 
    [SerializeField] private float maxFogDensity = 0.2f; 
    [SerializeField] private float minFogDensity = 0f; 
    private Coroutine fogCoroutine;

    public bool isRoom2 = true;

    public string fogColorRed = "#53201F";
    public string fogColorBlue = "#1F2B53";
    private UnityEngine.Color colorRed;
    private UnityEngine.Color colorBlue;

    private void Start()
    {
        RenderSettings.fogDensity = minFogDensity; 
    }

    /// <summary>
    /// Method to warm up fog, so it is not activate at same time of enemies at room 2
    /// </summary>
    public void WarmUpFog()
    {
        if (!RenderSettings.fog)
        {
            RenderSettings.fogDensity = 0.01f;
            RenderSettings.fog = true;
        }
    }

    public void StartFogFadeIn()
    {
        if (fogCoroutine != null) StopCoroutine(fogCoroutine);

        if (isRoom2)
        {
            if (ColorUtility.TryParseHtmlString(fogColorRed, out colorRed))
            {
                RenderSettings.fogColor = colorRed;
            }
           
        }
        else
        {
            if (ColorUtility.TryParseHtmlString(fogColorBlue, out colorBlue))
            {
                RenderSettings.fogColor = colorBlue;
            }
          
        }

        if(!RenderSettings.fog)
        RenderSettings.fog = true;// It makes sure fog is activated before do fade

        minFogDensity = RenderSettings.fogDensity;
        fogCoroutine = StartCoroutine(DoFade(minFogDensity, maxFogDensity));
    }

    public void StartFogFadeOut()
    {
        if (fogCoroutine != null) StopCoroutine(fogCoroutine);

        fogCoroutine = StartCoroutine(DoFade(maxFogDensity,minFogDensity ));
    }


    private IEnumerator DoFade(float startDensity, float finalDensity)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < fogFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            RenderSettings.fogDensity = Mathf.Lerp(startDensity, finalDensity, elapsedTime / fogFadeDuration);
            yield return null;
        }

        RenderSettings.fogDensity = finalDensity; 

        if (finalDensity == minFogDensity)
        {
            RenderSettings.fog = false; 
        }
    }

}
