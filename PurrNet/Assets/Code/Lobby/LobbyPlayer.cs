using TMPro;
using UnityEngine;

public class LobbyPlayer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI statusText;

    public void SetNameText(string name) => nameText.SetText(name);
    public void SetReadyText(bool ready) => statusText.SetText(ready ? "<color=green>Ready</color>" : "<color=red>Not ready</color>");
}
