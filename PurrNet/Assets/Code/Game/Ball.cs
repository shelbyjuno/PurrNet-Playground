using System;
using PurrNet;
using UnityEngine;

public class Ball : NetworkBehaviour
{
    enum BallState { Grounded, Held }

    [Header("References")]
    [SerializeField] MeshRenderer[] meshRenderers;
    [SerializeField] Collider[] colliders;
    [SerializeField] Rigidbody rb;

    [Header("Settings")]
    [SerializeField] Vector3 spawnPosition = Vector3.zero;
    [SerializeField] Vector3 ballVisualOffset;

    SyncVar<BallState> state = new SyncVar<BallState>(BallState.Grounded);

    private SyncVar<PlayerThrow> currentOwner = new SyncVar<PlayerThrow>();
    private SyncVar<PlayerThrow> lastOwner = new SyncVar<PlayerThrow>();
    private bool canBePickedUpByLastOwner;

    void Awake()
    {
        state.onChanged += OnStateChanged;
    }

    protected override void OnSpawned()
    {
        base.OnSpawned();

        foreach (var collider in colliders)
            collider.enabled = isServer;

        rb.isKinematic = !isServer;
    }

    void Update()
    {
        // If the ball is owned by a player, hover it over them
        if (currentOwner.value)
        {
            transform.position = currentOwner.value.transform.position + ballVisualOffset;
        }
    }

    private void OnStateChanged(BallState state)
    {
        switch (state)
        {
            case BallState.Grounded:
                // Enable mesh renderers
                foreach (var mesh in meshRenderers)
                    mesh.enabled = true;
                // Set the ball to the correct team color
                Team team = GameManager.Instance.TeamManager.GetPlayerTeam(lastOwner.value.owner.Value);
                meshRenderers[0].gameObject.SetActive(team == Team.Red);
                meshRenderers[1].gameObject.SetActive(team == Team.Blue);
                break;
            case BallState.Held:
                // Disable mesh renderers
                foreach (var mesh in meshRenderers)
                    mesh.enabled = false;
                break;
        }
    }

    public void TryThrow(PlayerThrow playerThrow, Vector3 origin, Vector3 direction, float force)
    {
        CmdThrow(playerThrow, origin, direction, force);
    }

    [ServerRpc(requireOwnership: false)]
    void CmdThrow(PlayerThrow playerThrow, Vector3 origin, Vector3 direction, float force)
    {
        if(currentOwner.value!= playerThrow)
            return;
        
        rb.isKinematic = false;

        foreach (var collider in colliders)
            collider.enabled = true;

        transform.position = origin;
        rb.AddForce(direction * force, ForceMode.Impulse);

        currentOwner.value = null;

        playerThrow.SetBall(null);

        state.value = BallState.Grounded;
    }

    void OnCollisionEnter(Collision other)
    {
        if (!isServer) return;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isServer) return;

        // Check for player
        if (other.TryGetComponent(out PlayerThrow playerThrow) && currentOwner.value == null && other.CompareTag("Player"))
        {
            // Don't let the player pick up the ball right away if they threw it
            if (lastOwner.value == playerThrow && !canBePickedUpByLastOwner)
                return;

            ResetVelocity();

            rb.isKinematic = true;

            foreach (var collider in colliders)
                collider.enabled = false;

            currentOwner.value= playerThrow;
            lastOwner.value = playerThrow;
            canBePickedUpByLastOwner = false;

            playerThrow.SetBall(this);

            state.value = BallState.Held;
        }
        else if (other.transform.root.TryGetComponent(out Goal goal) && currentOwner.value== null && other.CompareTag("Goal"))
        {
            ResetVelocity();

            foreach (var collider in colliders)
                collider.enabled = false;

            goal.Score(this);
        }
        else
        {
            canBePickedUpByLastOwner = true;
        }
    }

    void ResetVelocity()
    {
        if (!isServer) return;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public PlayerID GetLastOwner() => lastOwner.value.owner.Value;
}
