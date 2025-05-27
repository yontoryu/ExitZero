using System;
using UnityEngine;

public class PlayerMovementRB : MonoBehaviour {
    public float horizontal_speed = 12f;
    public float jumpHeight = 3f;
    public float gravity = -25f;
    public float jumpToSlideGravityFactor = 2f; // Gravity applied when jumping to sliding

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public Animator animator;

    private Rigidbody rb;
    private Vector3 velocity;
    private bool isGrounded;

    string[,] Buttons = { { "Horizontal_1", "Horizontal_2" }, { "Jump_1", "Jump_2" }, { "Slide_1", "Slide_2" }, { "Interact_1", "Interact_2" } };
    string input_horizontal;
    string input_jump;
    string input_slide;
    string input_interact;
    public bool Player1;

    void Start() {
        rb = GetComponent<Rigidbody>();

        if (Player1) {
            input_horizontal = Buttons[0, 0];
            input_jump = Buttons[1, 0];
            input_slide = Buttons[2, 0];
        }
        else {
            input_horizontal = Buttons[0, 1];
            input_jump = Buttons[1, 1];
            input_slide = Buttons[2, 1];
        }
    }

    void Update() {
        // Input
        float horizontalInput = Input.GetAxisRaw(input_horizontal);

        // Ground Check
        bool wasGrounded = isGrounded;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && !wasGrounded) {
            // animator.ResetTrigger("jump");
            Debug.Log("Grounded");
        }

        // Jump
        if (Input.GetButtonDown(input_jump) && isGrounded) {
            animator.SetTrigger("jump");
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            Debug.Log("Jumping");
        }

        // Apply gravity
        if (!isGrounded) {
            velocity.y += gravity * Time.deltaTime;
        }
        else if (velocity.y < 0f) {
            velocity.y = -2f;
        }

        // Slide (stub for animation and later collider change)
        if (Input.GetButtonDown(input_slide) && !isGrounded) {
            velocity.y += gravity * Time.deltaTime * jumpToSlideGravityFactor;
            animator.SetTrigger("slide");
            Debug.Log("Jumping to Sliding");
            // You’ll add collider resize here later
        }
        else if (Input.GetButtonDown(input_slide)) {
            animator.SetTrigger("slide");
            Debug.Log("Sliding");
        }

        // Apply horizontal movement and gravity
        Vector3 move = new Vector3(0f, velocity.y, horizontalInput * horizontal_speed);
        rb.linearVelocity = new Vector3(move.x, move.y, move.z);  // Overwrite Rigidbody velocity directly
        Debug.Log("jump: " + animator.GetCurrentAnimatorStateInfo(0).IsName("Jump") + "\nslide: " + animator.GetCurrentAnimatorStateInfo(0).IsName("Slide") + "\tvelocity.y: " + rb.linearVelocity.y);
    }

    void OnDrawGizmosSelected() {
        if (groundCheck != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}