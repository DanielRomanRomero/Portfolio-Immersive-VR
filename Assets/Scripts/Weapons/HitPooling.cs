using laplahce.Projectiles;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Simple object pooler for hit visual effects. Expands if needed.
/// </summary>
public class HitPooling : MonoBehaviour
{
    public static HitPooling Instance;

    [SerializeField] private GameObject hitPrefab;
    [SerializeField] private int initialPoolSize = 20;

    private List<GameObject> hitPool = new();

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
            GameObject hit = Instantiate(hitPrefab);
            hit.SetActive(false);
            hit.transform.parent = transform;
            hitPool.Add(hit);
        }
    }

    public GameObject GetHit()
    {
        foreach (var hit in hitPool)
        {
            if (!hit.activeInHierarchy)
                return hit;
        }

        // If none are available, we expand the pool automatically.
        GameObject newHit = Instantiate(hitPrefab);
        hitPool.Add(newHit);
        return newHit;
    }

    // Optional method for returning hits to the pool
    public void ReturnHit(GameObject hit)
    {
        hit.SetActive(false);
    }
}
