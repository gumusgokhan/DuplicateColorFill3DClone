using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable object to create object pool.
/// </summary>

[CreateAssetMenu]
public class ReplicaCubeObjectPool : ScriptableObject
{
    public int amount;

    public ReplicaCube replicaCubePrefab;

    public Queue<ReplicaCube> replicaCubePool;

    private Transform parent;

    /// <summary>
    /// Creates an object pool.
    /// </summary>
    public void SpawnPool()
    {

        replicaCubePool = new Queue<ReplicaCube>();

        if (parent == null)
            parent = new GameObject(name).transform;

        for (int i = 0; i < amount; i++)
        {
            ReplicaCube repCube = Instantiate(replicaCubePrefab,parent);
            repCube.gameObject.SetActive(false);
            repCube.filled = false;
            replicaCubePool.Enqueue(repCube);
        }

    }

    /// <summary>
    /// Get object from pool and make it active in the scene in correct position and rotation.
    /// </summary>
    public ReplicaCube GetObjectFromPool(Vector3 newPos, Quaternion newRot)
    {
        if (replicaCubePool == null || replicaCubePool.Count == 0)
        {
            SpawnPool();
        }

        ReplicaCube repCube = replicaCubePool.Dequeue();

        repCube.gameObject.SetActive(false);

        repCube.transform.position = newPos;
        repCube.transform.rotation = newRot;
        repCube.gameObject.SetActive(true);

        return repCube;
    }

    /// <summary>
    /// If player dies, unfilled cubes will be put back in the pool.
    /// </summary>
    public void DeactivateReplicaCube(ReplicaCube repCube)
    {
        repCube.filled = false;
        repCube.shouldHarm = false;
        repCube.gameObject.SetActive(false);
        replicaCubePool.Enqueue(repCube);
    }
}
