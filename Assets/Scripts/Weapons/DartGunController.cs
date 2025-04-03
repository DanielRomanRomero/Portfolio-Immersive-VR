using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// VR Dart Gun system using a socket interactor. Detects loading and fires with force.
/// Registers a player interaction the first time a dart is shot.
/// </summary>
public class DartGunController : MonoBehaviour
{
    [SerializeField] private XRSocketInteractor socketInteractor;
    [SerializeField] private BoxCollider socketCollider;

    private bool isLoaded = false;
    private bool firstTimeInteractingObject = false;
    private Coroutine resetSocketCoroutine = null;

    public void ShootDart()
    {
        if (isLoaded)
        {
            if(socketInteractor.interactablesSelected.Count > 0)
            {
                // Acces to the object that is in the socket through XR Socket Interactor
                IXRSelectInteractable dartInteractable = socketInteractor.interactablesSelected[0];
                Transform dart = dartInteractable.transform;

                // Get its Ribidbody
                Rigidbody rbDart = dart.gameObject.GetComponent<Rigidbody>();

                // Deselect object which is in socket 
                socketInteractor.interactionManager.SelectCancel(socketInteractor, dartInteractable);

                if(resetSocketCoroutine == null)
                    resetSocketCoroutine = StartCoroutine(ResetSocket());

                // Apply force to the object to shot 
                rbDart.AddForce(transform.forward * 20, ForceMode.Impulse);

                if (!firstTimeInteractingObject)
                {
                    GameManager.Instance.RegisterObjectInteraction();
                    firstTimeInteractingObject = true;
                }

                isLoaded = false;

            }
        }
        else
        {
           // print("No dart loaded");
        }
    }

    private IEnumerator ResetSocket()
    {
        socketCollider.enabled = false;
        yield return new WaitForSeconds(1f);
        socketCollider.enabled = true;

        resetSocketCoroutine = null;

    }


    public void DartLoaded() => isLoaded = true;

}
