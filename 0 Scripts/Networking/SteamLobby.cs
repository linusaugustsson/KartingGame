using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using TMPro;

public class SteamLobby : MonoBehaviour
{

    public static SteamLobby Instance;
    
    // Callbacks
    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEntered;

    // Lobbies Callbacks
    protected Callback<LobbyMatchList_t> LobbyList;
    protected Callback<LobbyDataUpdate_t> LobbyDataUpdated;

    public List<CSteamID> lobbyIDs = new List<CSteamID>();

    // Variables
    public ulong currentLobbyID;
    private const string hostAddressKey = "HostAddress";
    private NetworkManagerCustom networkManagerCustom;

    public GameManager gameManager;

    // Gameobjects
    //public GameObject hostButton;
    public TextMeshProUGUI lobbyNameText;

    private void Start() {
        if(SteamManager.Initialized == false) {
            return;
        }

        if(Instance == null) {
            Instance = this;
        }

        if(networkManagerCustom == null) {
            networkManagerCustom = GetComponent<NetworkManagerCustom>();
        }
        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

        LobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbyList);
        LobbyDataUpdated = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyData);
        

    }

    public void HostLobby() {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, networkManagerCustom.maxConnections);
    }

    private void OnLobbyCreated(LobbyCreated_t callback) {
        if(callback.m_eResult != EResult.k_EResultOK) {
            return;
        }

        Debug.Log("Lobby created successfully");

        networkManagerCustom.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), hostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s Lobby");
    }


    private void OnJoinRequest(GameLobbyJoinRequested_t callback) {
        Debug.Log("Request to join lobby");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback) {
        // Everyone
        //hostButton.SetActive(false);
        currentLobbyID = callback.m_ulSteamIDLobby;
        //lobbyNameText.gameObject.SetActive(true);
        //lobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name");

        // Clients
        if(NetworkServer.active == true) {
            return;
        }

        gameManager.gameState = GameManager.GameState.lobby;
        
        gameManager.menuManager.SetActiveMenu(gameManager.menuManager.lobbyMenuObject);

        networkManagerCustom.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), hostAddressKey);

        networkManagerCustom.StartClient();

    }

    public void JoinLobby(CSteamID lobbyID) {
        SteamMatchmaking.JoinLobby(lobbyID);
    }





    //public void LeaveLobby() {
        //SteamMatchmaking.LeaveLobby();
    //}


    public void GetLobbiesList() {
        if(lobbyIDs.Count > 0) {
            lobbyIDs.Clear();
        }

        SteamMatchmaking.AddRequestLobbyListResultCountFilter(60);
        SteamMatchmaking.RequestLobbyList();

    }


    void OnGetLobbyList(LobbyMatchList_t result) {
        if(LobbiesListManager.Instance.listOfLobbies.Count > 0) {
            LobbiesListManager.Instance.DestroyLobbies();
        }

        for(int i = 0; i < result.m_nLobbiesMatching; i++) {
            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
            lobbyIDs.Add(lobbyID);
            SteamMatchmaking.RequestLobbyData(lobbyID);
        }

    }

    void OnGetLobbyData(LobbyDataUpdate_t result) {
        LobbiesListManager.Instance.DisplayLobbies(lobbyIDs, result);
    }
    

}
