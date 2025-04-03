using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;

[RequireComponent(typeof (Animator))]
public class HandAnimatorOld : MonoBehaviour
{

    #region InputActions performed
    /// <summary>
    /// Parameters for animation on action performed 
    /// </summary>
    //[SerializeField] private InputActionReference controllerActionGrip; //Select
    //[SerializeField] private InputActionReference controllerActionTrigger; //Activate
    //[SerializeField] private InputActionReference controllerActionPrimary; //Telport or Thumbstick
    //[SerializeField] private InputActionReference controllerActionFist; //Fist
    #endregion

    /// <summary>
    /// Parameters for realtime animation
    /// </summary>
    [SerializeField] private XRInputValueReader<Vector2> m_StickInput = new XRInputValueReader<Vector2>("Thumbstick"); //Thumbstick
    [SerializeField] private XRInputValueReader<float> m_TriggerInput = new XRInputValueReader<float>("Trigger"); //Activate Value
    [SerializeField] private XRInputValueReader<float> m_GripInput = new XRInputValueReader<float>("Grip");//Select Value
    //[SerializeField] private XRInputValueReader<float> m_FistInput = new XRInputValueReader<float>("Fist"); //Fist

    [SerializeField] private XRPokeInteractor xRPokeInteractor;
    private bool isUIAnimationPlaying = false;
    private Animator handAnimator = null;

    /// <summary>
    /// List of fingers animated when grabbing / using grab action
    /// </summary>
    private readonly List<Finger> grippingFingers = new List<Finger>()
    {
        new Finger(FingerType.Middle),
        new Finger(FingerType.Ring),
        new Finger(FingerType.Pinky)
    };

    /// <summary>
    /// List of fingers animated when pointing / using trigger action
    /// </summary>
    private readonly List<Finger> pointingFingers = new List<Finger>()
    {
        new Finger(FingerType.Index)
    };

    /// <summary>
    /// List of fingers animated when using ui
    /// </summary>
    private readonly List<Finger> uiFingers = new List<Finger>()
    {
        new Finger(FingerType.Thumb),
        new Finger(FingerType.Middle),
        new Finger(FingerType.Ring),
        new Finger(FingerType.Pinky)
    };

    /// <summary>
    /// List of fingers animated when primaryButtonUsed / using primary action
    /// </summary>
    private readonly List<Finger> primaryFingers = new List<Finger>()
    {
        new Finger(FingerType.Thumb)
    };

    /// <summary>
    /// List of fingers animated when grabbing objects
    /// </summary>
    private readonly List<Finger> grabFingers = new List<Finger>()
    {   new Finger(FingerType.Index),
        new Finger(FingerType.Thumb),
        new Finger(FingerType.Middle),
        new Finger(FingerType.Ring),
        new Finger(FingerType.Pinky)
    };

 
    /// <summary>
    /// Add your own hand animation here. For example a fist or a peace sign.
    /// </summary>
    //private readonly List<Finger> Fingers = new List<Finger>()
    //{
    //    new Finger(FingerType.Index)
    //};

    #region InputActions performed suscriptions
    /*  private void OnEnable()
      {
          // Have it run your code when the Action is triggered.
          controllerActionGrip.action.performed += GripAction_performed;
          controllerActionTrigger.action.performed += TriggerAction_performed;
          controllerActionPrimary.action.performed += PrimaryAction_performed;
          controllerActionFist.action.performed += FistAction_performed;

          controllerActionGrip.action.canceled += GripAction_canceled;
          controllerActionTrigger.action.canceled += TriggerAction_canceled;
          controllerActionPrimary.action.canceled += PrimaryAction_canceled;
          controllerActionFist.action.canceled += FistAction_canceled;


          CloseToUICollision.OnEnterUIArea += CloseToUICollision_OnEnterUIArea;
          CloseToUICollision.OnExitUIArea += CloseToUICollision_OnExitUIArea;

          selfAwarnenessHandName = gameObject.name.Replace("HandModel(Clone)", "");
      }



      private void OnDisable()
      {
          controllerActionGrip.action.performed -= GripAction_performed;
          controllerActionTrigger.action.performed -= TriggerAction_performed;
          controllerActionPrimary.action.performed -= PrimaryAction_performed;
          controllerActionFist.action.performed -= FistAction_performed;

          controllerActionGrip.action.canceled -= GripAction_canceled;
          controllerActionTrigger.action.canceled -= TriggerAction_canceled;
          controllerActionPrimary.action.performed -= PrimaryAction_canceled;
          controllerActionFist.action.canceled -= FistAction_canceled;
          CloseToUICollision.OnEnterUIArea -= CloseToUICollision_OnEnterUIArea;
          CloseToUICollision.OnExitUIArea -= CloseToUICollision_OnExitUIArea;
      }*/
    #endregion


    private void OnEnable()
    {
        xRPokeInteractor.uiHoverEntered.AddListener(ActivateUIHandPose);
        xRPokeInteractor.uiHoverExited.AddListener(DeactivateUIHandPose);
    }
  

    private void ActivateUIHandPose(UIHoverEventArgs arg0)
    {
        isUIAnimationPlaying = true;
        SetFingerAnimationValues(uiFingers, 1.0f);
        AnimateActionInput(uiFingers);
    }

    private void DeactivateUIHandPose(UIHoverEventArgs arg0)
    {
        isUIAnimationPlaying = true;
        SetFingerAnimationValues(uiFingers, 0.0f);
        AnimateActionInput(uiFingers);
    }



    private void Start()
    {
        this.handAnimator = GetComponent<Animator>();
    }



    private void Update()
    {

        if (isUIAnimationPlaying )
            return;

        if (m_StickInput != null)
        {
            var stickVal = m_StickInput.ReadValue();
            SetFingerAnimationValues(primaryFingers, stickVal.y);
            AnimateActionInput(primaryFingers);
        }

        if (m_TriggerInput != null)
        {
            var triggerVal = m_TriggerInput.ReadValue();
            SetFingerAnimationValues(pointingFingers, triggerVal);
            AnimateActionInput(pointingFingers);
        }

        if (m_GripInput != null)
        {
            var gripVal = m_GripInput.ReadValue();
            SetFingerAnimationValues(grippingFingers, gripVal);
            AnimateActionInput(grippingFingers);
        }

    }

    

    #region InputActions performed methods
    /* private void GripAction_performed(InputAction.CallbackContext obj)
     {
         if (!uiAnimationRunning)
         {
             SetFingerAnimationValues(grippingFingers, 1.0f);
             AnimateActionInput(grippingFingers);
             //SmoothFingerAnimation(grippingFingers);
         }
     }
     private void TriggerAction_performed(InputAction.CallbackContext obj)
     {
         if (!uiAnimationRunning)
         {
             SetFingerAnimationValues(pointingFingers, 1.0f);
             AnimateActionInput(pointingFingers);
             //SmoothFingerAnimation(pointingFingers);
         }
     }
     private void PrimaryAction_performed(InputAction.CallbackContext obj)
     {
         if (!uiAnimationRunning)
         {
             SetFingerAnimationValues(primaryFingers, 1.0f);
             AnimateActionInput(primaryFingers);
             //SmoothFingerAnimation(primaryFingers);
         }
     }
     private void FistAction_performed(InputAction.CallbackContext context)
     {
         if (!uiAnimationRunning)
         {
             SetFingerAnimationValues(fistFingers, 1.0f);
             AnimateActionInput(fistFingers);
             //SmoothFingerAnimation(primaryFingers);
         }
     }
     private void GripAction_canceled(InputAction.CallbackContext obj)
     {
         if (!uiAnimationRunning)
         {
             SetFingerAnimationValues(grippingFingers, 0.0f);
             AnimateActionInput(grippingFingers);
             //SmoothFingerAnimation(primaryFingers);
         }
     }
     private void TriggerAction_canceled(InputAction.CallbackContext obj)
     {
         if (!uiAnimationRunning)
         {
             SetFingerAnimationValues(pointingFingers, 0.0f);
             AnimateActionInput(pointingFingers);
             //SmoothFingerAnimation(primaryFingers);
         }
     }
     private void PrimaryAction_canceled(InputAction.CallbackContext obj)
     {
         if (!uiAnimationRunning)
         {
             SetFingerAnimationValues(primaryFingers, 0.0f);
             AnimateActionInput(primaryFingers);
             //SmoothFingerAnimation(primaryFingers);
         }
     }
     private void FistAction_canceled(InputAction.CallbackContext context)
     {
         if (!uiAnimationRunning)
         {
             SetFingerAnimationValues(fistFingers, 0.0f);
             AnimateActionInput(fistFingers);
             //SmoothFingerAnimation(primaryFingers);
         }
     }
    */
    #endregion


    #region UI Interaction Old
 /*   private void CloseToUICollision_OnExitUIArea(GameObject triggeringHand)
    {
        isUIAnimationPlaying = false;
        if (!triggeringHand.name.Contains(selfAwarnenessHandName))
        {
            return;
        }
        PerformExitUiAnimation();
    }
    private void CloseToUICollision_OnEnterUIArea(GameObject triggeringHand)
    {
        isUIAnimationPlaying = true;
        if (!triggeringHand.name.Contains(selfAwarnenessHandName))
        {
            return;
        }
        PerformEnterUiAnimation();
    }
 */
  /*  public void PerformEnterUiAnimation()
    {
        AnimateActionInput(uiFingers);
        SetFingerAnimationValues(uiFingers, 1.0f);
        //SmoothFingerAnimation(uiFingers);
    }
    public void PerformExitUiAnimation()
    {
        AnimateActionInput(uiFingers);
        SetFingerAnimationValues(uiFingers, 0.0f);
        
        //SmoothFingerAnimation(uiFingers);
    }*/
    #endregion

    /// <summary>
    /// Set the target value for the fingers to animate
    /// </summary>
    /// <param name="fingersToAnimate"></param>
    /// <param name="targetValue"></param>
    private void SetFingerAnimationValues(List<Finger> fingersToAnimate, float targetValue)
    {
        foreach (Finger finger in fingersToAnimate)
        {
            finger.target = targetValue;
        }
    }

    /// <summary>
    /// Animate the fingers based on the target value
    /// </summary>
    /// <param name="fingersToAnimate"></param>
    private void AnimateActionInput(List<Finger> fingersToAnimate)
    {
        foreach (Finger finger in fingersToAnimate)
        {
            var fingerName = finger.type.ToString();
            var animationBlendValue = finger.target;
            handAnimator.SetFloat(fingerName, animationBlendValue);
        }
        /*  foreach (Finger finger in fingersToAnimate)
          {
              AnimateFinger(finger.type.ToString(), finger.target); 
          }*/
    }

     #region InputActions performed methods
    private void AnimateFinger(string fingerName, float animationBlendValue)
    {
        handAnimator.SetFloat(fingerName, animationBlendValue);
    }
     #endregion

   

   /* private void SmoothFingerAnimation(List<Finger> fingersToSmooth)
    {
        foreach (Finger finger in fingersToSmooth)
        {
            float animationTimeStep = animationSpeed * Time.unscaledDeltaTime;
            finger.current = Mathf.MoveTowards(finger.current, finger.target, animationTimeStep);
        }
    }*/



}
