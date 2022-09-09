using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;



    public LevelData loadedLevelData;


    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }


    private void Start() {
        DontDestroyOnLoad(this.gameObject);
    }


}
