using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;
using static ObstacleType;
using Unity.VisualScripting;
using System;

public class ObstacleSpawner : MonoBehaviour {
    [Header("Spawn Settings")]
    public float spawnRate;
    public float randomness;
    public int maxSpawnAttempts = 100;
    public List<Obstacle> obstacles;
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
    private Queue<GameObject> sectionsToPopulate = new Queue<GameObject>();
    private List<Bounds> activeSafeZones = new List<Bounds>();

    [Header("Test")]
    public GameObject mapSectionTest;
    public Bounds currentSpawnArea;
    public GameObject unsafelySpawnedObstacle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake() {
    }

    void Start() {
        sectionsToPopulate = GetActiveSections();

        CalculateBorders(msManager.mapSection);
        if (spawnRate < 1) {
            spawnRate = 1;
        }

        PopulateSections();
        //populateSection(mapSectionTest);
    }

    // Update is called once per frame
    void Update() {

    }

    private void SpawnObstacle(GameObject mapSection) {
        Obstacle obstacle = GetRandomObstacle();
        Bounds spawnArea = GetSpawnArea(mapSection, obstacle);

        currentSpawnArea = spawnArea;
        int count = 0;
        GameObject instantiatedObstacle;
        Vector3 position;
        GameObject lastInstantiatedWrongObstacle;

        do {
            position = GetRandomPositionInBounds(spawnArea);
            count++;
            lastInstantiatedWrongObstacle = Instantiate(unsafelySpawnedObstacle, position, Quaternion.identity, mapSection.transform);
        } while (IsObstacleInsideSafeZone(obstacle, position) && count < maxSpawnAttempts);

        Destroy(lastInstantiatedWrongObstacle);
        instantiatedObstacle = Instantiate(obstacle.body, position, Quaternion.identity, mapSection.transform);
        Bounds safeZone = GetSafeZone(obstacle, instantiatedObstacle);
        activeSafeZones.Add(safeZone);
        DisplaySafeZoneOnSelected(instantiatedObstacle, safeZone);
        DisplaySpawnAreaOnSelected(instantiatedObstacle, spawnArea);
    }

    public void PopulateSections() {
        // for each map section that is yet to be populated
        while (sectionsToPopulate.Count > 0) {
            GameObject mapSection = sectionsToPopulate.Dequeue();
            for (int j = 0; j < spawnRate; j++) {
                SpawnObstacle(mapSection);
            }
        }
    }

    public void populateSection(GameObject mapSection) {
        for (int j = 0; j < spawnRate; j++) {
            SpawnObstacle(mapSection);
        }
    }

    public void QueueSection(GameObject mapSection) {
        if (mapSection != null) {
            sectionsToPopulate.Enqueue(mapSection);
        }
    }

    private bool IsObstacleInsideSafeZone(Obstacle obstacle, Vector3 position) {
        Bounds boundsToTest = new Bounds(position, obstacle.body.GetComponent<Renderer>().bounds.size);

        foreach (Bounds safeZone in activeSafeZones) {
            if (boundsToTest.Intersects(safeZone))
                return true;
        }

        return false;
    }

    private Bounds GetSpawnArea(GameObject mapSection, Obstacle obstacle) {
        CalculateBorders(mapSection);

        Renderer obRenderer = obstacle.body.GetComponent<Renderer>();

        // width of spawnArea = width of MapSection (from left to right wall) - width of obstacle - offset
        float sizeZ = borderR - borderL - obRenderer.bounds.size.z - 2 * spawnOffset;
        sizeZ = (sizeZ > 0) ? sizeZ : 0;
        Debug.Log("obCollider.bounds.size.z = " + obRenderer.bounds.size.z);

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
        Vector3 areaOrigin = new Vector3(mapSection.transform.position.x, areaOriginY, mapSection.transform.position.z);
        Debug.Log("areaOrigin: " + areaOrigin);

        // return length (X-Size of Ground) and width (sizeZ) of spawnArea
        return new Bounds(areaOrigin, new Vector3(sizeX, sizeY, sizeZ));
    }

    private void CalculateBorders(GameObject mapSection) {
        Transform wallLTransform = mapSection.transform.Find("Wall_left");
        Collider wallLCollider = wallLTransform?.gameObject.GetComponent<Collider>();
        borderL = wallLCollider.bounds.max.z;

        Transform wallRTransform = mapSection.transform.Find("Wall_right");
        Collider wallRCollider = wallRTransform?.gameObject.GetComponent<Collider>();
        borderR = wallRCollider.bounds.min.z;
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

    private Obstacle GetRandomObstacle() {
        ObstacleType selectedType = GetRandomObstacleType(obstacleWeights);

        var matching = obstacles.Where(o => o.type == selectedType).ToList();

        if (matching.Count == 0) {
            Debug.LogWarning($"No obstacles found for type: {selectedType}");
            return null;
        }

        int index = Random.Range(0, matching.Count);
        return matching[index];
    }

    private Bounds GetSafeZone(Obstacle obstacle, GameObject instance) {
        Vector3 szSize = obstacle.safeZoneSize;
        Vector3 obstacleSize = obstacle.body.GetComponent<Renderer>().bounds.size;

        float sizeY = Mathf.Max(maxY, obstacleSize.y);
        Vector3 safeZoneSize = new Vector3(Mathf.Max(szSize.x, obstacleSize.x), sizeY, Mathf.Max(szSize.z, obstacleSize.z));
        Vector3 safeZoneCenter = new Vector3(instance.transform.position.x, sizeY / 2, instance.transform.position.z);

        return new Bounds(safeZoneCenter, safeZoneSize);
    }

    private Queue<GameObject> GetActiveSections() {
        Queue<GameObject> activeSections = new Queue<GameObject>();
        foreach (Transform section in transform)
            activeSections.Enqueue(section.gameObject);

        return activeSections;
    }

    private void DisplaySpawnAreaOnSelected(GameObject instance, Bounds spawnArea) {
        instance.GetComponent<DisplayObstacleAreas>().spawnArea = spawnArea;
    }

    private void DisplaySafeZoneOnSelected(GameObject instance, Bounds safeZone) {
        instance.GetComponent<DisplayObstacleAreas>().safeZone = safeZone;
    }
}
