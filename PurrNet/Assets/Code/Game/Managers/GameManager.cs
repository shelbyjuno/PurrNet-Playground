using System;
using System.Collections;
using System.Linq;
using PurrNet;
using PurrNet.Modules;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : NetworkBehaviour
{
    [Header("References")]
    public GameStateManager GameStateManager;
    public TeamManager TeamManager;
    public SpawnManager SpawnManager;

    [Header("Events")]
    public UnityEvent<PlayerID> OnPlayerJoined = new UnityEvent<PlayerID>();
    public UnityEvent<PlayerID> OnPlayerLeft = new UnityEvent<PlayerID>();
    public UnityEvent<PlayerID[]> AllPlayersConnected = new UnityEvent<PlayerID[]>();

    public static GameManager Instance { get; private set; }

    private int loadedPlayers = 0;

    void Awake()
    {
        Instance = this;
    }

    protected override void OnSpawned(bool asServer)
    {
        base.OnSpawned(asServer);

        if(!asServer || !isServer)
            return;

        networkManager.scenePlayersModule.onPlayerLoadedScene += OnPlayerLoadedScene;
        networkManager.scenePlayersModule.onPlayerUnloadedScene += OnPlayerUnloadedScene;
    }

    protected override void OnDespawned(bool asServer)
    {
        base.OnDespawned(asServer);

        if(!asServer || !isServer)
            return;

        networkManager.scenePlayersModule.onPlayerLoadedScene -= OnPlayerLoadedScene;
        networkManager.scenePlayersModule.onPlayerUnloadedScene -= OnPlayerUnloadedScene;
    }

    private void OnPlayerLoadedScene(PlayerID player, SceneID scene, bool asServer)
    {   
        if (!asServer ||!networkManager.sceneModule.TryGetSceneID(gameObject.scene, out var sceneID) || sceneID != scene)
            return;

        loadedPlayers++;

        OnPlayerJoined?.Invoke(player);

        if (loadedPlayers >= networkManager.playerCount && loadedPlayers >= 2)
            AllPlayersConnected?.Invoke(GetAllPlayers());
    }


    private void OnPlayerUnloadedScene(PlayerID player, SceneID scene, bool asServer)
    {
        if (!asServer ||!networkManager.sceneModule.TryGetSceneID(gameObject.scene, out var sceneID) || sceneID != scene)
            return;

        loadedPlayers--;

        OnPlayerLeft?.Invoke(player);
    }

    public PlayerID[] GetAllPlayers() => networkManager.players.ToArray();
}
