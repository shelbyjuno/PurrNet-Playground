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

    private PlayerThrow currentOwner;
    private PlayerThrow lastOwner;
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
        if (currentOwner)
        {
            transform.position = currentOwner.transform.position + ballVisualOffset;
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
        if(currentOwner != playerThrow)
            return;
        
        rb.isKinematic = false;

        foreach (var collider in colliders)
            collider.enabled = true;

        transform.position = origin;
        rb.AddForce(direction * force, ForceMode.Impulse);

        currentOwner = null;

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
        if (other.TryGetComponent(out PlayerThrow playerThrow) && currentOwner == null)
        {
            // Don't let the player pick up the ball right away if they threw it
            if (lastOwner == playerThrow && !canBePickedUpByLastOwner)
                return;

            ResetVelocity();

            rb.isKinematic = true;

            foreach (var collider in colliders)
                collider.enabled = false;

            currentOwner = playerThrow;
            lastOwner = playerThrow;
            canBePickedUpByLastOwner = false;

            playerThrow.SetBall(this);

            state.value = BallState.Held;
        }
        else if (other.transform.root.TryGetComponent(out Goal goal) && currentOwner == null)
        {
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

    public PlayerID GetLastOwner() => lastOwner.owner.Value;
}
