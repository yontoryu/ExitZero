using UnityEngine;

public class ObstacleCollisionHandler : MonoBehaviour {
    private ObstacleSpawner obstacleSpawner;
    private bool frontTriggered = false;
    private int appliedDamage;
    private int extraDamage;
    private int ID;
    private int sectionID;

    public void Initialize(ObstacleSpawner obstacleSpawner, int sectionID, int ID, int baseDamage, int extraDamage) {
        this.obstacleSpawner = obstacleSpawner;
        this.sectionID = sectionID;
        this.ID = ID;
        appliedDamage = baseDamage;
        this.extraDamage = extraDamage;
    }

    public void NotifyTrigger() {
        frontTriggered = true;
    }

    // void OnCollisionEnter(Collision collision) {
    //     GameObject player = collision.gameObject;

    //     if (collision.gameObject.CompareTag("Player")) {
    //         ContactPoint contact = collision.contacts[0];
    //         Vector3 normal = contact.normal.normalized;

    //         PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
    //         string info = "";

    //         // Frontal Collision
    //         if (normal.x != 0) {
    //             info += "frontal";
    //             // direct frontal collision?
    //             if (frontTriggered) {
    //                 info += " hard";
    //                 appliedDamage += extraDamage;
    //                 obstacleSpawner.SafeDeleteObstacle(gameObject, sectionID, ID);
    //             }
    //             playerHealth.ApplyDamage(appliedDamage);
    //         }
    //         // Lateral Collision
    //         else if (normal.z != 0) {
    //             // check if front triggered
    //             if (frontTriggered) {
    //                 info += "side slight";
    //                 playerHealth.ApplyDamage(appliedDamage);
    //             }
    //         }
    //         // lower collision
    //         else if (normal.y > 0) {
    //             info += "lower";
    //             if (frontTriggered) {
    //                 info += " hard";
    //                 appliedDamage += extraDamage;
    //                 obstacleSpawner.SafeDeleteObstacle(gameObject, sectionID, ID);
    //             }
    //             playerHealth.ApplyDamage(appliedDamage);
    //         }
    //         // upper collision
    //         else if (normal.y < 0) {
    //             if (frontTriggered) {
    //                 playerHealth.ApplyDamage(appliedDamage);
    //                 info += "upper slight";
    //             }
    //         }

    //         Debug.Log(name + ":\t" + info);
    //     }
    // }

    void OnCollisionEnter(Collision collision) {
        GameObject player = collision.gameObject;

        if (collision.gameObject.CompareTag("Player")) {
            ContactPoint contact = collision.contacts[0];
            Vector3 normal = contact.normal.normalized;

            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

            // Frontal Collision
            if (normal.x != 0) {
                appliedDamage += extraDamage;
                playerHealth.ApplyDamage(appliedDamage, ID);
                obstacleSpawner.SafeDeleteObstacle(gameObject, sectionID, ID);
            }
            // lower collision
            else if (normal.y > 0) {
                appliedDamage += extraDamage;
                playerHealth.ApplyDamage(appliedDamage, ID);
                obstacleSpawner.SafeDeleteObstacle(gameObject, sectionID, ID);
            }
        }
    }
}