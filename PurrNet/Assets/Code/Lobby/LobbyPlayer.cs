using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayer : MonoBehaviour
{
    [SerializeField] Image steamImage;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI statusText;

    public void SetSteamImage(Sprite image) => steamImage.sprite = image;
    public void SetNameText(string name) => nameText.SetText(name);
    public void SetReadyText(bool ready) => statusText.SetText(ready ? "<color=green>Ready</color>" : "<color=red>Not ready</color>");
}
