using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using TMPro;

public class PlayerListItem : MonoBehaviour
{

    public string playerName;
    public int connectionID;
    public ulong playerSteamID;
    private bool avatarReceived = false;

    public TextMeshProUGUI playerNameText;
    public RawImage playerIcon;
    

    public Color unreadyColor;
    public Color readyColor;
    public bool ready;

    public Image background;

    public PlayerCharacters playerCharacter;
    public Image characterSelectedImage;
    public List<Sprite> characterSprites = new List<Sprite>();



    protected Callback<AvatarImageLoaded_t> ImageLoaded;


    private void Start() {
        ImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnImageLoaded);
    }

    public void ChangeReadyStatus() {
        if(ready == true) {
            background.color = readyColor;
        } else {
            background.color = unreadyColor;
        }
    }
    /*
    public void SelectCharacter(Sprite _characterSprite) {
        characterSelectedSprite = _characterSprite;

    }
    */
    public void UpdateCharacterSelected() {
        for(int i = 0; i < characterSprites.Count; i++) {
            characterSelectedImage.sprite = characterSprites[(int)playerCharacter];
        }
        /*
        if(playerCharacter == PlayerCharacters.Spicy) {
            characterSelectedImage.sprite = spicySprite;
        } else if(playerCharacter == PlayerCharacters.Kobe) {
            characterSelectedImage.sprite = spicySprite;
        }
        */
        
    }

    /*
    public void DeselectCharacter() {
        characterSelectedSprite = null;
    }
    */
    private void GetPlayerIcon() {
        int imageID = SteamFriends.GetLargeFriendAvatar((CSteamID)playerSteamID);
        if(imageID == -1) {
            return;
        }
        playerIcon.texture = GetSteamImageAsTexture(imageID);
    }

    public void SetPlayerValues() {
        playerNameText.text = playerName;
        ChangeReadyStatus();
        UpdateCharacterSelected();
        if (avatarReceived == false) {
            GetPlayerIcon();
        }
    }


    private void OnImageLoaded(AvatarImageLoaded_t callback) {
        if(callback.m_steamID.m_SteamID == playerSteamID) {
            playerIcon.texture = GetSteamImageAsTexture(callback.m_iImage);
        } else {
            // Another player
            return;
        }
    }

    private Texture2D GetSteamImageAsTexture(int iImage) {
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);
        if (isValid) {
            byte[] image = new byte[width * height * 4];

            isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));

            if (isValid) {
                //texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, false);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }
        avatarReceived = true;
        return texture;
    }

}
