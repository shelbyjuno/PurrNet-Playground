using System;
using PurrNet;
using UnityEngine;
using UnityEngine.Events;

public class GameStateManager : NetworkBehaviour
{
    public enum GameState { None, Waiting, Starting, Playing, Intermission, Ended }
    public SyncVar<GameState> gameState = new SyncVar<GameState>(GameState.None);
    public UnityEvent<GameState> OnGameStateChanged = new UnityEvent<GameState>();

    [Header("Timers")]
    [SerializeField] float startingTime = 5f;
    private float startingTimer;

    [SerializeField] float gameTime = 300f;
    private float gameTimer;

    [SerializeField] float intermissionTime = 5f;
    private float intermissionTimer;

    [SerializeField] float endGameTime = 5f;
    private float endGameTimer;

    void Awake()
    {
        startingTimer = startingTime;
        gameTimer = gameTime;
        intermissionTimer = intermissionTime;
        endGameTimer = endGameTime;

        gameState.onChanged += (value) => OnGameStateChanged?.Invoke(value);
    }

    protected override void OnSpawned(bool asServer)
    {
        base.OnSpawned(asServer);

        if (!asServer)
            return;

        // Set default game state
        gameState.value = GameState.Waiting;
    }

    void Update()
    {
        if (gameState.value == GameState.Starting)
        {
            startingTimer -= Time.deltaTime;

            if (startingTimer <= 0 && isServer) 
                gameState.value = GameState.Playing;
        }
        else if (gameState.value == GameState.Playing)
        {
            // Reset the intermission timer
            intermissionTimer = intermissionTime;

            gameTimer -= Time.deltaTime;

            if (gameTimer <= 0 && isServer) 
                gameState.value = GameState.Ended;
        }
        else if (gameState.value == GameState.Intermission)
        {
            intermissionTimer -= Time.deltaTime;

            if (intermissionTimer <= 0 && isServer) 
                gameState.value = GameState.Playing;
        }
    }

    public void OnGoalScored(TeamManager.Team team, PlayerID player)
    {
        Debug.Log($"Goal scored by {team} team!");

        gameState.value = GameState.Intermission;
    }

    public void TryStartGame()
    {
        if (!isServer)
            return;
    
        if (gameState.value == GameState.Waiting)
            gameState.value = GameState.Starting;
    }

    public float GetTime()
    {
        switch (gameState.value)
        {
            case GameState.Starting:
                return startingTimer;
            case GameState.Playing:
                return gameTimer;
            case GameState.Intermission:
                return intermissionTimer;
            case GameState.Ended:
                return endGameTimer;
            default:
                return 0;
        }
    }
}
