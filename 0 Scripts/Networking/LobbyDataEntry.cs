using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using TMPro;

public class LobbyDataEntry : MonoBehaviour
{
    public CSteamID lobbyID;
    public string lobbyName;
    public TextMeshProUGUI lobbyNameText;


    public void SetLobbyData() {
        

        if(lobbyName == "") {
            lobbyNameText.text = "Empty Name";
        } else {
            lobbyNameText.text = lobbyName;
        }

    }



    public void JoinLobby() {
        SteamLobby.Instance.JoinLobby(lobbyID);
    }

}
