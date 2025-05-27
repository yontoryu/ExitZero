using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public CharacterController controller;
    public float horizontal_speed = 12f;
    public float jumpHeight = 3f;
    public float gravity = -25f;
    Vector3 velocity;
    bool isGrounded;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public Animator animator;

    string[,] Buttons = { { "Horizontal_1", "Horizontal_2" }, { "Jump_1", "Jump_2" }, { "Slide_1", "Slide_2" }, { "Interact_1", "Interact_2" } };
    string input_horizontal;
    string input_jump;
    string input_slide;
    string input_interact;
    public bool Player1;

    void Start() {
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
        // Move horizontally
        float horizontal = Input.GetAxisRaw(input_horizontal);
        Vector3 horizontal_movement = new Vector3(0f, 0f, horizontal).normalized * horizontal_speed;
        controller.Move(horizontal_movement * Time.deltaTime);

        // Ground Check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (controller.isGrounded && velocity.y < 0) {
            velocity.y = -2f;
        }

        // Jump
        if (Input.GetButtonDown(input_jump) && isGrounded) {
            animator.SetBool("isJumping", true);
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
            Debug.Log("Starting to Jump");
        }
        else if (isGrounded) {
            animator.SetBool("isJumping", false);
            Debug.Log("Grounded");
        }
        else if (!isGrounded) {
            Debug.Log("In the Air");
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

}
