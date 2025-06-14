using Unity.VisualScripting;
using UnityEngine;

public enum ObstacleType {
    Flat,
    Tall,
    Floating
}

[CreateAssetMenu(fileName = "Obstacle", menuName = "Scriptable Objects/Obstacle")]
public class Obstacle : ScriptableObject {
    public ObstacleType type;
    public float floatingCenterY, floatingSizeY;
    public GameObject body;
    public Vector3 safeZoneSize;
}