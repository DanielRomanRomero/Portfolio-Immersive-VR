using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;


public enum EnemyState
{
    Chasing, Attacking, Death
}

/// <summary>
/// Controls enemy behavior: movement, attack, and death.
/// Enemies die after 2 hits and return to pool.
/// </summary>
public class EnemyController : MonoBehaviour
{
    public EnemyState enemyState = EnemyState.Chasing;

    [Header("Movement")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform targetPosition;
    [SerializeField] private float speed = 3f;

    [Header("Attacks")]
    [SerializeField] private float attackDistance = 1.5f;
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("Hits Life")]
    [SerializeField] private int maxHits = 2;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip showClip;
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip deathClip;

    private Animator animator;
    private int currentHits = 0;
    private float attackTimer = 0f;

    private readonly int attack_id = Animator.StringToHash("Attack");
    private readonly int Death_id = Animator.StringToHash("FlyingDeath");


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        audioSource.PlayOneShot(showClip);
    }

    private void Update()
    {
        switch (enemyState)
        {
            case EnemyState.Chasing:
                MoveToTarget();
                break;

            case EnemyState.Attacking:
                Attack();
                break;
        }
    }

    //  1. Movement to player
    private void MoveToTarget()
    {
        if (targetPosition == null) return;

        Vector3 direction = (targetPosition.position - transform.position).normalized;
        characterController.Move(direction * speed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(direction);

        if (Vector3.Distance(transform.position, targetPosition.position) <= attackDistance)
        {
            enemyState = EnemyState.Attacking;
            animator.CrossFadeInFixedTime(attack_id, 0.1f);
        }
    }


    //  2. Attack to player every 'attackCooldown' float seconds
    private void Attack()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;
            audioSource.PlayOneShot(attackClip);
            animator.CrossFadeInFixedTime(attack_id, 0.1f);
            //animator.Play(attack_id);
            
        }
    }
    //  3. Get Damage and die after receive maxHits
    public void TakeDamage()
    {
        currentHits++;

        if (currentHits >= maxHits)
        {
            Death();
        }
    }

    //  4. Die and return to the pool
    private void Death()
    {
        enemyState = EnemyState.Death;
        audioSource.PlayOneShot(deathClip);
        animator.CrossFadeInFixedTime(Death_id, 0.2f);
        characterController.enabled = false;

        StartCoroutine(ExaggerateEnemyDeath());
    }

    private IEnumerator ExaggerateEnemyDeath()
    {
        float time = 1.5f;
        float elapsedTime = 0;
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + (-transform.forward * 2.5f) + (Vector3.down * 1.5f); 

        while (elapsedTime < time)
        {
            if (!gameObject.activeSelf) yield break; 

            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / time);

            yield return null;
        }

        Invoke(nameof(ReturnToPool), 1f);
    }


    //  5. Return to the pool and Reset enemy
    private void ReturnToPool()
    {
        GameManager.Instance.CountDeathEnemiesRoom2();
        gameObject.SetActive(false);
        currentHits = 0;
        enemyState = EnemyState.Chasing;
    }

}
