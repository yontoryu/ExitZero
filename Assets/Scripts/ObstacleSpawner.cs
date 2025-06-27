using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;
using static ObstacleType;
using Unity.VisualScripting;
using System;
using System.Data.Common;
using UnityEngine.Rendering.Universal;

public class ObstacleSpawner : MonoBehaviour {
    [Header("Spawn Settings")]
    public int minObstacles;
    public int maxObstacles;
    public float randomness;
    public int maxSpawnAttempts = 100;
    public List<ObstacleSO> obstacles;
    public float spawnOffset = 0.05f;
    Dictionary<ObstacleType, float> obstacleWeights = new() {
        { Flat, 0.6f },
        { Tall, 0.05f },
        { Floating, 0.35f }
    };

    private float borderL, borderR;

    [Header("Safe Zone Settings")]
    public float maxY = 10f;

    [Header("MapSection Settings")]
    public MapSectionManager msManager;
    private List<(int, int, GameObject, Vector3)> activeObstacles = new List<(int, int, GameObject, Vector3)>();
    private GameObject currentMapSection;
    private int currentObstacleID = 0;
    public GameManager gameManager;

    public void CalculateBorders(GameObject mapSection) {
        Transform wallLTransform = mapSection.transform.Find("Wall_left");
        Collider wallLCollider = wallLTransform?.gameObject.GetComponent<Collider>();
        borderL = wallLCollider.bounds.max.z;

        Transform wallRTransform = mapSection.transform.Find("Wall_right");
        Collider wallRCollider = wallRTransform?.gameObject.GetComponent<Collider>();
        borderR = wallRCollider.bounds.min.z;
    }

    public void PopulateSection(GameObject mapSection) {
        CalculateBorders(mapSection);

        currentMapSection = mapSection;

        int spawnRate = Random.Range(minObstacles, maxObstacles + 1);

        currentObstacleID = 0;
        for (int j = 0; j < spawnRate; j++) {
            SpawnObstacle(mapSection);
        }
    }

    public void Refresh() {
        if (currentMapSection != null) {
            int currentMSID = msManager.GetID(currentMapSection);
            if (currentMSID != 0) {
                // Liste der Obstacles aktualisieren, unnötige rausschmeißen

                for (int i = activeObstacles.Count - 1; i >= 0; i--) {
                    (int sID, int oID, GameObject obs, Vector3 safeZoneSize) = activeObstacles[i];
                    Rigidbody obsRb = obs.GetComponent<Rigidbody>();
                    DisplaySafeZoneOnSelected(obs, GetCurrentSafeZone(obsRb.position, safeZoneSize));
                    obsRb.MovePosition(obsRb.position + new Vector3(gameManager.GetCurrentVelocity(), 0, 0) * gameManager.velocityFactor);

                    if (obsRb.position.x > 200) {
                        SafeDeleteObstacle(obs, sID, oID);
                    }
                }
            }
        }
    }

    public void SafeDeleteObstacle(GameObject obstacle, int sectionID, int obstacleID) {
        for (int i = 0; i < activeObstacles.Count; i++) {
            (int sID, int oID, GameObject obs, Vector3 sA) = activeObstacles[i];
            if (sID == sectionID && oID == obstacleID) {
                activeObstacles.Remove((sectionID, obstacleID, obs, sA));
                Destroy(obstacle);
            }
        }
    }

    private void SpawnObstacle(GameObject mapSection) {
        ObstacleSO obstacle = GetRandomObstacle();    // Generate a random Obstacle
        Bounds spawnArea = GetSpawnArea(mapSection, obstacle);  // Get the Spawn Area according to the obstacle
        // Debug.Log("Spawn Area MS " + msManager.GetID(mapSection) + ": " + spawnArea.center);
        int sectionID = msManager.GetID(mapSection);

        int count = 0;
        GameObject instantiatedObstacle;
        Vector3 position;
        // Enable to display unsafely spawned obstacles
        //GameObject lastInstantiatedWrongObstacle;

        do {
            position = GetRandomPositionInBounds(spawnArea);    // Get a random position in the Spawn Area
            count++;

            //lastInstantiatedWrongObstacle = Instantiate(unsafelySpawnedObstacle, position, Quaternion.identity, mapSection.transform);
        } while (WouldSpawnSafely(obstacle, position) && count < maxSpawnAttempts);

        if (count < maxSpawnAttempts) {
            //Destroy(lastInstantiatedWrongObstacle);
            instantiatedObstacle = Instantiate(obstacle.body, position, Quaternion.identity);
            instantiatedObstacle.name += "_" + sectionID + "." + currentObstacleID;
            currentObstacleID++;
            Vector3 safeZoneSize = GetSafeZoneSize(obstacle);
            activeObstacles.Add((sectionID, currentObstacleID, instantiatedObstacle, safeZoneSize));
            instantiatedObstacle.GetComponent<ObstacleCollisionHandler>().Initialize(this, sectionID, currentObstacleID, obstacle.baseDamage, obstacle.extraDamage);

            // Display Gizmos of Spawn Area and Safe Zones
            DisplaySafeZoneOnSelected(instantiatedObstacle, GetCurrentSafeZone(position, safeZoneSize));
            // DisplaySpawnAreaOnSelected(instantiatedObstacle, spawnArea);
        }
    }

    private bool WouldSpawnSafely(ObstacleSO obstacle, Vector3 position) {
        Bounds newObstacleBounds = new Bounds(position, obstacle.body.GetComponent<Renderer>().bounds.size);
        Bounds newObstacleSafeZone = GetCurrentSafeZone(position, GetSafeZoneSize(obstacle));

        foreach ((_, _, GameObject obsInstance, Vector3 safeZoneSize) in activeObstacles) {
            Rigidbody obsRb = obsInstance.GetComponent<Rigidbody>();
            Bounds safeZone = GetCurrentSafeZone(obsRb.position, safeZoneSize);
            if (newObstacleBounds.Intersects(safeZone) ||
                obsInstance.GetComponent<Renderer>().bounds.Intersects(newObstacleSafeZone)) {
                return true;
            }
        }

        return false;
    }

    private Bounds GetSpawnArea(GameObject mapSection, ObstacleSO obstacle) {
        Renderer obRenderer = obstacle.body.GetComponent<Renderer>();

        // width of spawnArea = width of MapSection (from left to right wall) - width of obstacle - offset
        float sizeZ = borderR - borderL - obRenderer.bounds.size.z - 2 * spawnOffset;
        sizeZ = (sizeZ > 0) ? sizeZ : 0;

        Transform groundTransform = mapSection.transform.Find("Ground");
        Renderer groundCollider = groundTransform?.gameObject.GetComponent<Renderer>();

        float sizeX = groundCollider.bounds.size.x;

        float sizeY;
        float areaOriginY;

        if (obstacle.type == Floating) {
            sizeY = obstacle.floatingSizeY;
            areaOriginY = obstacle.floatingCenterY;
        }
        else {
            sizeY = 0;
            areaOriginY = obRenderer.bounds.extents.y + spawnOffset;
        }

        Vector3 msOrigin = mapSection.GetComponent<Rigidbody>().position;
        Vector3 areaOrigin = new Vector3(msOrigin.x, areaOriginY, msOrigin.z);

        // return length (X-Size of Ground) and width (sizeZ) of spawnArea
        return new Bounds(areaOrigin, new Vector3(sizeX, sizeY, sizeZ));
    }

    private Vector3 GetRandomPositionInBounds(Bounds bounds) {
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        float z = Random.Range(bounds.min.z, bounds.max.z);
        return new Vector3(x, y, z);
    }

    private static ObstacleType GetRandomObstacleType(Dictionary<ObstacleType, float> weights) {
        float totalWeight = 0f;

        foreach (var pair in weights) {
            totalWeight += pair.Value;
        }

        float randomValue = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var pair in weights) {
            cumulative += pair.Value;
            if (randomValue <= cumulative) {
                return pair.Key;
            }
        }

        // fallback: only with empty list 
        return weights.Keys.First();
    }

    private ObstacleSO GetRandomObstacle() {
        ObstacleType selectedType = GetRandomObstacleType(obstacleWeights);

        var matching = obstacles.Where(o => o.type == selectedType).ToList();

        if (matching.Count == 0) {
            Debug.LogWarning($"No obstacles found for type: {selectedType}");
            return null;
        }

        int index = Random.Range(0, matching.Count);
        return matching[index];
    }

    private Vector3 GetSafeZoneSize(ObstacleSO obstacle) {
        Vector3 szSize = obstacle.safeZoneSize;
        Vector3 obstacleSize = obstacle.body.GetComponent<Renderer>().bounds.size;

        float sizeY = Mathf.Max(maxY, obstacleSize.y);
        Vector3 safeZoneSize = new Vector3(Mathf.Max(szSize.x, obstacleSize.x), sizeY, Mathf.Max(szSize.z, obstacleSize.z));

        return safeZoneSize;
    }

    private Bounds GetCurrentSafeZone(Vector3 position, Vector3 size) {
        Vector3 center = new Vector3(position.x, size.y / 2, position.z);

        return new Bounds(center, size);
    }

    private void DisplaySpawnAreaOnSelected(GameObject instance, Bounds spawnArea) {
        instance.GetComponent<DisplayObstacleAreas>().spawnArea = spawnArea;
    }

    private void DisplaySafeZoneOnSelected(GameObject instance, Bounds safeZone) {
        instance.GetComponent<DisplayObstacleAreas>().safeZone = safeZone;
    }
}
