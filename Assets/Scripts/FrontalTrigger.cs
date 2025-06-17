using UnityEngine;

public class FrontalTrigger : MonoBehaviour {
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {

    }

    // Update is called once per frame
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            ObstacleCollisionHandler handler = GetComponentInParent<ObstacleCollisionHandler>();
            handler.NotifyTrigger();
        }
    }
}
