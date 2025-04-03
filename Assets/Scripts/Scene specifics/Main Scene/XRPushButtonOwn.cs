using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
/// <summary>
/// Custom physical push button compatible with any orientation.
/// Calculates local push depth and triggers an event when pressed.
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class XRPushButtonOwn : XRBaseInteractable
{
    public UnityEvent OnPushed;

    [SerializeField] private float minimalPushDepth;
    [SerializeField] private float maximunPushDepth;

    private XRBaseInteractor pushInteractor = null;
    private bool previouslyPushed = false;
    private float oldPushPosition;

    protected override void OnEnable()
    {
        base.OnEnable();
        hoverEntered.AddListener(StartPush);
        hoverExited.AddListener(EndPush);
    }


    protected override void OnDisable()
    {
        base.OnDisable();
        hoverEntered.RemoveListener(StartPush);
        hoverExited.RemoveListener(EndPush);
    }

    private void Start()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();

        minimalPushDepth = transform.localPosition.y;
        maximunPushDepth = transform.localPosition.y - (boxCollider.bounds.size.y * 0.55f); // 0.55 is a magic number that works for me xd, you may need to adjust it.
    }

    private void StartPush(HoverEnterEventArgs arg0)
    {
     
        pushInteractor = (XRBaseInteractor)arg0.interactorObject;
        oldPushPosition = GetLocalYPosition(arg0.interactorObject.transform.position);
    }

    private void EndPush(HoverExitEventArgs arg0)
    {
        pushInteractor = null;
        oldPushPosition = 0.0f;
        previouslyPushed = false;
        SetYPosition(minimalPushDepth);
    }
    
    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        if(pushInteractor != null)
        {
            float newPushPosition = GetLocalYPosition(pushInteractor.transform.position);
            float pushDifference = oldPushPosition - newPushPosition;
            oldPushPosition = newPushPosition;

            float newPosition = transform.localPosition.y - pushDifference;
            SetYPosition(newPosition);

            CheckPress();
        }
    }

    /// <summary>
    /// Get the local instead of the world position  so we can use this button in any orientation insitead of just vertical.
    /// </summary>
    /// <param name="interactorPosition"></param>
    /// <returns></returns>
    private float GetLocalYPosition(Vector3 interactorPosition)
    {
        return transform.root.InverseTransformDirection(interactorPosition).y;
    }

    /// <summary>
    /// Sets the y position of the button without going over (below or above) the limits.
    /// </summary>
    /// <param name="yPosition"></param>
    private void SetYPosition(float yPosition)
    {
         Vector3 newPosition = transform.localPosition;
         newPosition.y = Mathf.Clamp(yPosition,maximunPushDepth,minimalPushDepth);

         transform.localPosition = newPosition;
    }
    /// <summary>
    /// Checks of the button is pushed inside a specific range
    /// </summary>
    private void CheckPress()
    {
        float inRange =Mathf.Clamp(transform.localPosition.y, maximunPushDepth, minimalPushDepth + 0.01f);
        bool isPushedDown = transform.localPosition.y == inRange; 

        if(isPushedDown && !previouslyPushed)
            OnPushed?.Invoke();

        previouslyPushed = isPushedDown;
    }

}
