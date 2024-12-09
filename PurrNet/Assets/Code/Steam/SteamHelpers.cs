using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public static class SteamHelpers
{
    private static Dictionary<CSteamID, Texture2D> avatarTextures = new Dictionary<CSteamID, Texture2D>();

    public static void GetAvatarSprite(CSteamID id, Action<Sprite> callback)
    {
        GetAvatarTexture(id, (avatar) => callback(Sprite.Create(avatar, new Rect(0, 0, avatar.width, avatar.height), new Vector2(0.5f, 0.5f))));
    }

    public static void GetAvatarTexture(CSteamID id, Action<Texture2D> callback)
    {
        if(id == CSteamID.Nil) return;

        if (avatarTextures.ContainsKey(id))
        {
            callback(avatarTextures[id]);
            return;
        }

        int avatar_id = SteamFriends.GetLargeFriendAvatar(id);
        
        // Avatar is not loaded yet
        if(avatar_id <= 0)
        {
            // Request the users information if unknown
            if(avatar_id == 0)
            {
                SteamFriends.RequestUserInformation(id, false);
                Callback<PersonaStateChange_t>.Create((param) =>
                {
                    if (param.m_nChangeFlags == EPersonaChange.k_EPersonaChangeAvatar && param.m_ulSteamID == id.m_SteamID)
                        SteamFriends.GetLargeFriendAvatar(id);
                });
            }

            // Callback for when the avatar is loaded
            Callback<AvatarImageLoaded_t>.Create((param) =>
            {
                if (param.m_steamID == id)
                    GetAvatarTexture(id, callback);
            });

            return;
        }
        
        if (SteamUtils.GetImageSize(avatar_id, out uint width, out uint height) == true && width > 0 && height > 0)
        {
            var image = new byte[width * height * 4];
            SteamUtils.GetImageRGBA(avatar_id, image, (int)(width * height * 4));

            Texture2D avatar = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
            avatar.LoadRawTextureData(image);
            avatar.Apply();

            // flip the texture vertically
            var pixels = avatar.GetPixels();
            Array.Reverse(pixels, 0, pixels.Length);
            avatar.SetPixels(pixels);
            avatar.Apply();

            // flip the texture horizontally
            pixels = avatar.GetPixels();
            for (int i = 0; i < avatar.width; i++)
                Array.Reverse(pixels, i * avatar.width, avatar.width);
            avatar.SetPixels(pixels);
            avatar.Apply();

            // Convert RGBA to sRGB
            pixels = avatar.GetPixels();
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i].r = Mathf.GammaToLinearSpace(pixels[i].r);
                pixels[i].g = Mathf.GammaToLinearSpace(pixels[i].g);
                pixels[i].b = Mathf.GammaToLinearSpace(pixels[i].b);
            }
            avatar.SetPixels(pixels);
            avatar.Apply();

            avatarTextures.Add(id, avatar);

            callback(avatar);
        }

        return;
    }
    
    public static string GetPersonaName() => SteamFriends.GetPersonaName();
    public static string GetPersonaName(CSteamID id) => SteamFriends.GetFriendPersonaName(id);

    public static string GetPersonaState() => SteamFriends.GetPersonaState().ToString();
    public static string GetPersonaState(CSteamID id) => SteamFriends.GetFriendPersonaState(id).ToString();

    public static CSteamID GetSteamID() => SteamUser.GetSteamID();

    public static CSteamID ConvertToCSteamID<T>(T id)
    {
        if(id is ulong)
            return new CSteamID((ulong)(object)id);
        else if(id is string)
            return new CSteamID(ulong.Parse((string)(object)id));
        else
            throw new ArgumentException("Invalid type");
    }
}
