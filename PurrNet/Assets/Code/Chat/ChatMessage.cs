using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatMessage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Image profileImage;

    public void SetMessage(string messageText) => this.messageText.text = messageText;
    public void SetProfileImage(ulong steamID) => SteamHelpers.GetAvatarSprite(SteamHelpers.ConvertToCSteamID(steamID), (texture) => profileImage.sprite = texture);
}
