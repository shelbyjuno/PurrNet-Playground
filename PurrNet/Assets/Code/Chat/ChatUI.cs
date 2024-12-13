using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

        // Send the message
        var chatMessage = new ChatMessage
        {
            ID = SteamHelpers.GetSteamID().m_SteamID,
            message = text
        };

        ChatManager.SendChatMessage(chatMessage);
    }

    void OnChatMessageReceived(ChatMessage data)
    {
        // Remove old messages
        if(chatMessages.Count >= maxMessages)
        {
            var messageToRemove = chatMessages[0];
            chatMessages.RemoveAt(0);
            Destroy(messageToRemove.gameObject);
        }

        var chatMessageUI = Instantiate(chatMessagePrefab, chatMessageParent);
        chatMessages.Add(chatMessageUI);

        chatMessageUI.SetProfileImage(data.ID);
        chatMessageUI.SetMessage(data.message);
    }
}
