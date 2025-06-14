using UnityEngine;

public class DisplayObstacleAreas : MonoBehaviour {
    public Bounds spawnArea;
    public Bounds safeZone;

    void OnDrawGizmosSelected() {
        // Draw a line to visualize the destroy distance
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(spawnArea.center, spawnArea.size);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(safeZone.center, safeZone.size);
    }
}
