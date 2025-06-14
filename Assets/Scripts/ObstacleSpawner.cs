using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;
using static ObstacleType;

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

    [Header("MapSection Settings")]
    public MapSectionManager msManager;
    private Queue<GameObject> sectionsToPopulate = new Queue<GameObject>();
    private List<GameObject> activeObstacles = new List<GameObject>();

    [Header("Test")]
    public GameObject mapSectionTest;
    public Bounds currentSpawnArea;

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
    }

    // Update is called once per frame
    void Update() {

    }

    private void SpawnObstacle(GameObject mapSection) {
        Obstacle randomObstacle = GetRandomObstacle();
        Bounds spawnArea = GetSpawnArea(mapSection, randomObstacle);
        DisplaySpawnAreaOnSelected(randomObstacle, spawnArea);

        currentSpawnArea = spawnArea;
        int count = 0;

        do {
            Vector3 position = GetRandomPositionInBounds(spawnArea);
            Instantiate(randomObstacle.body, position, Quaternion.identity, mapSection.transform);
            count++;
        } while (!ObstacleSpawnedSafely() && count < maxSpawnAttempts);
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

    public void popSection(GameObject mapSection) {
        for (int j = 0; j < spawnRate; j++) {
            SpawnObstacle(mapSection);
        }
    }

    public void QueueSection(GameObject mapSection) {
        if (mapSection != null) {
            sectionsToPopulate.Enqueue(mapSection);
        }
    }

    private bool ObstacleSpawnedSafely() {
        return true;
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

    private Queue<GameObject> GetActiveSections() {
        Queue<GameObject> activeSections = new Queue<GameObject>();
        foreach (Transform section in transform)
            activeSections.Enqueue(section.gameObject);

        return activeSections;
    }

    private void DisplaySpawnAreaOnSelected(Obstacle obstacle, Bounds spawnArea) {
        obstacle.body.GetComponent<ShowSpawnArea>().spawnArea = spawnArea;
    }
}
