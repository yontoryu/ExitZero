using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public CharacterController controller;
    public float speed = 12f;
    public float jumpHeight = 3f;
    public float gravity = -9.81f;
    Vector3 velocity;
    bool isGrounded;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    // Update is called once per frame
    void Update() {
        float horizontal = Input.GetAxisRaw("Horizontal");
        Vector3 direction = new Vector3(0f, 0f, horizontal).normalized;

        if (direction.magnitude >= 0.1f) {
            controller.Move(direction.normalized * speed * Time.deltaTime);
        }

        //jump

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0) {
            velocity.y = -2f;
        }

        if (Input.GetButtonDown("Jump") && isGrounded) {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        //gravity

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

    }
}
