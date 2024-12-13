using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Image profileImage;

    public void SetData(ChatMessage data)
    {
        messageText.text = $"{data.name}: {data.message}";
        SteamHelpers.GetAvatarSprite(SteamHelpers.ConvertToCSteamID(data.userID), (texture) => profileImage.sprite = texture);
    }
}
