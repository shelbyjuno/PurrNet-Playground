using System;
using PurrNet.Steam;
using Steamworks;
using UnityEngine;
using UnityEngine.InputSystem;

public class SteamLobbyManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] SteamTransport transport;

    [Header("Lobby Settings")]
    [SerializeField] int maxConnections = 4;
    [SerializeField] ELobbyType lobbyType;

    // Constants
    public const string LOBBY_ID = "LobbyID";

    // Callbacks from steam
    private Callback<LobbyCreated_t> lobbyCreated;
    private Callback<LobbyEnter_t> lobbyEntered;
    private Callback<LobbyChatUpdate_t> lobbyChatUpdate;

    [SerializeField] private CSteamID currentLobby;

    void Awake()
    {
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        lobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
    }

    public void CreateLobby()
    {
        SteamMatchmaking.CreateLobby(lobbyType, maxConnections);
    }

    public void JoinLobby() => JoinLobby(currentLobby);
    public void JoinLobby(CSteamID id)
    {
        SteamMatchmaking.JoinLobby(id);
    }

    public void LeaveLobby() => LeaveLobby(currentLobby);
    public void LeaveLobby(CSteamID id)
    {
        SteamMatchmaking.LeaveLobby(id);
    }

    private void OnLobbyCreated(LobbyCreated_t param)
    {
        string lobbyID = SteamHelpers.GetSteamID().ToString();

        SteamMatchmaking.SetLobbyData((CSteamID)param.m_ulSteamIDLobby, LOBBY_ID, lobbyID);

        transport.address = lobbyID;
        transport.StartServer();

        Debug.Log($"Steam Lobby Created! ({lobbyID})");
    }

    private void OnLobbyEntered(LobbyEnter_t param)
    {
        string lobbyID = SteamMatchmaking.GetLobbyData((CSteamID)param.m_ulSteamIDLobby, LOBBY_ID);

        transport.address = lobbyID;
        currentLobby = (CSteamID)param.m_ulSteamIDLobby;
        transport.StartClient();

        Debug.Log($"Steam Lobby Entered! ({lobbyID})");
    }

    private void OnLobbyChatUpdate(LobbyChatUpdate_t param)
    {
        Debug.Log($"Lobby Chat Update! ({param.m_ulSteamIDLobby})");
    }
}
