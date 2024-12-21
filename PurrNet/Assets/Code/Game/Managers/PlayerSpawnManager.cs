using System;
using UnityEngine;
using PurrNet;
using PurrNet.Modules;
using System.Collections.Generic;

public class PlayerSpawnManager : PurrMonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    Dictionary<PlayerID, GameObject> spawnedPlayers = new Dictionary<PlayerID, GameObject>();

    public override void Subscribe(NetworkManager manager, bool asServer)
    {
        if (!asServer)
            return;

        manager.scenePlayersModule.onPlayerLoadedScene += OnPlayerLoadedScene;
    }

    public override void Unsubscribe(NetworkManager manager, bool asServer)
    {
        if (!asServer)
            return;

        manager.scenePlayersModule.onPlayerLoadedScene -= OnPlayerLoadedScene;
    }

    private void OnPlayerLoadedScene(PlayerID player, SceneID scene, bool asServer)
    {
        if (!NetworkManager.main.TryGetModule(out ScenesModule scenes, true))
            return;

        var unityScene = gameObject.scene;
        
        if (!scenes.TryGetSceneID(unityScene, out var sceneID))
            return;
        
        if (sceneID != scene)
            return;

        if (!asServer)
            return;

        var p = Instantiate(playerPrefab);
        p.GetComponent<NetworkIdentity>().GiveOwnership(player);
    }
}
