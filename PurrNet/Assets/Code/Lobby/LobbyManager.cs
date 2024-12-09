using System;
using System.Collections.Generic;
using System.Linq;
using PurrNet;
using UnityEngine;

public struct PlayerLobbyData
{
    public PlayerID playerID;
    public bool isReady;
    public string name;
}

public class LobbyManager : NetworkBehaviour
{
    [SerializeField] string gameSceneName = "Game";

    public SyncDictionary<PlayerID, PlayerLobbyData> playerLobbyData = new();

    protected override void OnSpawned(bool asServer)
    {
        base.OnSpawned(asServer);

        if (asServer)
            return;

        CmdSetPlayerName($"Player {localPlayer}");        
    }

    protected override void OnInitializeModules()
    {
        base.OnInitializeModules();

        networkManager.scenePlayersModule.onPlayerLoadedScene += OnPlayerJoined;
        networkManager.scenePlayersModule.onPlayerUnloadedScene += OnPlayerLeft;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        networkManager.scenePlayersModule.onPlayerLoadedScene -= OnPlayerJoined;
        networkManager.scenePlayersModule.onPlayerUnloadedScene -= OnPlayerLeft;
    }

    public void SetReadyState()
    {
        CmdSetReadyState();
    }

    public void TryStartGame()
    {
        // Only let the host start the game if all players are ready
        if (!networkManager.isServer || !AllPlayersReady())
            return;

        networkManager.sceneModule.LoadSceneAsync(gameSceneName);
    }

    [ServerRpc(requireOwnership: false)]
    void CmdSetReadyState(RPCInfo info = default)
    {
        PlayerLobbyData oldData = playerLobbyData[info.sender];

        AddOrSetPlayerLobbyData(info.sender, new PlayerLobbyData
        {
            playerID = oldData.playerID,
            isReady = !oldData.isReady,
            name = oldData.name
        });
    }

    [ServerRpc(requireOwnership: false)]
    void CmdSetPlayerName(string name, RPCInfo info = default)
    {
        PlayerLobbyData oldData = playerLobbyData[info.sender];

        AddOrSetPlayerLobbyData(info.sender, new PlayerLobbyData
        {
            playerID = oldData.playerID,
            isReady = oldData.isReady,
            name = name
        });
    }

    private void OnPlayerJoined(PlayerID player, SceneID scene, bool asServer)
    {
        // 001 is the LobbyScene index
        if (!asServer || scene.id != 001)
            return;

        PlayerLobbyData data = new PlayerLobbyData
        {
            playerID = player,
            isReady = false,
            name = $"Loading ({player.id.ToString()})"
        };

        AddOrSetPlayerLobbyData(player, data);
    }

    private void OnPlayerLeft(PlayerID player, SceneID scene, bool asServer)
    {
        // 002 is the LobbyScene index
        if (!asServer || scene.id != 001)
            return;

        playerLobbyData.Remove(player);
    }

    public void AddOrSetPlayerLobbyData(PlayerID player, PlayerLobbyData data)
    {
        if (playerLobbyData.ContainsKey(player))
            playerLobbyData[player] = data;
        else
            playerLobbyData.Add(player, data);
    }

    public bool AllPlayersReady() => playerLobbyData.All(x => x.Value.isReady);
}
