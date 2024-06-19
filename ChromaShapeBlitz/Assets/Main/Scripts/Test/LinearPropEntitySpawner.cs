using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Serialization;

public struct EntitySpawnData
{
    public Vector3 SpawnPoint;
    public Vector3 EntityRotation;
    public LinearBounds BoundsAxis;
}

public class LinearPropEntitySpawner : MonoBehaviour
{
    [Space(5)] [Header("Entity Pooling")]
    [SerializeField] private GameObject[] linearEntities;
    private List<GameObject> entities;

    [Space(10)] [Header("Spawn Behaviour")]
    [SerializeField] private int minSpawnRate = 3;      // In Seconds, Inclusive
    [SerializeField] private int maxSpawnRate = 6;      // In seconds, Exclusive
    [SerializeField] private PointF spawnHeightRange;   // Inclusive of Upperbounds
    
    [Space(5)] [Header("Spawn height range is ignored when true")]
    [SerializeField] private bool isFixedSpawnHeight;
    [SerializeField] private float fixedSpawnHeight = 10.0F;

    //-----------------------------
    // ..:: RANDOMIZATION ::..
    //-----------------------------
    private int lastRandomEntityIndex;
    private int randomEntityIndex;

    [Space(5)] 
    [Header("Avoids picking the same number twice in a row")]
    [SerializeField] private int maxShuffleAttempts = 10;

    [Space(10)]
    [Header("Spawn points")]
    [SerializeField] private Point spawnRangeX;
    [SerializeField] private Point spawnRangeZ;

    void Start()
    {
        entities = new List<GameObject>();

        foreach (var entity in linearEntities)
        {
            var obj = Instantiate(entity, Vector3.zero, Quaternion.identity);
            obj.SetActive(false);

            entities.Add(obj);
        }

        StartCoroutine(SpawnEntities());
    }

    private IEnumerator SpawnEntities()
    {
        while (true) // (!isGameOver)
        {
            yield return new WaitForSeconds
            (
                Random.Range(minSpawnRate, maxSpawnRate + 1)
            );

            SpawnEntity();
        }
    }

    /// <summary>
    /// Spawn a single random unique entity.
    /// </summary>
    private void SpawnEntity()
    {
        var entityIndex = ShuffleEntityVariantIndex();
        var spawnData   = ShuffleSpawnPoint();
        var entityGO    = entities[entityIndex]; //Instantiate(entities[entityIndex], spawnPoint, Quaternion.identity);

        // If the selected entity is still active, 
        // we'll wait for it to become inactive
        if (entityGO.activeInHierarchy)
            return;

        entityGO.transform.position     = spawnData.SpawnPoint;
        entityGO.transform.eulerAngles  = spawnData.EntityRotation;

        entityGO.TryGetComponent(out LinearPropEntity propEntity);

        if (!entityGO.activeInHierarchy)
            entityGO.SetActive(true);

        if (propEntity != null)
        {
            propEntity.DisableOnReachedBounds = spawnData.BoundsAxis;
            propEntity.CanMove = true;
        }
    }

    /// <summary>
    /// Generate random integers that dont repeat.
    /// This makes multiple attempts to pick a new value to avoid repetition.
    /// </summary>
    /// <returns>Random number exclusive of upper bound</returns>
    private int ShuffleEntityVariantIndex()
    {
        for (int i = 0; randomEntityIndex == lastRandomEntityIndex && i < maxShuffleAttempts; i++)
        {
            randomEntityIndex = Random.Range(0, entities.Count);
        }

        lastRandomEntityIndex = randomEntityIndex;

        return randomEntityIndex;
    }

    /// <summary>
    /// <para>Generate random spawn points with their appropriate entity rotations.</para>
    /// <para>
    /// The generated spawn points are linear, which means the chosen spawn point must be a single Vector3 axis.
    /// The rotations are specific to spawn point axes.
    /// </para>
    /// </summary>
    /// <returns>Single axis spawn point</returns>
    private EntitySpawnData ShuffleSpawnPoint()
    {
        // Spawn axis can either be X (0) and Z (1)
        var spawnAxis = Random.Range(0, 2);

        // Spawn direction can either be negative or positive
        var spawnDirection = Random.Range(0, 2);
        
        var spawnHeight = isFixedSpawnHeight ? 
                          fixedSpawnHeight   : 
                          Random.Range(spawnHeightRange.A, spawnHeightRange.B);

        EntitySpawnData spawnData = default;        
        float entityEulerY;

        switch (spawnAxis)
        {
            // X Axis
            case 0:
                var dirX       = spawnDirection == 0 ? spawnRangeX.A : spawnRangeX.B;
                entityEulerY   = spawnDirection == 0 ? 90 : -90;

                spawnData = new EntitySpawnData
                {
                    SpawnPoint      = new Vector3(dirX, spawnHeight, 0.0F),
                    EntityRotation  = new Vector3(0.0F, entityEulerY, 0.0F),
                    BoundsAxis      = LinearBounds.XAxis
                };
                break;

            // Z Axis
            case 1:
                var dirZ       = spawnDirection == 0 ? spawnRangeZ.A : spawnRangeZ.B;
                entityEulerY   = spawnDirection == 0 ? 0 : -180;

                spawnData = new EntitySpawnData
                {
                    SpawnPoint      = new Vector3(0.0F, spawnHeight, dirZ),
                    EntityRotation  = new Vector3(0.0F, entityEulerY, 0.0F),
                    BoundsAxis      = LinearBounds.ZAxis
                };
                break;
        }

        return spawnData;
    }
}
