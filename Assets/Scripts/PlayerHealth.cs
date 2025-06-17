using UnityEngine;

public class PlayerHealth : MonoBehaviour {
    public PlayerSO playerStats;
    public int health;
    int lastObstacleID;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        health = playerStats.health;
    }

    // Update is called once per frame
    void Update() {

    }

    public void ApplyDamage(int damage, int obstacleID) {
        if (lastObstacleID == obstacleID) return;
        lastObstacleID = obstacleID;
        health -= damage;
    }

    public int GetPlayerHealth() {
        return health;
    }
}
