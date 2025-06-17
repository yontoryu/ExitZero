using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSO", menuName = "Scriptable Objects/PlayerSO")]
public class PlayerSO : ScriptableObject {
    public float horizontalSpeedMultiplier = 12f;
    public float jumpHeight = 3f;
    public float gravity = -26f;
    public float boostFallMultiplier = 2f;
    public int health = 100;
}
