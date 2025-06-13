using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class ObstacleSpawner : MonoBehaviour {
    [Header("Spawn Settings")]
    public float spawnRate;
    public float randomness;
    public GameObject[] obstacles;
    public float spawnOffset = 0.05f;
    public float FloatingObstacleMinZ;
    public float FloatingObstacleMaxZ;
    private enum ObstacleType {
        Flat,
        Tall,
        Floating
    }
    private float borderL, borderR;

    [Header("MapSection Settings")]
    public MapSectionManager msManager;
    private Queue<GameObject> sectionsToPopulate;
    private List<GameObject> activeObstacles = new List<GameObject>();

    [Header("Test")]
    public GameObject mapSectionTest;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake() {
        sectionsToPopulate = new Queue<GameObject>(msManager.GetActiveSections());
    }

    void Start() {
        CalculateBorders(msManager.mapSection);
        if (spawnRate < 1) {
            spawnRate = 1;
        }
    }

    // Update is called once per frame
    void Update() {

    }

    private void SpawnObstacle(GameObject mapSection) {

    }

    public void PopulateSection() { }

    public void QueueSection(GameObject mapSection) {
        if (mapSection != null) {
            sectionsToPopulate.Enqueue(mapSection);
        }
    }

    private bool ObstacleSpawnedSafely() {
        return false;
    }

    private (Vector3 center, Vector3 size) CalculateSpawnArea(GameObject mapSection, GameObject obstacle) {
        CalculateBorders(mapSection);

        // Get the left and right borders of the map
        Collider obCollider = obstacle.GetComponent<Collider>();
        float sizeZ = borderR - borderL - obCollider.bounds.size.z - 2 * spawnOffset;
        Debug.Log("obCollider.bounds.size.z = " + obCollider.bounds.size.z);

        Transform groundTransform = mapSection.transform.Find("Ground");
        Collider groundCollider = groundTransform?.gameObject.GetComponent<Collider>();
        Debug.Log("sizeZ: " + sizeZ);

        return (mapSection.transform.position, new Vector3(groundCollider.bounds.size.x, 0, sizeZ));
    }

    private void CalculateBorders(GameObject mapSection) {
        Transform wallLTransform = mapSection.transform.Find("Wall_left");
        Collider wallLCollider = wallLTransform?.gameObject.GetComponent<Collider>();
        borderL = wallLCollider.bounds.max.z;

        Transform wallRTransform = mapSection.transform.Find("Wall_right");
        Collider wallRCollider = wallRTransform?.gameObject.GetComponent<Collider>();
        borderR = wallRCollider.bounds.min.z;
    }


    void OnDrawGizmos() {
        // Draw a line to visualize the destroy distance
        Gizmos.color = Color.red;

        (Vector3 center, Vector3 size) = CalculateSpawnArea(mapSectionTest, obstacles[0]);
        Gizmos.DrawWireCube(center, size);
    }
}
