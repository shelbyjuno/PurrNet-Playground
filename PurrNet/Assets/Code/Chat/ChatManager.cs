using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PurrNet;
using UnityEngine.InputSystem;

public class ChatManager : NetworkBehaviour
{
    [SerializeField] int maxMessages = 4;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] ChatMessage chatMessagePrefab;
    [SerializeField] Transform chatMessageParent;

    private List<ChatMessage> chatMessages = new();

    private bool submitting = false;

    void Awake()
    {
        inputField.onSubmit.AddListener(OnSubmit);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        inputField.onSubmit.RemoveListener(OnSubmit);
    }

    void Update()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            if (submitting)
            {
                submitting = false;
            }
            else
            {
                inputField.ActivateInputField();
            }
        }
    }

    private void OnSubmit(string text)
    {
        // Don't send empty messages
        if (text == string.Empty)
            return;

        // Flag
        submitting = true;

        // Clear the input field
        inputField.text = string.Empty;

        // Unfocus the input field
        inputField.DeactivateInputField();

        // Send the message
        CmdSendChatMessage(SteamHelpers.GetSteamID().m_SteamID, text);
    }

    [ServerRpc(requireOwnership: false)]
    void CmdSendChatMessage(ulong userID, string message) => RpcReceiveChatMessage(userID, message);

    [ObserversRpc]
    void RpcReceiveChatMessage(ulong userID, string message)
    {
        // Remove old messages
        if(chatMessages.Count >= maxMessages)
        {
            var messageToRemove = chatMessages[0];
            chatMessages.RemoveAt(0);
            Destroy(messageToRemove.gameObject);
        }

        var chatMessage = Instantiate(chatMessagePrefab, chatMessageParent);
        chatMessages.Add(chatMessage);

        chatMessage.SetProfileImage(userID);
        chatMessage.SetMessage(message);
    }
}
