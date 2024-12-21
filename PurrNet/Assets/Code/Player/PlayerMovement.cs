using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController controller;
    
    [Header("Settings")]
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;
    
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    void Awake()
    {
        controller.detectCollisions = false;
    }

    void Update()
    {
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

        // Gravity
        playerVelocity.y += gravityValue * Time.deltaTime;

        // Localize
        playerVelocity = transform.TransformDirection(playerVelocity);

        // Move (velocity)
        controller.Move(playerVelocity * Time.deltaTime);
    }
}
