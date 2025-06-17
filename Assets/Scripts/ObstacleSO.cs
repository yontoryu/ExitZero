using Unity.VisualScripting;
using UnityEngine;

public enum ObstacleType {
    Flat,
    Tall,
    Floating
}

[CreateAssetMenu(fileName = "ObstacleSO", menuName = "Scriptable Objects/ObstacleSO")]
public class ObstacleSO : ScriptableObject {
    public ObstacleType type;
    public float floatingCenterY, floatingSizeY;
    public GameObject body;
    public Vector3 safeZoneSize;
    public int baseDamage;
    public int extraDamage;
}