# Broadcasting with PurrNet 

## 0. The Idea
Broadcsting in PurrNet is useful, as it allows us to do some basic network functionality, all without needing a `NetworkBehaviour` on our object. For things that are trivial, such as game chat, we don't necessarily need all the functionality of a `NetworkBehaviour`.

The idea is as follows:
1. We will create a *struct* to hold our chat message data, such as a username and a message.
2. We will allow **Clients** to send a message to the **Server**, containing this information.
3. The **Server** will then take this information, and pass it back down to all of the **Clients**.
4. With this information, we can then print it out to some UI, or place it above a player head.

## 1. Creating the `ChatMessage` struct:
To get our chat message to the server, we need to first create a struct to hold our data. As previously mentioned, this struct will hold a `name`, and a `message`. This struct will need to implement the `IPackedAuto` interface, which will automatically handle the reading and writing of the data to the network. If this is not your style, take a look at the `IPacked` and `IPackedSimple` interfaces.

The final struct is as follows:
```C#
public struct ChatMessage : IPackedAuto
{
    public string name;
    public string message;
}
```

## 2. Allowing **Clients** to send a `ChatMessage` to the **Server**
For our **Clients** to be able to send a message to the **Server**, we first need to hook into the `Subscribe` event from the `NetworkManager`
```C#
void NetworkManager.Subscribe<ChatMessage>(PlayerBroadcastDelegate<ChatMessage> callback, bool asServer)
```
The easiest way to do this, is create a script that inherits from `PurrMonoBehaviour`, as this gives us access to two very useful events:

```C#
public abstract void Subscribe(NetworkManager manager, bool asServer);
public abstract void Unsubscribe(NetworkManager manager, bool asServer);
```

For our case, let's create a `ChatManager` script, that inherits from `PurrMonoBehaviour` and subscribe to our chat events, as well as Creating an `OnChatMessage` function to pass in as our callback:

```C#
public class ChatManager : PurrMonoBehaviour
{
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

    // Called when a ChatMessage broadcast is sent from either the Server or a Client
    private void OnChatMessage(PlayerID player, ChatMessage data, bool asServer)
    {
        // TODO: Make this work
    }
}
```

Now that we've subscribed to the events required, we can actually send a `ChatMessage` to the server! You can do this however you'd like, for testing, something like this will be more than sufficient for our needs: 

```C#
void Update()
{
    if (Keyboard.current.enterKey.wasPressedThisFrame)
    {
        ChatMessage message = new ChatMessage
        {
            name = InstanceHandler.NetworkManager.localPlayer.ToString(),
            message = "Hello World!"
        };

        InstanceHandler.NetworkManager.SendToServer<ChatMessage>(message);
    }
}
```

## 3. Receiving a `ChatMessage` on the server
Now that we are sending messages from the **Client**, lets sketch our `OnChatMessage` function out to handle receiving a `ChatMessage` broadcast on the server. As mentioned, if we are the server receiving the broadcast, we want to relay this information and broadcast it back to all of our clients, and we can do it very simply with `NetworkManager.SendToAll<ChatMessage>(ChatMessage)`:

```C#
// Called when a ChatMessage broadcast is sent from either the Server or a Client
private void OnChatMessage(PlayerID player, ChatMessage data, bool asServer)
{
    if (asServer)   // The broadcast was sent to the server from a client
    {
        // Send the broadcast down to the Clients
        InstanceHandler.NetworkManager.SendToAll<ChatMessage>(data);
    }
}
```

## 4. Receiving a `ChatMessage` on the Client
Now that we are receiving messages from the **Server**, we can use our same `OnChatMessage` function to handle the data from the server. For now, let's just debug the message:

```C#
// Called when a ChatMessage broadcast is sent from either the Server or a Client
private void OnChatMessage(PlayerID player, ChatMessage data, bool asServer)
{
    if (asServer)   // The broadcast was sent to the server from a client
    {
        // Send the broadcast down to the Clients
        InstanceHandler.NetworkManager.SendToAll<ChatMessage>(data);
    }
    else    // The broadcast was send to the clients from the server
    {
        Debug.Log($"Received {data.message} from {data.name}!");
    }
}
```

## 5. Wrap Up
With what we have, our final script should look like such:

```C#
public struct ChatMessage : IPackedAuto
{
    public string name;
    public string message;
}

public class ChatManager : PurrMonoBehaviour
{
    void Update()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            ChatMessage message = new ChatMessage
            {
                name = InstanceHandler.NetworkManager.localPlayer.ToString(),
                message = "Hello World!"
            };

            InstanceHandler.NetworkManager.SendToServer<ChatMessage>(message);
        }
    }

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

    // Called when a ChatMessage broadcast is sent from either the Server or a Client
    private void OnChatMessage(PlayerID player, ChatMessage data, bool asServer)
    {
        if (asServer)   // The broadcast was sent to the server from a client
        {
            // Send the broadcast down to the Clients
            InstanceHandler.NetworkManager.SendToAll<ChatMessage>(data);
        }
        else    // The broadcast was send to the clients from the server
        {
            Debug.Log($"Received {data.message} from {data.name}!");
        }
    }
}
```

If you'd like to see a working example in action, take a look at [ChatManager.cs](https://github.com/shelbyjuno/PurrNet-Playground/blob/f73e54eb6ef40670b3d99ffaf24519d1778e1cf2/PurrNet/Assets/Code/Chat/ChatManager.cs)!