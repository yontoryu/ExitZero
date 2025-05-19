using System;
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

    string[,] Buttons = { { "Horizontal_1", "Horizontal_2" }, { "Jump_1", "Jump_2" } };
    string input_axis;
    string input_jump;
    string input_interact;
    public bool Player1;

    void Start() {
        if (Player1) {
            input_axis = Buttons[0, 0];
            input_jump = Buttons[1, 0];
        }
        else {
            input_axis = Buttons[0, 1];
            input_jump = Buttons[1, 1];
        }
    }

    void Update() {
        float horizontal = Input.GetAxisRaw(input_axis);
        Vector3 direction = new Vector3(0f, 0f, horizontal).normalized;

        if (direction.magnitude >= 0.1f) {
            Console.WriteLine("HORIZONTALLLLL, player1:" + Player1);
            controller.Move(direction.normalized * speed * Time.deltaTime);
        }

        //jump

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0) {
            velocity.y = -2f;
        }

        if (Input.GetButtonDown(input_jump) && isGrounded) {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        //gravity

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

    }
}
