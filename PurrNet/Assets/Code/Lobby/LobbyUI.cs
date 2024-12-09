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
    [SerializeField] TextMeshProUGUI readyButtonText;

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

        var localPlayer = InstanceHandler.NetworkManager.localPlayer;

        switch (change.operation)
        {
            case SyncDictionaryOperation.Added:
                UpdatePlayerCard(change.key, change.value);
                break;
            case SyncDictionaryOperation.Set:
                UpdatePlayerCard(change.key, change.value);
                break;
            case SyncDictionaryOperation.Removed:
                break;
        }
    }

    void UpdatePlayerCard(PlayerID playerID, PlayerLobbyData data)
    {
        // Need to give a new player a UI card
        if (!lobbyPlayerUIs.ContainsKey(playerID))
        {
            var p = Instantiate(lobbyPlayerPrefab, transform).GetComponent<LobbyPlayer>();
            lobbyPlayerUIs.Add(playerID, p);
        }

        LobbyPlayer player = lobbyPlayerUIs[playerID];

        // Update the UI card data
        player.SetNameText(data.name);
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
