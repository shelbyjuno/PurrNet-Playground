using System;
using PurrNet;
using PurrNet.Modules;
using UnityEngine;

public enum Team { Red, Blue }

public class TeamManager : NetworkBehaviour
{
    public SyncDictionary<PlayerID, Team> playerTeams = new SyncDictionary<PlayerID, Team>();
    public SyncDictionary<Team, int> teamScores = new SyncDictionary<Team, int>();

    protected override void OnSpawned(bool asServer)
    {
        base.OnSpawned(asServer);

        if (!isServer)
            return;

        teamScores.Clear();

        teamScores.Add(Team.Red, 0);
        teamScores.Add(Team.Blue, 0);
    }

    public void OnPlayerJoined(PlayerID player)
    {
        if (!isServer)
            return;

        playerTeams.Add(player, playerTeams.Count % 2 == 0 ? Team.Red : Team.Blue);
    }

    public void OnPlayerLeft(PlayerID player)
    {
        if (!isServer)
            return;

        playerTeams.Remove(player);
    }

    public void OnGoalScored(Team team, PlayerID player)
    {
        if (!isServer)
            return;

        if (team == Team.Red)
            teamScores[Team.Blue]++;
        else
            teamScores[Team.Red]++;
    }

    public Team GetPlayerTeam(PlayerID player)
    {
        if (playerTeams.TryGetValue(player, out var team))
            return team;

        Debug.LogError($"Player {player} is not in the playerTeams dictionary.");
        return Team.Red;
    }

    public int GetTeamScore(Team team)
    {
        if (teamScores.TryGetValue(team, out var score))
            return score;

        return 0;
    }
}
