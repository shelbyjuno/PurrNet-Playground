using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using PurrNet;
using PurrNet.Packing;

/// <summary>
/// A simple chat message struct (Needs the IPackedAuto interface as we are broadcasting it)
/// </summary>
public struct ChatMessage : IPackedAuto
{
    public ulong ID;
    public string message;
}

/// <summary>
/// Handles sending and receiving chat messages with broadcasting (avoids having to use a NetworkBehaviour)
/// </summary>
public class ChatManager : PurrMonoBehaviour
{
    // Event to invoke when a chat message is received on the client
    public UnityEvent<ChatMessage> OnChatMessageReceived = new UnityEvent<ChatMessage>();

    // Subscribe to ChatMessage events as either the server, client, or both
    public override void Subscribe(NetworkManager manager, bool asServer)
    {
        manager.Subscribe<ChatMessage>(OnChatMessage, asServer);
    }

    // Unsubscribe to ChatMessage events as either the server, client, or both
    public override void Unsubscribe(NetworkManager manager, bool asServer)
    {
        manager.Unsubscribe<ChatMessage>(OnChatMessage, asServer);
    }

    /// <summary>
    /// Called from a client to send a chat message to the server
    /// </summary>
    /// <param name="data">Message Data to send</param>
    public static void SendChatMessage(ChatMessage data) => InstanceHandler.NetworkManager.SendToServer(data);

    /// <summary>
    /// Called when a ChatMessage broadcast is sent. from either the server or a client
    /// </summary>
    /// <param name="player">The player who sent the message</param>
    /// <param name="data">The message data</param>
    /// <param name="asServer">
    /// If the message is received on the server (asServer = true), send it to all clients (NetworkManager.SendToAll)
    /// If the message is received on a client (asServer = false), invoke the OnChatMessageReceived event
    /// </param>
    private void OnChatMessage(PlayerID player, ChatMessage data, bool asServer)
    {
        if (asServer)   // If called on the server, we need to send the broadcast to the clients
            InstanceHandler.NetworkManager.SendToAll(data);
        else    // If called on a client, we need to invoke the OnChatMessageReceived event (see ChatUI.cs)
            OnChatMessageReceived?.Invoke(data);
    }
}