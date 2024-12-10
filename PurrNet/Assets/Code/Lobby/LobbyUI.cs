using System;
using System.Collections.Generic;
using PurrNet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] LobbyManager lobbyManager;
    [SerializeField] GameObject lobbyPlayerPrefab;
    [SerializeField] Transform lobbyPlayerParent;
    [SerializeField] TextMeshProUGUI lobbyNameText;
    [SerializeField] TextMeshProUGUI readyButtonText;
    [SerializeField] Button startButton;

    Dictionary<PlayerID, LobbyPlayer> lobbyPlayerUIs = new Dictionary<PlayerID, LobbyPlayer>(); 

    void Awake()
    {
        lobbyManager.playerLobbyData.onChanged += OnPlayerLobbyDataChanged;
    }

    void OnDestroy()
    {
        lobbyManager.playerLobbyData.onChanged -= OnPlayerLobbyDataChanged;
    }

    private void OnPlayerLobbyDataChanged(SyncDictionaryChange<PlayerID, PlayerLobbyData> change)
    {
        if (change.operation == SyncDictionaryOperation.Cleared)
            return;

        if (!lobbyPlayerUIs.TryGetValue(change.key, out var player) && change.operation == SyncDictionaryOperation.Set)
        {
            Debug.LogError($"Unable to find player UI for player {change.key}");
            return;
        }

        // If info is added or set, update the UI
        if (change.operation == SyncDictionaryOperation.Set || change.operation == SyncDictionaryOperation.Added)
        {
            UpdateLobbyText(change.key, change.value);
            UpdatePlayerCard(change.key, change.value);
            UpdateReadyButton(change.key, change.value);
            UpdateStartButton();
        }
        else if (change.operation == SyncDictionaryOperation.Removed)
        {
            RemovePlayerCard(change.key);
        }

    }

    void UpdateLobbyText(PlayerID playerID, PlayerLobbyData data)
    {
        // Check if the player is the host
        if(playerID.id != 1)
            return;
        
        // If the player is the host, update the lobby name
        lobbyNameText.SetText($"{data.name}'s\n<size=22.5>Lobby</size>");
    }

    void UpdateReadyButton(PlayerID playerID, PlayerLobbyData data)
    {
        // if the local player is the player, update the ready button text to reflect ready / unready
        var localPlayer = InstanceHandler.NetworkManager.localPlayer;
        if (localPlayer != playerID)
            return;

        readyButtonText.SetText(!data.isReady ? "Ready" : "Unready");
    }

    void UpdateStartButton()
    {
        // Only let the host start the game
        if (!InstanceHandler.NetworkManager.isServer)
            return;

        startButton.interactable = lobbyManager.AllPlayersReady();
    }

    void UpdatePlayerCard(PlayerID playerID, PlayerLobbyData data)
    {
        // Need to give a new player a UI card
        if (!lobbyPlayerUIs.ContainsKey(playerID))
        {
            var p = Instantiate(lobbyPlayerPrefab, lobbyPlayerParent).GetComponent<LobbyPlayer>();
            lobbyPlayerUIs.Add(playerID, p);
        }

        LobbyPlayer player = lobbyPlayerUIs[playerID];

        // Update the UI card data
        player.SetNameText(data.name);
        SteamHelpers.GetAvatarSprite(SteamHelpers.ConvertToCSteamID(data.steamID), (sprite) => player.SetSteamImage(sprite));
        player.SetReadyText(data.isReady);
    }

    void RemovePlayerCard(PlayerID playerID)
    {
        if (!lobbyPlayerUIs.ContainsKey(playerID))
            return;

        LobbyPlayer lobbyPlayer = lobbyPlayerUIs[playerID];
        
        // Destroy the UI card
        Destroy(lobbyPlayer.gameObject);

        // Remove them from the dictionary
        lobbyPlayerUIs.Remove(playerID);
    }
}
