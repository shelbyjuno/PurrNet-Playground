using System;
using PurrNet;
using UnityEngine;

public class SpawnManager : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] GameObject playerPrefab;
    [SerializeField] Ball ballPrefab;

    [SerializeField] GameManager gameManager;
    [SerializeField] GameStateManager gameStateManager;
    [SerializeField] TeamManager teamManager;

    [Header("Settings")]
    [SerializeField] Vector3 ballSpawnPos = new Vector3(0, 1f, 0);

    Ball currentBall;

    public void OnGameStateChanged(GameStateManager.GameState state)
    {
        if (!isServer)
            return;

        switch (state)
        {
            case GameStateManager.GameState.Waiting:
                break;
            case GameStateManager.GameState.Starting:
                SpawnPlayers();
                break;
            case GameStateManager.GameState.Playing:
                SpawnBall();
                break;
            case GameStateManager.GameState.Intermission:
                DespawnBall();
                break;
            case GameStateManager.GameState.Ended:
                DespawnBall();
                break;
            default:
                break;
        }
    }

    private void SpawnPlayers()
    {
        foreach (PlayerID playerID in gameManager.GetAllPlayers())
        {
            Team team = teamManager.GetPlayerTeam(playerID);

            var teamSpawnSide = team == Team.Red ? -11.5f : 11.5f;
            var spawnPos = new Vector3(UnityEngine.Random.Range(-5, 5), 1.5f, teamSpawnSide);
            var center = new Vector3(0, 1f, 0);
    
            var p = Instantiate(playerPrefab, spawnPos, Quaternion.LookRotation(center - spawnPos));
            p.GetComponent<NetworkIdentity>().GiveOwnership(playerID);
        }
    }

    private void SpawnBall()
    {
        if (currentBall != null)
            DespawnBall();
        currentBall = Instantiate(ballPrefab, ballSpawnPos, Quaternion.identity);
    }

    private void DespawnBall()
    {
        Destroy(currentBall.gameObject);
    }
}
