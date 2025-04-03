using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// Changes glove material and enables teleportation interaction layer.
/// Used to visually and functionally switch gloves into teleport mode.
/// </summary>
public class TeleportGloves : MonoBehaviour
{
    [SerializeField] private Material newHandMaterial;
    [SerializeField] private SkinnedMeshRenderer[] gloveSkins;
    [SerializeField] private XRRayInteractor rightRay;
    [SerializeField] private UnityEngine.XR.Interaction.Toolkit.InteractionLayerMask teleportMask;

    public void SwapOutMaterial()
    {
       foreach (var gloveSkin in gloveSkins)
       {
           gloveSkin.material = newHandMaterial;
       }
    }

    public void SetInteractionLayerMask()
    {
       // activate line visuals for teleport for right hand
       rightRay.enabled = true;
       rightRay.interactionLayers = teleportMask;//Allows the right hand to teleport
    }
   
}
