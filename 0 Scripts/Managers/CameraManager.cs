using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;


    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }


    public CinemachineVirtualCamera kartCamera;
    public CinemachineVirtualCamera menuCamera;
    public CinemachineVirtualCamera levelIntroCamera;

    public List<CinemachineVirtualCamera> allVirtualCameras;


    public void ShowKartCamera() {
        for(int i = 0; i < allVirtualCameras.Count; i++) {
            if(allVirtualCameras[i] == kartCamera) {
                kartCamera.Priority = 10;
            } else {
                allVirtualCameras[i].Priority = 0;
            }
        }
    }

    public void ShowMenuCamera() {
        for (int i = 0; i < allVirtualCameras.Count; i++) {
            if (allVirtualCameras[i] == kartCamera) {
                menuCamera.Priority = 10;
            } else {
                allVirtualCameras[i].Priority = 0;
            }
        }
    }

    public void ShowLevelIntroCamera() {
        for (int i = 0; i < allVirtualCameras.Count; i++) {
            if (allVirtualCameras[i] == kartCamera) {
                levelIntroCamera.Priority = 10;
            } else {
                allVirtualCameras[i].Priority = 0;
            }
        }
    }




}
