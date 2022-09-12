using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    public GameObject itemBoxParticles;

    public void PlayerPickedUpItem(int _itemBox, KartController _player)
    {
        GameObject.Instantiate(itemBoxParticles, LevelManager.Instance.loadedLevelData.itemBoxList[_itemBox].transform.position, Quaternion.Euler(new Vector3(-90.0f, 0.0f, 0.0f)));
        LevelManager.Instance.loadedLevelData.itemBoxList[_itemBox].SetActive(false);
        GivePlayerItem(_player);
    }

    private void GivePlayerItem(KartController _player)
    {
        _player.GotItem(Random.Range(0, 2));
    }



    public void RespawnItems()
    {
        for(int i = 0; i < LevelManager.Instance.loadedLevelData.itemBoxList.Count; i++)
        {
            LevelManager.Instance.loadedLevelData.itemBoxList[i].SetActive(true);
        }
    }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

}
