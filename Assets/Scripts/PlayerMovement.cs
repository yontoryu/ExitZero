using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.Subsystems;
using Vector3 = UnityEngine.Vector3;

public enum PlayerID {
    None,
    Player1,
    Player2
}

public class PlayerMovement : MonoBehaviour {

    [Header("Movement Settings")]
    private float horizontalSpeed = 0f;
    private bool boostFall = false;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private bool isGrounded;
    private bool addJumpForce;

    [Header("References")]
    private Animator animator;
    private Rigidbody rb;

    [Header("Player Settings")]
    public PlayerSO playerStats;
    public PlayerID playerID;
    private string inputJump;
    private string inputSlide;
    private string inputHorizontal;

    void Awake() {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        AssignInputStrings();
    }

    private void AssignInputStrings() {
        string prefix = "_" + (playerID == PlayerID.Player1 ? "1" : "2");
        inputHorizontal = "Horizontal" + prefix;
        inputJump = "Jump" + prefix;
        inputSlide = "Slide" + prefix;
    }

    void Update() {
        /* ------------------------------ Handle Inputs ----------------------------- */
        // if player is grounded and jump button is pressed
        if (Input.GetButtonDown(inputJump) && isGrounded) {
            animator.SetBool("isJumping", true); // start the jump animation
            addJumpForce = true;
            // velocity.y = Mathf.Sqrt(playerStats.jumpHeight * -2f * playerStats.gravity); // set the velocity to sqrt(2 * g * h) (formula for a jump)
        }

        // if the slide button is pressed
        if (Input.GetButtonDown(inputSlide)) {
            // if the player is grounded
            if (isGrounded) {
                animator.SetTrigger("slide"); // start the slide animation
            }
            else { // if the player is in the air
                boostFall = true; // apply the boost for falling faster when jumping and wanting to slide
                animator.SetBool("jumpToSlide", true); // Reset jump state
                animator.SetBool("isJumping", false); // Reset jump state
                animator.SetTrigger("slide"); // start the slide animation
            }
        }

        horizontalSpeed = Input.GetAxisRaw(inputHorizontal);
        /* --------------------------------------------------------------------------- */
    }

    void FixedUpdate() {
        GroundCheck();
        ApplyVerticalMovement();
        ApplyHorizontalMovement();
    }

    private void GroundCheck() {
        bool wasGrounded = isGrounded;

        if (Physics.Raycast(groundCheck.position, Vector3.down, out RaycastHit hit, groundDistance)) {
            isGrounded = true;
        }
        else {
            isGrounded = false;
        }

        // Spieler ist gerade gelandet
        if (isGrounded && !wasGrounded) {
            // Debug.Log("Grounded!");
            animator.SetBool("isJumping", false);
            animator.SetBool("jumpToSlide", false);
        }
    }

    private void ApplyVerticalMovement() {
        if (addJumpForce) {
            rb.AddForce(Vector3.up * Mathf.Sqrt(playerStats.jumpHeight * -2f * playerStats.gravity), ForceMode.VelocityChange);
            addJumpForce = false;
        }
        if (!isGrounded) {
            if (boostFall) {
                rb.AddForce(Vector3.up * playerStats.gravity * playerStats.boostFallMultiplier, ForceMode.Acceleration);
            }
            else {
                rb.AddForce(Vector3.up * playerStats.gravity, ForceMode.Acceleration);
            }
        }
        else {
            boostFall = false;
        }
    }

    private float v(float t, float vStart, float vMax, float alpha = 0.05f) {
        return (t > 300) ? vMax : alpha * t + vStart;
    }

    private void ApplyHorizontalMovement() {
        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 targetVelocity = new Vector3(currentVelocity.x, currentVelocity.y, horizontalSpeed * playerStats.horizontalSpeedMultiplier);
        Vector3 deltaVelocity = new Vector3(0, 0, targetVelocity.z - currentVelocity.z);
        // Debug.Log("deltaVelocity = " + deltaVelocity);

        rb.AddForce(deltaVelocity, ForceMode.VelocityChange);
    }

    void OnDrawGizmosSelected() {
        if (groundCheck != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundDistance);
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