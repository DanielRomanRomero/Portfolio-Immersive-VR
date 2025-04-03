using UnityEngine;

/// <summary>
/// Manages the instantiation and reuse of red and blue cubes in the Beat Saber-style minigame.
/// Handles spawning cubes from pools and triggering dissolve effects on active ones.
/// </summary>
public class SpawnCubes : MonoBehaviour
{
    [Header("Spawn Points")]
    public Transform redCubesSpawnPoint;
    public Transform blueCubesSpawnPoint;

    [Header("Cube Pools")]
    public GameObject[] blueCubes;
    public GameObject[] redCubes;
    
    // Index tracking the next cube to spawn from each pool
    private int nextBlueCubeIndex = 0;
    private int nextRedCubeIndex = 0;


    /// <summary>
    /// Activates the next blue cube in the pool and positions it at the spawn point.
    /// </summary>
    public void CreateNewBlueCubes()
    {
        if (blueCubes.Length == 0)
            return;

        GameObject cube = blueCubes[nextBlueCubeIndex];
        cube.transform.SetParent(blueCubesSpawnPoint, false);
        cube.SetActive(true);

        if (nextBlueCubeIndex < blueCubes.Length -1)
        {
            nextBlueCubeIndex++;
        }
    }

    /// <summary>
    /// Activates the next red cube in the pool and positions it at the spawn point.
    /// </summary>
    public void CreateNewRedCubes()
    {
        if (redCubes.Length == 0)
            return;

        GameObject cube = redCubes[nextRedCubeIndex];
        cube.transform.SetParent(redCubesSpawnPoint, false);
        cube.SetActive(true);

        if (nextRedCubeIndex < redCubes.Length -1)
        {
            nextRedCubeIndex++;
        }

    }

    /// <summary>
    /// Calls the dissolve effect on all currently active cubes in both pools.
    /// Used when the minigame ends.
    /// </summary>
    public void DissolveAllActiveCubes()
    {
        foreach (var cube in blueCubes)
        {
            if (cube.activeInHierarchy)
            {
                cube.GetComponent<CubeController>().DissolveCube();
            }
        }

        foreach (var cube in redCubes)
        {
            if (cube.activeInHierarchy)
            {
                cube.GetComponent<CubeController>().DissolveCube();
            }
        }
    }
}
