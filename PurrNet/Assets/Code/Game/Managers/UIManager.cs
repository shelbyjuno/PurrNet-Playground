using System;
using PurrNet;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] GameStateManager gameStateManager;
    [SerializeField] TeamManager teamManager;
    [SerializeField] TextMeshProUGUI gameStateText; 
    [SerializeField] TextMeshProUGUI scoreText;

    void Awake()
    {
        gameStateManager.gameState.onChanged += OnGameStateChanged;
        teamManager.teamScores.onChanged += OnTeamScoresChanged;
    }

    void Update()
    {
        var timeSpan = TimeSpan.FromSeconds(gameStateManager.GetTime());

        switch (gameStateManager.gameState.value)
        {
            case GameStateManager.GameState.Waiting:
                gameStateText.text = "Waiting for players...";
                break;
            case GameStateManager.GameState.Starting:
                gameStateText.text = string.Format("Starting in: {0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
                break;
            case GameStateManager.GameState.Playing:
                gameStateText.text = string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
                break;
            case GameStateManager.GameState.Ended:
                gameStateText.text = "Game over!";
                break;
        }
    }

    private void OnTeamScoresChanged(SyncDictionaryChange<TeamManager.Team, int> change)
    {
        scoreText.SetText($"<Color=red>{teamManager.GetTeamScore(TeamManager.Team.Red)}</color> : <color=lightblue>{teamManager.GetTeamScore(TeamManager.Team.Blue)}</color>");
    }

    private void OnGameStateChanged(GameStateManager.GameState state)
    {

    }
}
