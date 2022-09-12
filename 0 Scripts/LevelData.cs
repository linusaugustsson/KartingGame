using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LevelData : MonoBehaviour
{
    public AudioSource levelMusic;
    public AudioSource lastLapMusic;

    public List<Transform> startTransforms;

    // Have to pass all checkpoints before reaching goal and go to next lap
    public List<BoxCollider> checkPointColliders;
    public List<BoxCollider> goalCollider;

    public List<GameObject> itemBoxList;

    public CinemachineVirtualCamera introCamera;

    public Sprite levelMap;

    public GameObject deathZone;


}
