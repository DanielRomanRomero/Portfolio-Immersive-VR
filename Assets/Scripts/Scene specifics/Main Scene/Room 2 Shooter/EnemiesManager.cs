using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages a pool of enemies and spawns them using a predefined sequence.
/// Used in Room 2 to activate waves of enemies with delays.
/// </summary>
public class EnemiesManager : MonoBehaviour
{

    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject[] enemyPool;
    [SerializeField] private float spawnDelay = 3f; 

    private List<GameObject> remainingEnemies;

    // Index of secuence spawn
    private int spawnIndex = 0;
    // order the enemies should be spawned ( there are 4 positions)
    private List<int> spawnSequence = new List<int> { 0, 1, 2, 3, 2, 0, 1, 0, 2, 3, 2, 1, 0, 2, 3 }; // Secuencia predefinida

    public void ActivateEnemiesEvent()
    {
        remainingEnemies = new List<GameObject>(enemyPool);
        StartCoroutine(SpawnEnemiesSequence());
    }

    private IEnumerator SpawnEnemiesSequence()
    {
        while (spawnIndex < spawnSequence.Count && remainingEnemies.Count > 0)
        {
            int spawnerID = spawnSequence[spawnIndex]; 
            Transform spawnPoint = spawnPoints[spawnerID];

            GameObject enemy = GetInactiveEnemy();
            if (enemy != null)
            {
                SpawnEnemy(enemy, spawnPoint);
            }

            spawnIndex++; 
            yield return new WaitForSeconds(spawnDelay);
        }

        Debug.Log("No enemies remain. Game Manager should send to IState the data and ISTate just finish the event");
    }

    public int ReturnEnemyPool()
    {
        return enemyPool.Length;
    }

    private GameObject GetInactiveEnemy()
    {
        foreach (GameObject enemy in remainingEnemies)
        {
            if (!enemy.activeInHierarchy)
            {
                remainingEnemies.Remove(enemy);
                return enemy;
            }
        }
        return null;
    }

    private void SpawnEnemy(GameObject enemy, Transform spawnPoint)
    {
        enemy.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
        enemy.SetActive(true);
    }
}
