using System.Collections;
using UnityEngine;
using UnityEngine.Events;


namespace ArmnomadsGames
{

    [System.Serializable]
    public class TriggerEvent : UnityEvent<Collider> { }

    [System.Serializable]
    public class SliceEventOne : UnityEvent<Vector3> { }

    [System.Serializable]
    public class SliceEventTwo : UnityEvent<GameObject, GameObject> { }



    /// <summary>
    /// This class was part of a laserSword asset. currently is just used for:
    /// 1.Check saber color
    /// 2.Check saber velocity/direction
    /// 3.Call enable/disable saber and keep saber consistency as it was design
    /// </summary>
    public class LaserSword : MonoBehaviour
    {
        [Space(10)]

        [SerializeField] bool enableAtStart = true;

        [Space(10)]

        [Header("---------- General Settings ----------")]
        [SerializeField] private bool isRedSaber; //  Define if this saber is red or blue
        public bool IsRedSaber { get { return isRedSaber; } }

        [SerializeField] private AudioClip hitSound;
       
        [Header("---------- Cut Direction ----------")]
        private Vector3 lastPos;
        public Vector3 velocity { get; private set; }


        [Header("---------- Laser ----------")]
        [SerializeField] [Range(12, 40)] float laserFadeSpeed = 18;
        [SerializeField] float laserShowTime = 0.3f;
        [SerializeField] float laserHideTime = 0.3f;
        [SerializeField] AnimationCurve curveScaleUp;
        [SerializeField] AnimationCurve curveScaleDown;


        [SerializeField] PhysicsMaterial slicedPhysicMaterial;

        [Header("---------- Flickering ----------")]
        [SerializeField] bool flickering;
        [SerializeField] [Range(0.01f, 0.05f)] float flickRate = 0.025f;
        [SerializeField] [Range(0.1f, 0.9f)] float flickMin = 0.5f;
        [SerializeField] [Range(1, 8)] float lightMultiplier = 2f;
        [SerializeField] Light laserLight;

        [Header("---------- Audio ----------")]
        //[SerializeField] [Range(0.1f, 0.5f)] float hitSoundCooldown = 0.3f;
        [SerializeField] AudioSource audioIdle;
        [SerializeField] AudioSource audioInstant;
        [SerializeField] AudioClip clipHit;
        [SerializeField] AudioClip clipMove;
        [SerializeField] AudioClip clipShowHide;

        [Header("---------- Events ----------")]
        [SerializeField] TriggerEvent OnTriggerEnter;
        [SerializeField] TriggerEvent OnTriggerStay;
        [SerializeField] TriggerEvent OnTriggerExit;
        [SerializeField] SliceEventOne OnSliceStart;
        [SerializeField] SliceEventTwo OnSliceEnd;


        private bool reset;
        private int laserLayerIndex;
        private float lerpTime;
        private float currentTime;
        private float flickTimer;
        //private float hitAudioTimer;
        private LayerMask laserLayerMask;
        private RaycastHit rHit;
        private Vector3 collStartPoint;
        private Vector3 collEndPoint_1;
        private Vector3 collEndPoint_2;
        private Transform boneTarget;
        private Transform laserTrans;
        private Transform laserBone;
        private Transform rigTrans;
        private Transform origin;



        #region MONO BEHAVIOUR
        void Start()
        {
            // Caching References
            laserTrans = transform.Find("Laser");
            rigTrans = laserTrans.Find("rig");
            laserBone = rigTrans.Find("Bone03");
            origin = rigTrans.Find("origin");

            // Set layers
            laserLayerMask = LayerMask.GetMask("LaserSword");
            laserLayerIndex = LayerMask.NameToLayer("LaserSword");


            // Disable
            this.enabled = false;

            rigTrans.localScale = Vector3.zero;

            if (enableAtStart)
                Enable();
        }

        void Update()
        {
            UpdateLaser();

            velocity = (transform.position - lastPos) / Time.deltaTime;
            lastPos = transform.position;
           
        }

        void FixedUpdate()
        {
            if (!flickering)
                return;

            flickTimer += Time.deltaTime;
            if (flickTimer > flickRate)
            {
                flickTimer = 0f;
                float r = Random.Range(flickMin, 1f);

                rigTrans.localScale = new Vector3(r * 1f, 1f, r * 1f);

                if (laserLight != null)
                    laserLight.intensity = r * lightMultiplier;
            }
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(collStartPoint, 0.1f);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(collEndPoint_1, 0.1f);
            Gizmos.DrawSphere(collEndPoint_2, 0.1f);
        }
#endif

       

        //void TriggerStay(Collider coll)
        //{
        //    OnTriggerStay?.Invoke(coll);

        //    // Other Laser
        //    if (coll.gameObject.layer == laserLayerIndex)
        //    {
        //        if (Physics.Linecast(laserTrans.position, origin.position, out rHit, laserLayerMask))
        //        {
        //            if (contactLensFlare != null)
        //            {
        //                if (!contactLensFlare.enabled)
        //                    contactLensFlare.enabled = true;

        //                contactLensFlare.brightness = Random.Range(lensMin, lensMax);
        //                contactLensFlare.transform.position = rHit.point;
        //            }
        //        }
        //    }
        //}


       

        private IEnumerator DisableAfterDelay(GameObject obj, float delay)
        {
            yield return new WaitForSeconds(delay);
            obj.SetActive(false);
        }

       
        
        #endregion


        public void Enable()
        {
            if (this.enabled)
                return;

            this.enabled = true;

          //  laserCollider.Enable = true;
            boneTarget.position = origin.position;

            StartCoroutine(IE_ScaleLaser(laserShowTime, curveScaleUp));

            #region EFFECTS

            if (audioIdle != null)
                audioIdle.Play();
            if (audioInstant != null)
            {
                audioInstant.clip = clipShowHide;
                audioInstant.Play();
            }

            if (laserLight != null)
                laserLight.enabled = true;
            //if (lightning_particle != null && lightning_particle.gameObject.activeSelf == true)
            //    lightning_particle.Play();
          //  if (smoke_particle != null && smoke_particle.gameObject.activeSelf == true)
            //    smoke_particle.Play();

            #endregion
        }

        /// <summary>
        /// Disable the laser.
        /// </summary>
        public void Disable()
        {

            StartCoroutine(IE_ScaleLaser(laserHideTime, curveScaleDown));
            #region EFFECTS

            if (audioIdle != null)
                audioIdle.Stop();
            if (audioInstant != null)
            {
                audioInstant.clip = clipShowHide;
                audioInstant.Play();
            }

            if (laserLight != null)
                laserLight.enabled = false;

            //if (lightning_particle != null && lightning_particle.gameObject.activeSelf == true)
            //    lightning_particle.Stop();

           // if (smoke_particle != null && smoke_particle.gameObject.activeSelf == true)
           //     smoke_particle.Stop();

            //if (contactLensFlare != null)
            //    contactLensFlare.enabled = false;

            #endregion
        }

        private void UpdateLaser()
        {
            if (!boneTarget)
                return;

            boneTarget.position = Vector3.Lerp(boneTarget.position, origin.position, lerpTime);
            if (Vector3.Distance(boneTarget.position, origin.position) > 0.1f)
            {
                reset = false;
                currentTime = 0;
            }

            if (!reset)
            {
                laserBone.right = laserBone.position - boneTarget.position;
                lerpTime = Time.deltaTime * laserFadeSpeed;
            }
            else
            {
                laserBone.localEulerAngles = new Vector3(0, 0, 269.7f);
            }

            currentTime += Time.deltaTime;
            if (currentTime > lerpTime - 0.1f)
                reset = true;
        }

     
 

        private IEnumerator IE_ScaleLaser(float duration, AnimationCurve curve)
        {
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                timer = Mathf.Clamp(timer, 0f, duration);
                float percent = timer / duration;
                float value = curve.Evaluate(percent);
                rigTrans.localScale = Vector3.one * value;

                yield return null;
            }
        }
    }

}