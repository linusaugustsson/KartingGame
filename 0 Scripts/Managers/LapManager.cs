using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LapManager : MonoBehaviour
{


    public float currentLapTime = 0.0f;

    public float bestTime = 999999999.0f;

    public int lapsMade = 0;

    public bool hasStarted = false;

    public TextMeshProUGUI currentTimeText;
    public TextMeshProUGUI bestLapText;


    private void Update() {
        if(hasStarted == true) {
            currentLapTime += Time.deltaTime;

            var minutes = (int)(currentLapTime / 60);
            var seconds = (int)(currentLapTime - minutes * 60);
            //var milliseconds = (int)((currentLapTime - ((minutes * 60) - seconds ))* 100 );

            var milliseconds = (int)((currentLapTime  - seconds) * 100);

            currentTimeText.text = minutes.ToString() + ":" + seconds.ToString("00") + ":" + milliseconds.ToString("00");


        }

    }


    public void HitFinish() {
        if(hasStarted == true) {

            if (currentLapTime < bestTime) {
                bestTime = currentLapTime;
                
                var minutes = (int)(bestTime / 60);
                var seconds = (int)(bestTime - minutes * 60);
                //var milliseconds = (int)((currentLapTime - ((minutes * 60) - seconds ))* 100 );

                var milliseconds = (int)((bestTime - seconds) * 100);

                bestLapText.text = "Best Lap: " + minutes.ToString() + ":" + seconds.ToString("00") + ":" + milliseconds.ToString("00");
            }


            currentLapTime = 0.0f;
        }

        if(hasStarted == false) {
            hasStarted = true;
            
        }





    }

}
