using System.Collections;
using UnityEngine;

/// <summary>
/// Handles individual cube piece behavior after a successful hit in the Beat Saber-style minigame at room3
/// Each piece can be launched with physics and dissolve over time before resetting and deactivating itself.
/// </summary>
public class PieceBehaviour : MonoBehaviour
{

    [Header("Jump Settings")]
    [SerializeField] private float extraJumpForce = 1.5f;

    [Header("References")]
    [SerializeField] private CubeController cubeParent;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private MeshRenderer mesh;

    private Material mat;

    // Shader dissolve control
    private float dissolveValue = 1f;
    private float dissolveSpeed = 1f;
    private int hash_Dissolve = Shader.PropertyToID("_DISSOLVE");

    // Initial position/rotation for reset
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private bool iniciateObjectOnces = false;

    /// <summary>
    /// Initializes the object if not already initialized.
    /// Caches original transform and prepares the material for dissolve effect.
    /// </summary>
    private void InitObject()
    {
       //Take mesh renderer to change material later
       mesh = GetComponent<MeshRenderer>();
       mat = mesh.material;

       //Save initial position & rotation
       initialPosition = transform.localPosition;
       initialRotation = transform.localRotation;
       
        //Set it has been iniciated
       iniciateObjectOnces = true;
    }


    /// <summary>
    /// Resets transform and shader to prepare for reuse.
    /// Called when the cube is (re)activated.
    /// </summary>
    public void ResetBasicValues()
    {
        // If first time, init the object
        if (!iniciateObjectOnces)
        {
            InitObject();
            iniciateObjectOnces = true;
        }

        // Reset Rigidbody values
        rb.isKinematic = true;
        rb.useGravity = false;

        // Reset position & rotation
        transform.localPosition = initialPosition;
        transform.localRotation = initialRotation;

        // Reset shader dissolve value
        mat.SetFloat(hash_Dissolve, 1f); 
    }

    /// <summary>
    /// Called by the cube parent when the cube is sliced.
    /// Applies random force to simulate an explosion and starts dissolve routine.
    /// </summary>
    public void Jump()
    {
        // Set rigidbody ready for jump (activates physics)
        rb.isKinematic = false;
        rb.useGravity = true;

        // Choose a random dirección...
        Vector3 dir = new Vector3(Random.Range(-2f, 2f), Random.Range(2f, 3f), Random.Range(-2f, 2f)).normalized;

        // ... then jump...
        rb.AddForce(dir * extraJumpForce, ForceMode.Impulse);

        // ... and rotate...
        rb.AddTorque(Random.insideUnitSphere * 10f, ForceMode.Impulse);//Activate later

        // ...and call corrutine to start dissolving trhough shader value
        if(gameObject != null)
            StartCoroutine(DissolveRoutine());
    }

    /// <summary>
    /// Coroutine that gradually dissolves the piece using shader property.
    /// After dissolving, it resets and deactivates the piece.
    /// </summary>
    private IEnumerator DissolveRoutine()
    {
        //Starts visible
        dissolveValue = 1f;

        yield return new WaitForSeconds(0.15f);

        // if shader dissolve value is less than 0...
        while (dissolveValue > 0)
        {
            // substract time to dissolve value
            dissolveValue -= Time.deltaTime * dissolveSpeed;
            mat.SetFloat(hash_Dissolve, dissolveValue);
            yield return null;
        }

        // Optional delay to allow piece to move further away before disappearing
        yield return new WaitForSeconds(5f);

        // after that time, reset values again (also cube parent calls this, just in case)
        ResetBasicValues();

        // deactivate object so is not around while is not taken again
        gameObject.SetActive(false);

        // Ensure parent cube deactivates itself
        cubeParent.DeactivateGO();  

    }
}
