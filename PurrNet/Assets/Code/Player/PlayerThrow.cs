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

    protected override void OnSpawned(bool asServer)
    {
        base.OnSpawned(asServer);

        Team team = GameManager.Instance.TeamManager.GetPlayerTeam(owner.Value);
        
        ballModel.transform.GetChild(0).gameObject.SetActive(team == Team.Red);
        ballModel.transform.GetChild(1).gameObject.SetActive(team == Team.Blue);
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
