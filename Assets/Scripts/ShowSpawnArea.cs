using UnityEngine;

public class ShowSpawnArea : MonoBehaviour {
    public Bounds spawnArea;

    void OnDrawGizmosSelected() {
        // Draw a line to visualize the destroy distance
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(spawnArea.center, spawnArea.size);
    }
}
