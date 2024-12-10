using Steamworks;
using TMPro;
using UnityEngine;

public class SteamName : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    void Start()
    {
        text.SetText(SteamFriends.GetPersonaName());
    }
}
