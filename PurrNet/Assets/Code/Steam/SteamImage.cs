using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class SteamImage : MonoBehaviour
{
    [SerializeField] Image image;

    void Start()
    {
        CSteamID steamID = SteamUser.GetSteamID();
        SteamHelpers.GetAvatarSprite(steamID, (avatar) => image.sprite = avatar);
    }
}
