using PurrNet;
using PurrNet.Transports;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] string lobbySceneName = "Lobby";    
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
        networkManager.StartServer();
        networkManager.StartClient();
    }

    public void StartClient()
    {
        networkManager.StartClient();
    }
}
