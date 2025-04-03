using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// Controls light intensity and material change based on slider interaction.
/// Triggers the objective progression once light level is high enough.
/// </summary>
public class XRSliderController : MonoBehaviour
{
    [SerializeField] private XRSlider slider;
    [SerializeField] private Light lightOne;
    [SerializeField] private MeshRenderer[] meshRenderers;
    [SerializeField] private Material normalMaterial;
    public UnityEvent OnLightsPerformed;


    private float value;
    private bool materialChanged = false;
    private bool lightsAreOn = false;

    private void Start()
    {
       if(slider == null) slider = GetComponent<XRSlider>();
       value = slider.Value;
       lightOne.intensity = value;
    }

    private void Update()
    {
        if(slider == null || lightsAreOn)
            return;

        if (slider.Value != value)
        {
            value = slider.Value;

            lightOne.intensity = value * 0.8f;
        }

        if(value > 0.3f && !materialChanged)
        {
            foreach (var meshRenderer in meshRenderers)
            {
                meshRenderer.material = normalMaterial;
            }

            OnLightsPerformed.Invoke();
            materialChanged = true;
            lightsAreOn = true;
        }
       
    }

    public void ObjectiveOneAcomplish() 
    {
        GameManager.Objectives.ChangeObjective(new NotebookObjectiveState(GameManager.Objectives));
    }

}
