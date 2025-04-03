using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VRGazeInteraction
{
    public enum GazeState { Inactive, Observed, Activated }

    /// <summary>
    /// Core class for GazeInteraction. This class shoots the ray and invokes the 
    /// Event on the GazeEvents class which has to be attached ob interactable objects.
    /// </summary>
    public class GazeManager : MonoBehaviour
    {
        [Tooltip("What layer do you want your objects to be interactable")]
        public LayerMask layer;
        [Tooltip("Mostly for debug purposes: Do you want to show/hide the gaze?")]
        public bool showRay = true;
        [Tooltip("For observing what gameObject is currently gazed at")]
        [SerializeField] private GameObject currentGazedObject = null;
        [SerializeField] private GameObject previousGazedObject = null;
        private float timer;
        private bool isGazeObject;
       


        void Update()
        {

            RaycastHit hit;
            bool hasHit = Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, layer);

            if (showRay)
                Debug.DrawRay(transform.position, transform.forward * 1000f, hasHit ? Color.green : Color.white);

            if (hasHit)
            {
                GameObject hitObject = hit.transform.gameObject;

                if (hitObject != currentGazedObject)
                {
                    // Llamamos al OnGazeLeft del anterior si es diferente
                    if (previousGazedObject != null && previousGazedObject != hitObject)
                        CallOnGazeLeft(previousGazedObject);

                    previousGazedObject = hitObject;
                    currentGazedObject = hitObject;
                    isGazeObject = true;
                }
            }
            else
            {
                if (previousGazedObject != null)
                {
                    CallOnGazeLeft(previousGazedObject);
                    previousGazedObject = null;
                }

                currentGazedObject = null;
                isGazeObject = false;
            }

            if (isGazeObject)
                ActivateGazeInteraction();


            /*ShootRayCast();

            if (isGazeObject)
                ActivateGazeInteraction();
            else
                DeactivateGazeInteraction();*/
        }

        /// <summary>
        /// The ray cast which get's shot every frame that represents the gaze
        /// </summary>
        public void ShootRayCast()
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit))
            {

                if (((1 << hit.transform.gameObject.layer) & layer) != 0)  // If both the layers are matching
                {
                    currentGazedObject = hit.transform.gameObject;
                    isGazeObject = true;
                }                   
                else
                {
                    isGazeObject = false;
                    currentGazedObject = null;
                }
               
            }
            else // Just incase it does not hit any wall or if there is gap in between walls
            {
                if (showRay)
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);

                if (currentGazedObject == null) return;
                
                StopAllCoroutines();
                Reset();
            }
        }

        private void ActivateGazeInteraction()
        {
            if (!HasGazeEventsComponent()) return;

            var events = currentGazedObject.GetComponent<GazeEvents>();

            if (events.gazeState == GazeState.Inactive)
            {
                StopAllCoroutines();
                StartCoroutine(GazeActivationRoutine(events));
            }
            /* if (showRay)
                 Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.green);

             if (!HasGazeEventsComponent()) return;

             if (currentGazedObject.GetComponent<GazeEvents>().gazeState == GazeState.Inactive)
             {
                 StopAllCoroutines();
                 StartCoroutine(GazeActivationRoutine());
             }
            */
        }

        private void DeactivateGazeInteraction()
        {
            if (showRay)
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);

            if (currentGazedObject == null) return;

            StopAllCoroutines();
            Reset();

        }

        /// <summary>
        /// The routine that get's called when you gaze at a inactive interactable object.
        /// Immediately invokes OnGazeEnter and after activationTime is over invokes OnGazeActivated
        /// </summary>
        /// <returns></returns>
        /// 
        /*  public IEnumerator GazeActivationRoutine()
          {


               timer = 0;
               currentGazedObject.GetComponent<GazeEvents>().gazeState = GazeState.Observed;
               currentGazedObject.GetComponent<GazeEvents>().OnGazeEnter.Invoke(currentGazedObject);

               while (timer <= currentGazedObject.GetComponent<GazeEvents>().activationTime)
               {
                   timer += Time.deltaTime;
                   yield return null;
               }

               currentGazedObject.GetComponent<GazeEvents>().OnGazeActivated.Invoke(currentGazedObject);
               currentGazedObject.GetComponent<GazeEvents>().gazeState = GazeState.Activated;
          }*/

        private IEnumerator GazeActivationRoutine(GazeEvents events)
        {
            timer = 0f;
            events.gazeState = GazeState.Observed;
            events.OnGazeEnter.Invoke(currentGazedObject);

            while (timer <= events.activationTime)
            {
                if (currentGazedObject == null || !currentGazedObject.activeInHierarchy)
                    yield break;

                timer += Time.deltaTime;
                yield return null;
            }

            events.OnGazeActivated.Invoke(currentGazedObject);
            events.gazeState = GazeState.Activated;
        }



        /// <summary>
        /// Get's called when you look at a new interactable object or when you look away from an active object
        /// Invokes OnGazeLeft
        /// </summary>
        public void Reset()
        {
            timer = 0;
            currentGazedObject.GetComponent<GazeEvents>().OnGazeLeft.Invoke(currentGazedObject);
            currentGazedObject.GetComponent<GazeEvents>().gazeState = GazeState.Inactive;
            
            currentGazedObject = null;
            /*  if (currentGazedObject != null)
              {
                  Debug.Log("Reset Gaze: " + currentGazedObject.name);
                  currentGazedObject.GetComponent<GazeEvents>().gazeState = GazeState.Inactive;
                  currentGazedObject.GetComponent<GazeEvents>().OnGazeLeft.Invoke(currentGazedObject);
              }
              currentGazedObject = null;*/
        }


     /*   bool HasGazeEventsComponent()
        {
            if (currentGazedObject == null)
            {
                Debug.LogWarning("GazeManager: currentGazedObject is null");
                return false;
            }

            if (!currentGazedObject.TryGetComponent(out GazeEvents events))
            {
                Debug.LogError("GazeManager: Missing GazeEvents component on " + currentGazedObject.name);
                currentGazedObject = null;
                return false;
            }

            return true;
        }
     */


        /// <summary>
        /// Errorhandling: When user forgets to add GazeEventsComponent to interactable object then he'll get notified. 
        /// </summary>
        /// <returns></returns>
        bool HasGazeEventsComponent()
        {
            if (currentGazedObject == null)
                return false;

            if (!currentGazedObject.TryGetComponent(out GazeEvents _))
            {
                currentGazedObject = null;
                return false;
            }

            return true;
            /*  if (currentGazedObject.GetComponent<GazeEvents>() == null)
              {
                  Debug.LogError("Jo Dude, you need to add the GazeEvents component if you want to make it gaze interactable");
                  currentGazedObject = null;
                  return false;
              }
              return true;*/
        }


        private void CallOnGazeLeft(GameObject obj)
        {
            if (obj == null) return;

            if (obj.TryGetComponent(out GazeEvents events))
            {
                if (events.gazeState != GazeState.Inactive)
                {
                    events.gazeState = GazeState.Inactive;
                    events.OnGazeLeft.Invoke(obj);
                }
            }
        }

    }
}