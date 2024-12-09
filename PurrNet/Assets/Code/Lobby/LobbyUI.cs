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
        lobbyManager.playerIDs.onChanged += OnPlayerIDsChanged;
        lobbyManager.playerReadyStates.onChanged += OnPlayerReadyStatesChanged;
        lobbyManager.playerNames.onChanged += OnPlayerNamesChanged;
    }

    void OnDestroy()
    {
        lobbyManager.playerIDs.onChanged -= OnPlayerIDsChanged;
        lobbyManager.playerReadyStates.onChanged -= OnPlayerReadyStatesChanged;
        lobbyManager.playerNames.onChanged -= OnPlayerNamesChanged;
    }

    private void OnPlayerIDsChanged(SyncListChange<PlayerID> change)
    {
        if (change.operation == SyncListOperation.Cleared)
            return;

        switch(change.operation)
        {   
            // New Player is added, create UI
            case SyncListOperation.Added:
                if (lobbyPlayerUIs.ContainsKey(change.value))
                    break;
                var player = Instantiate(lobbyPlayerPrefab, transform).GetComponent<LobbyPlayer>();
                lobbyPlayerUIs.Add(change.value, player);
                break;
            // Player is removed, destroy UI
            case SyncListOperation.Removed:
                player = lobbyPlayerUIs[change.value];
                lobbyPlayerUIs.Remove(change.value);
                Destroy(player.gameObject);
                break;        
        }
    }

    private void OnPlayerReadyStatesChanged(SyncDictionaryChange<PlayerID, bool> change)
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
                player.SetReadyText(change.value);
                break;
            case SyncDictionaryOperation.Set:
                player.SetReadyText(change.value);
                if (change.key == localPlayer)
                    readyButtonText.text = change.value ? "Cancel" : "Ready";
                break;
            case SyncDictionaryOperation.Removed:
                break;
        }
    }

    private void OnPlayerNamesChanged(SyncDictionaryChange<PlayerID, string> change)
    {
        if (change.operation == SyncDictionaryOperation.Cleared)
            return;

        if (!lobbyPlayerUIs.TryGetValue(change.key, out var player) && change.operation == SyncDictionaryOperation.Set)
        {
            Debug.LogError($"Unable to find player UI for player {change.key}");
            return;
        }

        switch (change.operation)
        {
            case SyncDictionaryOperation.Added:
                player.SetNameText(change.value);
                break;
            case SyncDictionaryOperation.Set:
                player.SetNameText(change.value);
                break;
            case SyncDictionaryOperation.Removed:
                break;
        }
    }
}
