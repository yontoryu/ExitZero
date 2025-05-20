using UnityEngine;

public class testscirpt : MonoBehaviour {
    CharacterController controller;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update() {
        if (controller.isGrounded) {
            Debug.Log("GROUND");
        }
        else {
            Debug.Log("AIR");
        }
    }
}
