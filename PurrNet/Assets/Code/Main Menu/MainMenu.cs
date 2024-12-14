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

    public void StartHost()
    {
        if (networkManager.transport is SteamTransport)
        {
            steamLobbyManager.CreateLobby();
        }
        else
        {
            networkManager.StartServer();
            networkManager.StartClient();
        }
    }

    public void StartClient()
    {
        if (networkManager.transport is SteamTransport)
        {
            steamLobbyManager.JoinLobby();
        }
        else
        {
            networkManager.StartClient();
        }
    }
}
