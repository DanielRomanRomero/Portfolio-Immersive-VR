using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Object pooler for projectiles. Prevents instantiation spikes during shooting.
/// </summary>
public class ProjectilePooling : MonoBehaviour
{
    public static ProjectilePooling Instance;

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private int initialPoolSize = 20;

    private List<GameObject> projectilePool = new List<GameObject>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        InitializePool();
    }

    void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject projectile = Instantiate(projectilePrefab);
            projectile.SetActive(false);
            projectile.transform.parent = transform;
            projectilePool.Add(projectile);
            
        }
    }

    public GameObject GetProjectile()
    {
        foreach (var projectile in projectilePool)
        {
            if (!projectile.activeInHierarchy)
                return projectile;
        }

        // If none are available, we expand the pool automatically.
        GameObject newProjectile = Instantiate(projectilePrefab);
        projectilePool.Add(newProjectile);
        return newProjectile;
    }

    // Optional method for returning projectiles to the pool
    public void ReturnProjectile(GameObject projectile)
    {
        projectile.SetActive(false);
        projectile.transform.rotation = Quaternion.identity;
    }
}
