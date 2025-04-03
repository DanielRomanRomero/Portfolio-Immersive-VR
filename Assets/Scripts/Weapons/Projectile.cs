using UnityEngine;
/// <summary>
/// Basic projectile with automatic despawn, hit feedback and damage.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifeTime = 5f;

    private float timer;
    private Rigidbody rb;

    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private TrailRenderer trailRenderer2;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        timer = 0f;
        trailRenderer.Clear();
        trailRenderer2.Clear();
       
        rb.linearVelocity = transform.forward * speed;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= lifeTime)
            ProjectilePooling.Instance.ReturnProjectile(gameObject);

    }

    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyController>().TakeDamage();
        }
       
        GameObject hit = HitPooling.Instance.GetHit();
        hit.transform.position = transform.position;
        hit.transform.rotation = transform.rotation;
        hit.SetActive(true);

        ProjectilePooling.Instance.ReturnProjectile(gameObject);
    }


    void OnDisable()
    {
        rb.linearVelocity = Vector3.zero; // Clean velocity after disable
    }
}
