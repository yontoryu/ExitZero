using System;
using UnityEngine;
using UnityEngine.Subsystems;

public class PlayerMovementRB : MonoBehaviour {
    [Header("Movement Settings")]
    public float horizontalSpeed = 12f;
    public float jumpHeight = 3f;
    public float gravity = -25f;
    public float boostedFallSpeed = 2f;
    private bool boostFall = false;
    private Vector3 velocity;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private bool isGrounded;

    [Header("References")]
    private Animator animator;
    private Rigidbody rb;

    [Header("Player Input Mapping")]
    public bool isPlayer1;
    private string inputJump;
    private string inputSlide;
    private string inputHorizontal;

    // public float highestPoint = 0f; // Used to track the highest point reached during the jump
    // bool isJumping = false; // Used to track if the player is currently jumping
    // public GameObject torso;

    void Awake() {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        AssignInputStrings();
    }

    private void AssignInputStrings() {
        string prefix = isPlayer1 ? "_1" : "_2";
        inputHorizontal = "Horizontal" + prefix;
        inputJump = "Jump" + prefix;
        inputSlide = "Slide" + prefix;
    }

    void Update() {
        CheckGrounded();

        /* ------------------------------ Handle Inputs ----------------------------- */
        // if player is grounded and jump button is pressed
        if (Input.GetButtonDown(inputJump) && isGrounded) {
            animator.SetBool("isJumping", true); // start the jump animation
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // set the velocity to sqrt(2 * g * h) (formula for a jump)
            Debug.Log("Jumping");
            // isJumping = true; // Set jumping state to true
            // highestPoint = 0f;
        }

        // if the slide button is pressed
        if (Input.GetButtonDown(inputSlide)) {
            // if the player is grounded
            if (isGrounded) {
                animator.SetTrigger("slide"); // start the slide animation
                Debug.Log("Sliding");
            }
            else { // if the player is in the air
                boostFall = true; // apply the boost for falling faster when jumping and wanting to slide
                animator.SetBool("jumpToSlide", true); // Reset jump state
                animator.SetBool("isJumping", false); // Reset jump state
                animator.SetTrigger("slide"); // start the slide animation
                Debug.Log("Jumping to Sliding");
            }
        }
        /* --------------------------------------------------------------------------- */
    }

    void FixedUpdate() {
        ApplyGravity();
        ApplyMovement();
        // if (isJumping) {
        //     Collider torsoCollider = torso.GetComponent<Collider>();
        //     // Check if the player has reached the highest point during the jump
        //     if (torsoCollider.bounds.max.y > highestPoint) {
        //         highestPoint = torsoCollider.bounds.max.y; // Update the highest point
        //     }
        // }
    }

    private void CheckGrounded() {
        bool wasGrounded = isGrounded; // save the previous grounded state
        // Check if the player is grounded using a sphere check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // if the player was not grounded and is now grounded, reset the jump and jumpToSlide states
        if (isGrounded && !wasGrounded) {
            animator.SetBool("isJumping", false);
            animator.SetBool("jumpToSlide", false);
        }
    }

    private void ApplyGravity() {
        if (!isGrounded) {
            if (boostFall) {
                velocity.y = Mathf.Lerp(velocity.y, -Mathf.Abs(boostedFallSpeed), Time.deltaTime * 10f);
            }
            else {
                velocity.y += gravity * Time.deltaTime;
            }
        }
        else {
            boostFall = false;
            if (velocity.y < 0) velocity.y = -2f;
        }
    }

    private void ApplyMovement() {
        float horizontalInput = Input.GetAxisRaw(inputHorizontal);
        Vector3 move = new Vector3(0f, velocity.y, horizontalInput * horizontalSpeed);
        rb.linearVelocity = move;
    }

    void OnDrawGizmosSelected() {
        if (groundCheck != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }

        Collider[] colliders = GetComponentsInChildren<Collider>();
        if (colliders != null) {
            Gizmos.color = Color.blue;


            foreach (Collider collider in colliders) {
                Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
            }
        }
    }
}