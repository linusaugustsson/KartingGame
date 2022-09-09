using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class LobbyController : MonoBehaviour
{
    public static LobbyController Instance;

    // UI elements
    public TextMeshProUGUI lobbyNameText;

    // Player data
    public GameObject playerListViewContent;
    public GameObject playerListItemPrefab;
    [HideInInspector]
    public GameObject localPlayerObject;

    // Other data
    public ulong currentLobbyID;
    public bool playerItemCreated = false;
    private List<PlayerListItem> playerListItems = new List<PlayerListItem>();
    [HideInInspector]
    public PlayerObjectController localPlayerController;

    public Button startGameButton;
    public TextMeshProUGUI readyButtonText;

    public PreviewCharacter previewCharacterScript;


    // Manager
    private NetworkManagerCustom networkManagerCustom;

    private NetworkManagerCustom NetworkManagerCustom {
        get {
            if (networkManagerCustom != null) {
                return networkManagerCustom;
            }
            return networkManagerCustom = NetworkManagerCustom.singleton as NetworkManagerCustom;
        }
    }

    private void Awake() {
        if(Instance == null) {
            Instance = this;
        }
    }

    public void ChangeCharacter(int _characterValue) {
        localPlayerController.ChangeCharacter(_characterValue);
        previewCharacterScript.ChangeCharacterPreview(_characterValue);
    }

    public void ReadyPlayer() {
        localPlayerController.ChangeReady();
    }



    public void UpdateButton() {
        if(localPlayerController.ready == true) {
            readyButtonText.text = "Unready";
        } else {
            readyButtonText.text = "Ready";
        }
    }

    public void CheckIfAllReady() {
        bool allReady = false;

        foreach(PlayerObjectController player in NetworkManagerCustom.gamePlayers) {
            if(player.ready == true) {
                allReady = true;
            } else {
                allReady = false;
                break;
            }
        }

        if(allReady == true) {
            if(localPlayerController.playerIdNumber == 1) {
                startGameButton.interactable = true;
            } else {
                startGameButton.interactable = false;
            }
            //startGameButton.interactable = true;
        } else {
            startGameButton.interactable = false;
        }

    }


    public void UpdateLobbyName() {
        currentLobbyID = NetworkManagerCustom.GetComponent<SteamLobby>().currentLobbyID;
        lobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(currentLobbyID), "name");
    }

    public void UpdatePlayerList() {
        if(playerItemCreated == false) {
            CreateHostPlayerItem();
        }

        if(playerListItems.Count < NetworkManagerCustom.gamePlayers.Count) {
            CreateClientPlayerItem();
        }

        if(playerListItems.Count > NetworkManagerCustom.gamePlayers.Count) {
            RemovePlayerItem();
        }

        if(playerListItems.Count == NetworkManagerCustom.gamePlayers.Count) {
            UpdatePlayerItem();
        }


    }

    public void FindLocalPlayer() {
        localPlayerObject = GameObject.Find("LocalGamePlayer");
        localPlayerController = localPlayerObject.GetComponent<PlayerObjectController>();

    }

    public void CreateHostPlayerItem() {
        foreach(PlayerObjectController player in NetworkManagerCustom.gamePlayers) {
            GameObject newPlayerItem = Instantiate(playerListItemPrefab) as GameObject;
            PlayerListItem newPlayerItemScript = newPlayerItem.GetComponent<PlayerListItem>();
            newPlayerItemScript.playerName = player.playerName;
            newPlayerItemScript.connectionID = player.connectionID;
            newPlayerItemScript.playerSteamID = player.playerSteamID;
            newPlayerItemScript.ready = player.ready;
            newPlayerItemScript.playerCharacter = player.playerCharacter;
            newPlayerItemScript.SetPlayerValues();

            newPlayerItem.transform.SetParent(playerListViewContent.transform);
            newPlayerItem.transform.localScale = Vector3.one;

            playerListItems.Add(newPlayerItemScript);

        }

        playerItemCreated = true;

    }

    public void CreateClientPlayerItem() {
        foreach (PlayerObjectController player in networkManagerCustom.gamePlayers) {
            if(playerListItems.Any(b => b.connectionID == player.connectionID) == false) {
                GameObject newPlayerItem = Instantiate(playerListItemPrefab) as GameObject;
                PlayerListItem newPlayerItemScript = newPlayerItem.GetComponent<PlayerListItem>();
                newPlayerItemScript.playerName = player.playerName;
                newPlayerItemScript.connectionID = player.connectionID;
                newPlayerItemScript.playerSteamID = player.playerSteamID;
                newPlayerItemScript.ready = player.ready;
                newPlayerItemScript.playerCharacter = player.playerCharacter;
                newPlayerItemScript.SetPlayerValues();

                newPlayerItem.transform.SetParent(playerListViewContent.transform);
                newPlayerItem.transform.localScale = Vector3.one;

                playerListItems.Add(newPlayerItemScript);
            }

        }
    }

    public void UpdatePlayerItem() {
        foreach (PlayerObjectController player in networkManagerCustom.gamePlayers) {

            foreach (PlayerListItem playerListItemScript in playerListItems) {
                if (playerListItemScript.connectionID == player.connectionID) {
                    playerListItemScript.playerName = player.playerName;
                    playerListItemScript.ready = player.ready;
                    playerListItemScript.playerCharacter = player.playerCharacter;
                    playerListItemScript.SetPlayerValues();
                    if(player == localPlayerController) {
                        UpdateButton();
                    }
                }
            }
        }
        CheckIfAllReady();
    }


    public void RemovePlayerItem() {
        List<PlayerListItem> playerListItemsToRemove = new List<PlayerListItem>();

        foreach(PlayerListItem playerListItem in playerListItems) {
            if(networkManagerCustom.gamePlayers.Any(b=>b.connectionID == playerListItem.connectionID) == false) {
                playerListItemsToRemove.Add(playerListItem);
            }
        }
        if(playerListItemsToRemove.Count > 0) {
            foreach(PlayerListItem playerListItemToRemove in playerListItemsToRemove) {
                GameObject objectToRemove = playerListItemToRemove.gameObject;
                playerListItems.Remove(playerListItemToRemove);
                Destroy(objectToRemove);
                objectToRemove = null;
            }
        }


    }

    public void StartGame() {
        //Debug.Log("Pressed button");
        localPlayerController.CanStartGame();
    }



}
