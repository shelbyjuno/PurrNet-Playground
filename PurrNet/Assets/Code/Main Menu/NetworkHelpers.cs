using PurrNet;
using PurrNet.Steam;
using UnityEngine;

public class NetworkHelpers : MonoBehaviour
{
    public static void StartHost()
    {
        var networkManager = InstanceHandler.NetworkManager;
        var steamLobbyManager = SteamLobbyManager.Instance;
        
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

    public static void StopHost()
    {
        var networkManager = InstanceHandler.NetworkManager;
        var steamLobbyManager = SteamLobbyManager.Instance;
        
        if (networkManager.transport is SteamTransport)
        {
            steamLobbyManager.LeaveLobby();
        }
        else
        {
            networkManager.StopServer();
            networkManager.StopClient();
        }
    }

    public static void Stop()
    {
        var networkManager = InstanceHandler.NetworkManager;
        var steamLobbyManager = SteamLobbyManager.Instance;
        
        if (networkManager.transport is SteamTransport)
        {
            steamLobbyManager.LeaveLobby();
        }
        else
        {
            networkManager.StopClient();
            
            if (networkManager.isServer)
                networkManager.StopServer();
        }
    }

    public static void StartClient()
    {
        var networkManager = InstanceHandler.NetworkManager;
        var steamLobbyManager = SteamLobbyManager.Instance;
        
        if (networkManager.transport is SteamTransport)
        {
            steamLobbyManager.JoinLobby();
        }
        else
        {
            networkManager.StartClient();
        }
    }

    private static void StopClient()
    {
        var networkManager = InstanceHandler.NetworkManager;
        var steamLobbyManager = SteamLobbyManager.Instance;
        
        if (networkManager.transport is SteamTransport)
        {
            steamLobbyManager.LeaveLobby();
        }
        else
        {
            networkManager.StopClient();
        }
    }
}
