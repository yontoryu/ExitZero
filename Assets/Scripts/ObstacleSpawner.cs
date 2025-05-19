using UnityEngine;

public class ObstacleSpawner : MonoBehaviour {
    public float spawnRate;
    public float spawnRateRandomness;
    public GameObject[] players;
    public GameObject MapSection;
    public float spawnRandomness;
    public GameObject[] obstacles;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        if (spawnRate < 1) {
            spawnRate = 1;
        }
        if (spawnRandomness < 0) {
            spawnRandomness = 0;
        }
    }

    // Update is called once per frame
    void Update() {

    }

    void GeneratePlayerPaths(GameObject player) {

    }
}
