using PurrNet;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerThrow : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] Transform cameraTransform;
    [SerializeField] GameObject ballModel;
    SyncVar<Ball> currentBall = new();

    [Header("Settings")]
    [SerializeField] float throwPower = 10f;

    void Awake()
    {
        currentBall.onChanged += ball => ballModel.SetActive(ball != null);
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && currentBall.value != null)
        {
            currentBall?.value.TryThrow(this, cameraTransform.position, cameraTransform.forward, throwPower);
            ballModel.SetActive(false);
        }
    }

    public void SetBall(Ball ball)
    {
        if (!isServer)
            return;

        currentBall.value = ball;
    }
}
