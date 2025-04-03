using laplahce.Projectiles;
using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// Manages shooting logic for the VR pistol, including pooling and FX.
/// </summary>
public class VRPistolGunController : MonoBehaviour
{
    [SerializeField] private InputActionReference triggerShotAction;
    [SerializeField] private Transform shotPoint;
    [SerializeField] private Particle muzzleParticle;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shotAudio;

    private float lastShotTime;

    private void OnEnable()
    {
        triggerShotAction.action.performed += Shot;
    }

    private void OnDisable()
    {
        triggerShotAction.action.performed -= Shot;
    }


    private void Shot(InputAction.CallbackContext context)
    {
        if (Time.time - lastShotTime < fireRate) 
            return; //Control fire Rate

        lastShotTime = Time.time;

        // Get projectile from pool
        GameObject projectile = ProjectilePooling.Instance.GetProjectile();

        // Positioning and shoot projectile
        projectile.transform.position = shotPoint.position;
        projectile.transform.rotation = shotPoint.rotation;
        projectile.SetActive(true);

        // Activate Muzzle Flash:
        muzzleParticle.PlayEffect();
        // Play shot audio
        audioSource.PlayOneShot(shotAudio);
    }
}
