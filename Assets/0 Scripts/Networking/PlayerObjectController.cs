using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public enum PlayerCharacters {
    Spicy = 0,
    Kobe = 1,
    Chatter = 2,
    Lizabel = 3
}

public class PlayerObjectController : NetworkBehaviour
{
    // Player data
    [SyncVar] public int connectionID;
    [SyncVar] public int playerIdNumber;
    [SyncVar] public ulong playerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string playerName;
    [SyncVar(hook = nameof(PlayerReadyUpdate))] public bool ready;
    [SyncVar(hook = nameof(PlayerCharacterUpdate))] public PlayerCharacters playerCharacter;
    [SyncVar] public int playerStartingPos = 0;
    public int score = 0;

    private NetworkManagerCustom networkManagerCustom;


    private void Start() {
        DontDestroyOnLoad(this.gameObject);
    }

    private void PlayerCharacterUpdate(PlayerCharacters _oldValue, PlayerCharacters _newValue) {
        if(isServer == true) {
            playerCharacter = _newValue;
        }
        if(isClient == true) {
            UpdateCharacter(_newValue);
            LobbyController.Instance.UpdatePlayerList();
        }
    }

    [Command]
    private void CmdSetPlayerCharacter(int _characterValue) {
        this.PlayerCharacterUpdate(this.playerCharacter, (PlayerCharacters)_characterValue);
    }


    public void ChangeCharacter(int _characterValue) {
        if(hasAuthority == true) {
            CmdSetPlayerCharacter(_characterValue);
        }
    }

    public void UpdateCharacter(PlayerCharacters _newValue) {
        playerCharacter = _newValue;
    }


    private void PlayerReadyUpdate(bool _oldValue, bool _newValue) {
        if(isServer == true) {
            this.ready = _newValue;
        }
        if(isClient == true) {
            LobbyController.Instance.UpdatePlayerList();
        }
    }

    [Command]
    private void CmdSetPlayerReady() {
        this.PlayerReadyUpdate(this.ready, !this.ready);
    }

    public void ChangeReady() {
        if(hasAuthority == true) {
            CmdSetPlayerReady();
        }
    }

    private NetworkManagerCustom NetworkManagerCustom {
        get {
            if(networkManagerCustom != null) {
                return networkManagerCustom;
            }
            return networkManagerCustom = NetworkManagerCustom.singleton as NetworkManagerCustom;
        }
    }

    public override void OnStartAuthority() {
        //base.OnStartAuthority();
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
        gameObject.name = "LocalGamePlayer";
        LobbyController.Instance.FindLocalPlayer();
        LobbyController.Instance.UpdateLobbyName();
    }

    public override void OnStartClient() {
        //base.OnStartClient();
        NetworkManagerCustom.gamePlayers.Add(this);
        LobbyController.Instance.UpdateLobbyName();
        LobbyController.Instance.UpdatePlayerList();
    }


    public override void OnStopClient() {
        //base.OnStopClient();
        NetworkManagerCustom.gamePlayers.Remove(this);
        LobbyController.Instance.UpdatePlayerList();
    }

    [Command]
    private void CmdSetPlayerName(string playerName) {
        this.PlayerNameUpdate(this.playerName, playerName);
    }

    public void PlayerNameUpdate(string _oldValue, string _newValue) {
        if(isServer == true) {
            this.playerName = _newValue;
        }

        if(isClient == true) {
            LobbyController.Instance.UpdatePlayerList();
        }
    }

    public void CanStartGame() {
        //Debug.Log("Check authority");
        if(hasAuthority == true) {
            //Debug.Log("Has authority");
            //CmdCanStartGame();
            RpcCanStartGame();
        }
    }

    [Command]
    public void CmdCanStartGame() {
        //Debug.Log("In Cmd");
        NetworkManagerCustom.StartGame();
    }

    [ClientRpc]
    public void RpcCanStartGame() {
        //Debug.Log("In Cmd");
        NetworkManagerCustom.StartGame();
    }

}
