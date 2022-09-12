using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;


    public List<GameObject> levelList = new List<GameObject>();
    public LevelData loadedLevelData;


    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }


    private void Start() {
        loadedLevelData = levelList[0].GetComponent<LevelData>();
        DontDestroyOnLoad(this.gameObject);
    }


}
