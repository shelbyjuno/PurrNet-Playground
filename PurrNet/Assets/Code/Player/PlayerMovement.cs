using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController controller;
    
    [Header("Settings")]
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float dashSpeed = 5.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float drag = 0.5f;
    [SerializeField] private float gravityValue = -9.81f;

    [SerializeField] private float dashCooldown = 1.5f;
    private float currentDashCooldown = 0;
    
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    void Awake()
    {
        controller.detectCollisions = false;
        currentDashCooldown = dashCooldown;
    }

    void Update()
    {
        currentDashCooldown = Mathf.Min(currentDashCooldown + Time.deltaTime, dashCooldown);

        groundedPlayer = controller.isGrounded;

        if (groundedPlayer && playerVelocity.y < 0)
            playerVelocity.y = 0f;

        // Ground Movement
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        // Localize
        move = transform.TransformDirection(move);

        // Move
        controller.Move(move * Time.deltaTime * playerSpeed);

        // Jump velocity
        if (Input.GetButtonDown("Jump") && groundedPlayer)
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);

        // Dash
        if (currentDashCooldown >= dashCooldown && (Keyboard.current.leftShiftKey.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame))
        {
            currentDashCooldown = 0;
            
            Vector3 dash = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            dash = transform.TransformDirection(dash);

            playerVelocity.x += dash.x * dashSpeed;
            playerVelocity.z += dash.z * dashSpeed;
        }

        // Gravity
        playerVelocity.y += gravityValue * Time.deltaTime;

        // Drag
        playerVelocity.x /= 1 + drag * Time.deltaTime;
        playerVelocity.z /= 1 + drag * Time.deltaTime;

        // Move (velocity)
        controller.Move(playerVelocity * Time.deltaTime);
    }
}
