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
    public SyncList<PlayerID> playerIDs = new SyncList<PlayerID>();
    public SyncDictionary<PlayerID, bool> playerReadyStates = new SyncDictionary<PlayerID, bool>();
    public SyncDictionary<PlayerID, string> playerNames = new SyncDictionary<PlayerID, string>();

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
        playerReadyStates[info.sender] = !playerReadyStates[info.sender];        
    }

    [ServerRpc(requireOwnership: false)]
    void CmdSetPlayerName(string name, RPCInfo info = default)
    {
        playerNames[info.sender] = name;
    }

    private void OnPlayerJoined(PlayerID player, SceneID scene, bool asServer)
    {
        if (!asServer || scene.id != 002)
            return;

        playerIDs.Add(player);
        playerReadyStates.Add(player, false);
        playerNames.Add(player, $"Loading ({player.id.ToString()})");
    }

    private void OnPlayerLeft(PlayerID player, SceneID scene, bool asServer)
    {
        if (!asServer || scene.id != 002)
            return;

        playerIDs.Remove(player);
        playerReadyStates.Remove(player);
        playerNames.Remove(player);
    }
}
