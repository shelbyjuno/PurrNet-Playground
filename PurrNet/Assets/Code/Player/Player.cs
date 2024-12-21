using System;
using PurrNet;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private CharacterController characterController;

    private Vector3 spawnPos;

    void Awake()
    {
        GameManager.Instance.GameStateManager.OnGameStateChanged.AddListener(OnGameStateChanged);

        spawnPos = transform.position;

        movement.enabled = false;
    }

    void Update()
    {
        if (!isOwner)
            return;
        var gameState = GameManager.Instance.GameStateManager.gameState.value;
        movement.enabled = gameState == GameStateManager.GameState.Playing;
    }

    private void OnGameStateChanged(GameStateManager.GameState state)
    {
        if (!isOwner)
            return;

        switch (state)
        {
            case GameStateManager.GameState.Waiting:
                movement.enabled = false;
                break;
            case GameStateManager.GameState.Starting:
                movement.enabled = false;
                break;
            case GameStateManager.GameState.Playing:
                characterController.enabled = true;
                movement.enabled = true;
                break;
            case GameStateManager.GameState.Intermission:
                characterController.enabled = false;
                movement.enabled = false;
                transform.position = spawnPos;
                break;
            case GameStateManager.GameState.Ended:
                movement.enabled = false;
                break;
            default:
                break;
        }
    }
}
