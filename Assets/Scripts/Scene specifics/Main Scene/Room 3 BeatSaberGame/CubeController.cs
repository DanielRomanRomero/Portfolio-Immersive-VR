using ArmnomadsGames;
using System.Collections;
using UnityEngine;

/// <summary>
/// This script controls each cube in the beat-based minigame.
/// It handles cut direction detection, saber color matching, dissolving, particle effects and scoring.
/// </summary>
public class CubeController : MonoBehaviour
{
    [Header("Cube References")]
    [SerializeField] private bool isRedCube;
    [SerializeField] private GameObject arrowObject;
    [SerializeField] private MeshRenderer cubeFullMesh;

    [Header("Cube Pieces")]
    [SerializeField] private GameObject upHalf;
    [SerializeField] private GameObject downHalf;
    [SerializeField] private GameObject leftHalf;
    [SerializeField] private GameObject rightHalf;
    [SerializeField] private PieceBehaviour[] scriptPieces;
    [SerializeField] private ParticleSystem sparkParticles;

    //Dissolve shader (event room 3 finished)
    private Material mat;
    private float dissolveValue = 1f;
    private float dissolveSpeed = 1f;
    private int hash_Dissolve = Shader.PropertyToID("_DISSOLVE");


    [Header("Saber Detection")]
    [SerializeField] private string lightSaberTag = "Laser"; // laser saber tag
    private bool hasBeenHit = false; // To avoid multiple detections
    private bool rightDirection = false; //right direction always is 'up'


    [Header("Audio")]
    [SerializeField] private AudioClip successClip;
    [SerializeField] private AudioClip failClip;


    private void OnEnable()
    {
        //Start with main cube mesh enable...
        if (cubeFullMesh != null)
            cubeFullMesh.enabled = true;

        //...and with all cube GO pieces disabled and reset (clean all pieces)
        foreach (var piece in scriptPieces)
        {
            piece.ResetBasicValues();
            piece.gameObject.SetActive(false);
        }

        // reset values in order to be able to beat it again
        hasBeenHit = false;
        rightDirection = false;

        // activate arrow 
        arrowObject.SetActive(true);

    }

    private void OnTriggerEnter(Collider other)
    {
        // if has been hit or collide with other that is not lightsaber, return
        if (hasBeenHit || !other.CompareTag(lightSaberTag))
            return;

        // deactivate arrow & main mesh cube
        arrowObject.SetActive(false);
        cubeFullMesh.enabled = false;


        // get laser component to get its direction. Returns if null
        LaserSword laser = other.GetComponentInParent<LaserSword>();
        if (laser == null) return;

        // gets direction using InverseTransformDirection
        Vector3 localDirection = transform.InverseTransformDirection(laser.velocity).normalized;

        // Returns direction in absolute values
        float absX = Mathf.Abs(localDirection.x);
        float absY = Mathf.Abs(localDirection.y);

        // for checking if saber is same color than this cube
        bool isSaberRed = laser.IsRedSaber;

        // if absY is bigger than abs X, the direction is up or down
        if (absY > absX)
        {
            // so check it out which is and activate GO pieces acordly
            if (localDirection.y > 0)
            {
               leftHalf.SetActive(true);
               rightHalf.SetActive(true);
              // print("Collision from down (Local)");
            }
            else if (localDirection.y < 0)
            {
                 leftHalf.SetActive(true);
                 rightHalf.SetActive(true);
                // print("Collision from up (Local)");
                rightDirection = true;
            }

            // we could also comment this above and call them here
            // leftHalf.SetActive(true);
            // rightHalf.SetActive(true);

            // Then call pieces to jump. Jump will call to dissolve 
            leftHalf.GetComponent<PieceBehaviour>().Jump();
            rightHalf.GetComponent<PieceBehaviour>().Jump();

        }
        else // if contrary, the direction is left or right
        {
            // so check it out which is and activate GO pieces acordly
            if (localDirection.x > 0)
            {
                upHalf.SetActive(true);
                downHalf.SetActive(true);
                //print("Collision from left (Local)");
            }
            else if (localDirection.y < 0)
            {
                upHalf.SetActive(true);
                downHalf.SetActive(true);
                //print("Collision from right (Local)");
            }

            // we could also comment this above and call them here
            // upHalf.SetActive(true);
            // downHalf.SetActive(true);

            // Then call pieces to jump. Jump will call to dissolve 
            upHalf.GetComponent<PieceBehaviour>().Jump();
            downHalf.GetComponent<PieceBehaviour>().Jump();
        }


        SpreadParticles(localDirection);

        // Play sounds depending if hit is right or not

        bool validHit = rightDirection && isSaberRed == isRedCube;

        if (validHit)
        {
            PlaySoundAt.Instance.PlayAtPosition(successClip, transform.position, 0.3f);

            if (isRedCube)
                GameManager.Instance.Room3RedCubesCounter();
            else
                GameManager.Instance.Room3BlueCubesCounter();
        }
        else
        {
            PlaySoundAt.Instance.PlayAtPosition(failClip, transform.position, 0.3f);
        }
            
        // set this cube has been hit
        hasBeenHit = true;
    }


    public void SpreadParticles(Vector3 cutDirection)
    {
        // Orienta las chispas según el corte
        sparkParticles.transform.rotation = Quaternion.LookRotation(cutDirection);

        // Reproduce las partículas
        sparkParticles.Play();
    }

    public void DeactivateGO()
    {
        gameObject.SetActive(false);
    }


    public void DissolveCube()
    {
        mat = cubeFullMesh.material;
        StartCoroutine(DissolveRoutine());
    }


    private IEnumerator DissolveRoutine()
    {
        // hide arrow
        arrowObject.SetActive(false);

        //Starts visible
        dissolveValue = 1f;

        yield return new WaitForSeconds(0.3f);

        // if shader dissolve value is bigger than 0...
        while (dissolveValue > 0)
        {
            // substract time to dissolve value
            dissolveValue -= Time.deltaTime * dissolveSpeed;
            mat.SetFloat(hash_Dissolve, dissolveValue);
            yield return null;
        }

        DeactivateGO();
    }

}
