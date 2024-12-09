using Steamworks;
using TMPro;
using UnityEngine;

public class SteamName : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    void Awake()
    {
        text.SetText(SteamFriends.GetPersonaName());
    }
}
