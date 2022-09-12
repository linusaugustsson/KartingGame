using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{

    public KartController player;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("HIT");
        /*
        foreach(GameObject box in LevelManager.Instance.loadedLevelData.itemBoxList)
        {
            if(other.gameObject == box)
            {
                player.ItemPickedUp();
            }
        }
        */

        for(int i = 0; i < LevelManager.Instance.loadedLevelData.itemBoxList.Count; i++)
        {
            if (other.gameObject == LevelManager.Instance.loadedLevelData.itemBoxList[i])
            {
                player.ItemPickedUp(i);
                SoundManager.Instance.PlaySound(other.transform.position, SoundManager.Instance.itemSound);
            }
        }

        if(other.gameObject == LevelManager.Instance.loadedLevelData.deathZone)
        {
            player.Respawn();
            transform.position = LevelManager.Instance.loadedLevelData.startTransforms[0].position;
            //transform.rotation = LevelManager.Instance.loadedLevelData.startTransforms[0].rotation;
        }
        /*
        if(other.gameObject.name.Contains("Bit"))
        {
            Debug.Log("Coll with: " + other.gameObject);
            player.ItemPickedUp(other.gameObject);
        }
        */


    }


}
