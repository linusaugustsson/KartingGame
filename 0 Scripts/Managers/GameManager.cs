using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int targetFramerate = 144;

    public string playerName = "Default Name";

    public GameState gameState = GameState.mainMenu;

    public MenuManager menuManager;

    public GameObject volumeObject;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        Application.targetFrameRate = targetFramerate;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
        */
    }


    public enum GameState {
        mainMenu = 0,
        lobby = 1,
        gameplay = 2,
        

    }


    public void QuitGame() {
        Application.Quit();
    }


}
