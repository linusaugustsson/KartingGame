using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartFinishCollider : MonoBehaviour
{

    public bool isStartFinish = false;

    public LapManager lapManager;
    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player") {
            lapManager.HitFinish();
        }
    }



}
