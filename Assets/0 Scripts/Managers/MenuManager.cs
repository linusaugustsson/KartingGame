using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;


    public GameManager gameManager;
    public SteamLobby steamLobby;
    public EventSystem eventSystem;

    public GameObject mainMenuCanvasObject;

    public GameObject mainMenuObject;
    public GameObject hostMenuObject;
    public GameObject joinMenuObject;
    public GameObject lobbyMenuObject;
    public GameObject optionsMenuObject;
    public GameObject creditsMenuObject;

    public NetworkManagerCustom networkManagerCustom;
    
    public void CloseMainMenu() {
        mainMenuCanvasObject.SetActive(false);
    }


    public enum MenuState {
        Start,
        Join,
        Host,
        Lobby,
        Options,
        Credits,
        InGame
    }

    [HideInInspector]
    public MenuState menuState = MenuState.Start;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        SetActiveMenu(mainMenuObject);
    }


    public void SetActiveMenu(GameObject _menu) {
        CloseMenus();
        if(_menu == lobbyMenuObject) {
            menuState = MenuState.Lobby;
        }
        if(_menu == joinMenuObject) {
            menuState = MenuState.Join;
        }
        if(_menu == mainMenuObject) {
            menuState = MenuState.Start;
        }
        _menu.SetActive(true);
    }

    public void CloseMenus() {
        mainMenuObject.SetActive(false);
        hostMenuObject.SetActive(false);
        joinMenuObject.SetActive(false);
        optionsMenuObject.SetActive(false);
        creditsMenuObject.SetActive(false);
        lobbyMenuObject.SetActive(false);
    }


    public void ClickJoin() {
        menuState = MenuState.Join;
        SetActiveMenu(joinMenuObject);
        LobbiesListManager.Instance.GetListOfLobbies();
    }

    public void ClickHost() {
        gameManager.gameState = GameManager.GameState.lobby;
        menuState = MenuState.Lobby;
        //SetActiveMenu(hostMenuObject);
        SetActiveMenu(lobbyMenuObject);
        steamLobby.HostLobby();

    }

    public void ClickOptions() {
        menuState = MenuState.Options;
        SetActiveMenu(optionsMenuObject);
    }


    public void ClickCredits() {
        menuState = MenuState.Credits;
        SetActiveMenu(creditsMenuObject);
    }

    public void ClickBack() {
        CloseMenus();
        
        if (menuState == MenuState.Join) {
            SetActiveMenu(mainMenuObject);
            LobbiesListManager.Instance.DestroyLobbies();

        } else if (menuState == MenuState.Host) {
            
        } else if (menuState == MenuState.Options) {
            
        } else if (menuState == MenuState.Credits) {
            
        } else if(menuState == MenuState.Lobby) {
            // Disconnect
            networkManagerCustom.StopHost();
            networkManagerCustom.StopClient();
            //SetActiveMenu(mainMenuObject);
        }
    }

    public void ClickLeave() {
        if (menuState == MenuState.Lobby) {
            // Disconnect
            networkManagerCustom.StopHost();
            networkManagerCustom.StopClient();
            //SetActiveMenu(mainMenuObject);
        }
    }

    public void ClickExit() {
        Application.Quit();
    }

    public void EnterName(string _name) {
        gameManager.playerName = _name;
    }

}
