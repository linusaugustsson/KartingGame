using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class LobbiesListManager : MonoBehaviour
{

    public static LobbiesListManager Instance;

    //public GameObject lobbiesMenu;
    public GameObject lobbyDataItemPrefab;
    public GameObject lobbyListContent;

    //public GameObject lobbiesButton;
    //public GameObject hostButton;

    public List<GameObject> listOfLobbies = new List<GameObject>();

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }


    public void GetListOfLobbies() {
        SteamLobby.Instance.GetLobbiesList();
    }

    public void DisplayLobbies(List<CSteamID> lobbyIDs, LobbyDataUpdate_t result) {
        for(int i = 0; i < lobbyIDs.Count; i++) {
            if(lobbyIDs[i].m_SteamID == result.m_ulSteamIDLobby) {
                GameObject createdItem = Instantiate(lobbyDataItemPrefab);
                LobbyDataEntry lobbyDataEntry = createdItem.GetComponent<LobbyDataEntry>();
                lobbyDataEntry.lobbyID = (CSteamID)lobbyIDs[i].m_SteamID;
                lobbyDataEntry.lobbyName = SteamMatchmaking.GetLobbyData((CSteamID)lobbyIDs[i].m_SteamID, "name");
                lobbyDataEntry.SetLobbyData();

                createdItem.transform.SetParent(lobbyListContent.transform);
                createdItem.transform.localScale = Vector3.one;

                listOfLobbies.Add(createdItem);
            }
        }
    }


    public void DestroyLobbies() {
        foreach(GameObject lobbyItem in listOfLobbies) {
            Destroy(lobbyItem);
        }
        listOfLobbies.Clear();
    }

}
