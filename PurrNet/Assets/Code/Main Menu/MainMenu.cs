using PurrNet;
using PurrNet.Steam;
using PurrNet.Transports;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] string lobbySceneName = "Lobby";
    [SerializeField] SteamLobbyManager steamLobbyManager;

    NetworkManager networkManager;

    void Awake()
    {
        networkManager = InstanceHandler.NetworkManager;
        InstanceHandler.NetworkManager.onClientConnectionState += OnClientConnectionState;
    }

    private void OnClientConnectionState(ConnectionState state)
    {
        if (state != ConnectionState.Connected || !networkManager.isServer)
            return;

        networkManager.sceneModule.LoadSceneAsync(lobbySceneName);
    }
}
