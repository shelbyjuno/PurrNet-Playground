using System;
using System.Collections.Generic;
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
    public SyncDictionary<PlayerID, PlayerLobbyData> playerLobbyData = new();

    protected override void OnSpawned(bool asServer)
    {
        base.OnSpawned(asServer);

        if (asServer)
            return;

        CmdSetPlayerName($"Shelby {localPlayer}");        
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
        // 002 is the LobbyScene index + 1
        if (!asServer || scene.id != 002)
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
        // 002 is the LobbyScene index + 1
        if (!asServer || scene.id != 002)
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
}
