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
    [HideInInspector] public SyncTimer startingTimer = new SyncTimer();

    [SerializeField] float gameTime = 300f;
    [HideInInspector] public SyncTimer gameTimer = new SyncTimer();

    [SerializeField] float intermissionTime = 5f;
    [HideInInspector] public SyncTimer intermissionTimer = new SyncTimer();

    [SerializeField] float endGameTime = 5f;
    [HideInInspector] public SyncTimer endGameTimer = new SyncTimer();

    void Awake()
    {
        gameState.onChanged += GameStateChanged;

        startingTimer.onTimerEnd += OnStartTimerEnd;
        gameTimer.onTimerEnd += OnGameTimerEnd;
        intermissionTimer.onTimerEnd += OnIntermissionTimerEnd;
        endGameTimer.onTimerEnd += OnEndGameTimerEnd;
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

    }

    private void GameStateChanged(GameState state)
    {
        OnGameStateChanged.Invoke(state);

        if (!isServer)
            return;

        switch (state)
        {
            case GameState.Starting:
                startingTimer.StartTimer(startingTime);
                break;
            case GameState.Playing:
                gameTimer.StartTimer(gameTime);
                break;
            case GameState.Intermission:
                intermissionTimer.StartTimer(intermissionTime);
                break;
            case GameState.Ended:
                endGameTimer.StartTimer(endGameTime);
                break;
        }
    }

    private void OnStartTimerEnd()
    {
        if (!isServer)
            return;

        gameState.value = GameState.Playing;
    }

    private void OnIntermissionTimerEnd()
    {
        if (!isServer)
            return;

        gameState.value = GameState.Playing;
    }

    private void OnGameTimerEnd()
    {
        if (!isServer)
            return;

        gameState.value = GameState.Ended;
    }

    private void OnEndGameTimerEnd()
    {
        if (!isServer)
            return;

        gameState.value = GameState.Waiting;
    }

    public void OnGoalScored(Team team, PlayerID player)
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
                return startingTimer.remaining;
            case GameState.Playing:
                return gameTimer.remaining;
            case GameState.Intermission:
                return intermissionTimer.remaining;
            case GameState.Ended:
                return endGameTimer.remaining;
            default:
                return 0;
        }
    }
}
