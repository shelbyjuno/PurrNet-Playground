using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using PurrNet;

public class ChatUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ChatManager chatManager;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] ChatMessageUI chatMessagePrefab;
    [SerializeField] Transform chatMessageParent;

    [Header("Settings")]
    [SerializeField] int maxMessages = 4;

    private List<ChatMessageUI> chatMessages = new List<ChatMessageUI>();

    void OnEnable()
    {
        inputField.onSubmit.AddListener(OnSubmit);
        chatManager.OnChatMessageReceived.AddListener(OnChatMessageReceived);
    }

    void OnDisable()
    {
        inputField.onSubmit.RemoveListener(OnSubmit);
        chatManager.OnChatMessageReceived.RemoveListener(OnChatMessageReceived);
    }

    void Update()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame && !inputField.isFocused)
            inputField.Select();
    }

    private void OnSubmit(string text)
    {
        // Unfocus the input field
        inputField.DeactivateInputField();

        // Don't send empty messages
        if (text == string.Empty)
            return;

        // Clear the input field
        inputField.text = string.Empty;

        // Unfocus the input field
        inputField.DeactivateInputField();

        // Construct the chat message
        var chatMessage = new ChatMessage
        {
            userID = SteamHelpers.GetSteamID().m_SteamID,
            name = $"{SteamHelpers.GetPersonaName()} ({InstanceHandler.NetworkManager.localPlayer})",
            message = text
        };

        // Send the chat message to the server
        ChatManager.SendChatMessage(chatMessage);
    }

    void OnChatMessageReceived(ChatMessage data)
    {
        // Remove old messages
        if (chatMessages.Count >= maxMessages)
        {
            var messageToRemove = chatMessages[0];
            chatMessages.RemoveAt(0);
            Destroy(messageToRemove.gameObject);
        }

        var chatMessageUI = Instantiate(chatMessagePrefab, chatMessageParent);
        chatMessages.Add(chatMessageUI);

        chatMessageUI.SetData(data);
    }
}
