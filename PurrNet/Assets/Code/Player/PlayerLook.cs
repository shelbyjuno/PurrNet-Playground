using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] float sensitivity = 1;
    private const float Y_ANGLE_MIN = -90.0f;
    private const float Y_ANGLE_MAX = 90.0f;

    public Transform playerTransform;
    public Transform playerCamera;

    private float currentX;
    private float currentY;

    private bool mouseLocked = true;

    private void Awake()
    {
        currentX = playerTransform.rotation.eulerAngles.y;
        currentY = playerCamera.rotation.eulerAngles.x;
    }
    private void Update()
    {
        Cursor.lockState = mouseLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !mouseLocked;

        if(mouseLocked) HandleRotation();        
    }

    private void HandleRotation()
    {
        // Get the mouse axis movement on both x and y axis
        currentX += Input.GetAxis("Mouse X") * sensitivity;
        currentY += Input.GetAxis("Mouse Y") * sensitivity;

        // Clamp the angle of rotation around player
        currentY = Mathf.Clamp(currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);
    }

    private void LateUpdate()
    {                
        playerTransform.rotation = Quaternion.Euler(0, currentX, 0);
        playerCamera.localRotation = Quaternion.Euler(-currentY, 0, 0);
    }
    
    public void ResetCameraRotation()
    {
        currentX = 0;
        currentY = 0;
    }
}
