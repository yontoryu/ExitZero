using System;
using UnityEngine;

public class PlayerMovementRB : MonoBehaviour {
    public float horizontal_speed = 12f;
    public float jumpHeight = 3f;
    public float gravity = -25f;
    public float boostedFallSpeed = 2f; // Gravity applied when jumping to sliding
    bool boostFallSpeed = false;

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

        bool wasGrounded = isGrounded;

        // Ground Check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && !wasGrounded) {
            animator.SetBool("isJumping", false);
            animator.SetBool("jumpToSlide", false); // Reset jump to slide state
            Debug.Log("Grounded");
        }

        // If Player is grounded and jump button is pressed, start jumping
        if (Input.GetButtonDown(input_jump) && isGrounded) {
            animator.SetBool("isJumping", true);
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            Debug.Log("Jumping");
        }

        // Apply gravity
        if (!isGrounded && !boostFallSpeed) {
            velocity.y += gravity * Time.deltaTime;
        }
        else if (!isGrounded && boostFallSpeed) {
            velocity.y = Mathf.Lerp(velocity.y, -Mathf.Abs(boostedFallSpeed), Time.deltaTime * 10f);
        }
        else if (isGrounded && boostFallSpeed) {
            boostFallSpeed = false; // Reset boost fall speed when grounded
            velocity.y = -2f; // Reset vertical velocity when grounded
        }
        else if (velocity.y < 0) {
            velocity.y = -2f;
        }

        // If the player is not grounded and the slide button is pressed, apply boosted fall speed
        if (Input.GetButtonDown(input_slide) && !isGrounded) {
            velocity.y = Mathf.Lerp(velocity.y, -Mathf.Abs(boostedFallSpeed), Time.deltaTime * 10f);
            boostFallSpeed = true;
            animator.SetBool("jumpToSlide", true); // Reset jump state
            animator.SetBool("isJumping", false); // Reset jump state
            animator.SetTrigger("slide");
            Debug.Log("Jumping to Sliding");
        }
        // If the player is grounded and the slide button is pressed, slide
        else if (Input.GetButtonDown(input_slide)) {
            animator.SetTrigger("slide");
            Debug.Log("Sliding");
        }

        // Apply movement
        Vector3 move = new Vector3(0f, velocity.y, horizontalInput * horizontal_speed);
        rb.linearVelocity = new Vector3(move.x, move.y, move.z);  // Overwrite Rigidbody velocity directly
        //Debug.Log("jump: " + animator.GetCurrentAnimatorStateInfo(0).IsName("Jump") + "\nslide: " + animator.GetCurrentAnimatorStateInfo(0).IsName("Slide") + "\tvelocity.y: " + rb.linearVelocity.y);
    }

    void OnDrawGizmosSelected() {
        if (groundCheck != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}