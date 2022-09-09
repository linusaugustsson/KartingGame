using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Mirror;

public class KartController : NetworkBehaviour
{

    private InputManager inputManager;
    private SoundManager soundManager;
    private PlayerObjectController playerObjectController;

    private GameObject volumeObject;

    private Volume postVolume;
    private VolumeProfile postProfile;
    private ChromaticAberration chromaticAberration;



    public GameObject baseObject;
    public Transform kartModel;
    public Transform kartNormal;
    public Rigidbody sphere;

    

    public List<ParticleSystem> primaryParticles = new List<ParticleSystem>();
    public List<ParticleSystem> secondaryParticles = new List<ParticleSystem>();

    private float speed, currentSpeed;
    private float rotate, currentRotate;
    private int driftDirection;
    private float driftPower;
    private int driftMode = 0;
    private bool first, second, third;
    private Color c;


    [Header("Bools")]
    public bool drifting;

    [Header("Parameters")]

    public float acceleration = 30f;
    public float steering = 80f;
    public float gravity = 10f;
    public LayerMask layerMask;

    [Header("Model Parts")]

    public Transform frontWheels;
    public Transform backWheels;
    public Transform steeringWheel;

    [Header("Particles")]
    public Transform wheelParticles;
    public Transform flashParticles;
    public Color[] turboColors;




    public float firstBoostThreshold = 100.0f;
    public float secondBoostThreshold = 300.0f;
    public float thirdBoostThreshold = 500.0f;


    public AudioSource idle;
    public AudioSource gas;
    public AudioSource driftSound;
    public AudioSource boostSound;
    public AudioSource flashSound;

    public ParticleSystem turbo1;
    public ParticleSystem turbo2;


    public List<CharacterData> characterData = new List<CharacterData>();
    public List<GameObject> characterModels = new List<GameObject>();


    private void Start() {
        volumeObject = GameManager.Instance.volumeObject;

        inputManager = InputManager.Instance;
        soundManager = SoundManager.Instance;
        postVolume = volumeObject.GetComponent<Volume>();
        postProfile = postVolume.profile;

        SetWheelParticles();


        SetFlashParticles();

        

        postProfile.TryGet(out chromaticAberration);

        if(hasAuthority == true) {
            CameraManager.Instance.kartCamera.Follow = baseObject.transform;
            CameraManager.Instance.kartCamera.LookAt = baseObject.transform;
        }


        playerObjectController = GetComponent<PlayerObjectController>();

        lastPos = transform.position;
    }
    public bool startedDrift = false;
    public bool endedDrift = false;
    public float previousDriftValue = 3.0f;

    public float maxMotorPitch = 2.0f;
    public float minMotorPitch = 0.5f;

    private void Update() {

    }

    private Vector3 lastPos = new Vector3();
    private Vector3 kartVel = new Vector3();

    private void FixedUpdate() {

        // TODO: Input in Authority, sync stuffs outside
        if(hasAuthority == true) {
            Movement();
        }

        if (lastPos != kartModel.position) {
            kartVel = kartModel.position - lastPos;
            kartVel /= Time.deltaTime;
            lastPos = kartModel.position;
        }
        //Debug.Log(kartVel.magnitude);

        gas.pitch = Mathf.Lerp(0.5f, 2.0f, kartVel.magnitude / 80.0f);
    }

    public void Movement() {
        baseObject.transform.position = sphere.transform.position - new Vector3(0, 0.4f, 0);



        //Accelerate
        if (inputManager.gas == 1.0f) {
            speed = acceleration;
        }

        //Steer
        if (inputManager.steerVal != 0.0f) {
            int dir = inputManager.steerVal > 0 ? 1 : -1;
            float amount = Mathf.Abs(inputManager.steerVal);
            Steer(dir, amount);
        }

        //Drift

        if (previousDriftValue == 0 && inputManager.drift != previousDriftValue) {
            startedDrift = true;
        }
        if (previousDriftValue == 1 && inputManager.drift != previousDriftValue) {
            endedDrift = true;
            //driftSound.Pause(); // Should be synced DONE
            CmdStopDriftSound();
        }
        if (startedDrift == true && !drifting && inputManager.steerVal != 0.0f) {
            drifting = true;
            //soundManager.PlaySoundRandomPitch(baseObject.transform.position, soundManager.jumpSound, 0.9f, 1.1f); // Should be synced DONE
            driftDirection = inputManager.steerVal > 0 ? 1 : -1;
            //driftSound.PlayDelayed(0.15f); // Should be synced DONE
            CmdPlayDriftSound();


            // Should be synced DONE
            /*
            foreach (ParticleSystem p in primaryParticles) {
                p.startColor = Color.clear;
                p.Play();
            }
            */
            CmdPlayDriftParticles();

            /*
            kartModel.parent.DOComplete();
            kartModel.parent.DOPunchPosition(baseObject.transform.up * .2f, .3f, 5, 1);
            */
        }

        if (drifting) {

            float control = (driftDirection == 1) ? ExtensionMethods.Remap(inputManager.steerVal, -1, 1, 0, 2) : ExtensionMethods.Remap(inputManager.steerVal, -1, 1, 2, 0);
            float powerControl = (driftDirection == 1) ? ExtensionMethods.Remap(inputManager.steerVal, -1, 1, .2f, 1) : ExtensionMethods.Remap(inputManager.steerVal, -1, 1, 1, .2f);
            //Debug.Log(powerControl);
            driftSound.pitch = Mathf.Lerp(0.8f, 1.0f, powerControl / 1.0f); // should be synced

            Steer(driftDirection, control);

            driftPower += powerControl;

            ColorDrift(); // Should be synced DONE
        }

        if (endedDrift == true && drifting) {
            Boost();
        }


        currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * 12f); speed = 0f;
        currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * 4f); rotate = 0f;

        //gas.pitch = Mathf.Lerp(0.5f, 2.0f, currentSpeed / acceleration); // Should be synced


        if (!drifting) {
            kartModel.localEulerAngles = Vector3.Lerp(kartModel.localEulerAngles, new Vector3(0, 90 + (inputManager.steerVal * 15), kartModel.localEulerAngles.z), .2f);
        } else {
            float control = (driftDirection == 1) ? ExtensionMethods.Remap(inputManager.steerVal, -1, 1, .5f, 2) : ExtensionMethods.Remap(inputManager.steerVal, -1, 1, 2, .5f);
            kartModel.parent.localRotation = Quaternion.Euler(0, Mathf.LerpAngle(kartModel.parent.localEulerAngles.y, (control * 15) * driftDirection, .2f), 0);
        }


        frontWheels.localEulerAngles = new Vector3(0, (inputManager.steerVal * 15), frontWheels.localEulerAngles.z);
        frontWheels.localEulerAngles += new Vector3(0, 0, sphere.velocity.magnitude / 2);
        backWheels.localEulerAngles += new Vector3(0, 0, sphere.velocity.magnitude / 2);


        steeringWheel.localEulerAngles = new Vector3(-25, 90, ((inputManager.steerVal * 45)));

        endedDrift = false;
        startedDrift = false;
        previousDriftValue = inputManager.drift;
















        //////////



        if (!drifting) {
            sphere.AddForce(-kartModel.transform.right * currentSpeed, ForceMode.Acceleration);
        } else {
            sphere.AddForce(baseObject.transform.forward * currentSpeed, ForceMode.Acceleration);
        }

        //Gravity
        sphere.AddForce(Vector3.down * gravity, ForceMode.Acceleration);

        //Steering
        baseObject.transform.eulerAngles = Vector3.Lerp(baseObject.transform.eulerAngles, new Vector3(0, baseObject.transform.eulerAngles.y + currentRotate, 0), Time.deltaTime * 5f);

        RaycastHit hitOn;
        RaycastHit hitNear;

        Physics.Raycast(baseObject.transform.position + (baseObject.transform.up * .1f), Vector3.down, out hitOn, 1.1f, layerMask);
        Physics.Raycast(baseObject.transform.position + (baseObject.transform.up * .1f), Vector3.down, out hitNear, 2.0f, layerMask);

        //Normal Rotation
        kartNormal.up = Vector3.Lerp(kartNormal.up, hitNear.normal, Time.deltaTime * 8.0f);
        kartNormal.Rotate(0, baseObject.transform.eulerAngles.y, 0);
    }

    public void Steer(int direction, float amount) {
        rotate = (steering * direction) * amount;
    }


    // Should be synced DONE
    public void ColorDrift() {
        if (first == false) {
            /*
            c = Color.clear;

            ChangeParticleColor();
            */
            CmdDriftPower(0);
        }
            

        if (driftPower > firstBoostThreshold && driftPower < secondBoostThreshold - 1 && !first) {
            /*
            first = true;
            c = turboColors[0];
            driftMode = 1;

            PlayFlashParticle(c);
            flashSound.pitch = 0.9f;
            flashSound.Play();

            ChangeParticleColor();
            */
            CmdDriftPower(1);

        }

        if (driftPower > secondBoostThreshold && driftPower < thirdBoostThreshold - 1 && !second) {
            /*
            second = true;
            c = turboColors[1];
            driftMode = 2;

            PlayFlashParticle(c);

            flashSound.pitch = 1.0f;
            flashSound.Play();

            ChangeParticleColor();
            */
            CmdDriftPower(2);
        }

        if (driftPower > thirdBoostThreshold && !third) {
            /*
            third = true;
            c = turboColors[2];
            driftMode = 3;

            PlayFlashParticle(c);

            flashSound.pitch = 1.1f;
            flashSound.Play();

            ChangeParticleColor();
            */
            CmdDriftPower(3);
        }
        

        
    }

    public void ChangeParticleColor() {
        foreach (ParticleSystem p in primaryParticles) {
            var pmain = p.main;
            pmain.startColor = c;
        }

        foreach (ParticleSystem p in secondaryParticles) {
            var pmain = p.main;
            pmain.startColor = c;
        }
    }


    // Should be synced DONE
    public void PlayFlashParticle(Color _c) {
        foreach (ParticleSystem p in secondaryParticles) {
            p.startColor = _c;
            p.Play();
        }
    }

    public void Boost() {
        drifting = false;

        if (driftMode > 0) {
            DOVirtual.Float(currentSpeed * 3.5f, currentSpeed, .33f * driftMode, Speed);
            DOVirtual.Float(0, 1, .5f, ChromaticAmount).OnComplete(() => DOVirtual.Float(1, 0, .5f, ChromaticAmount));
            //kartModel.Find("Tube001").GetComponentInChildren<ParticleSystem>().Play();
            //kartModel.Find("Tube002").GetComponentInChildren<ParticleSystem>().Play();

            // Below should be synced DONE
            /*
            turbo1.Play();
            turbo2.Play();
            if(driftMode == 1) {
                boostSound.pitch = Random.Range(0.8f, 0.9f);
            } else if (driftMode == 2) {
                boostSound.pitch = Random.Range(1.0f, 1.1f);
            } else if (driftMode == 3) {
                boostSound.pitch = Random.Range(1.2f, 1.3f);
            }
            boostSound.Play();
            */
            // Above should be synced DONE

            CmdBoost();

            //Debug.Log("BOOST!");
        }

        // Should be synced DONE
        /*
        driftPower = 0;
        driftMode = 0;
        first = false; second = false; third = false;
        */


        // Should be synced DONE
        /*
        foreach (ParticleSystem p in primaryParticles) {
            p.startColor = Color.clear;
            p.Stop();
        }
        */
        CmdStopDriftParticles();

        kartModel.parent.DOLocalRotate(Vector3.zero, .5f).SetEase(Ease.OutBack);

    }

    private void Speed(float x) {
        currentSpeed = x;
    }

    void ChromaticAmount(float x) {
        chromaticAberration.intensity.value = x;
        //postProfile.GetSetting<ChromaticAberration>().intensity.value = x;
    }



    // TODO GET PLAYER CHARACTER FROM PlayerObjectController DONE I THINK
    public void ChangeCharacter() {
        SelectCharacter(playerObjectController.playerCharacter);

    }

    // Might have to sync? SHOULD BE SYNCED ALREADY
    public void SelectCharacter(PlayerCharacters _character) {

        // Change Activate GameObject
        for(int i = 0; i < characterModels.Count; i++) {
            characterModels[i].SetActive(false);
        }
        characterData[(int)_character].model.SetActive(true);

        // Change kart transform
        kartModel = characterData[(int)_character].kartTransform;
        // Change front wheels
        frontWheels = characterData[(int)_character].frontWheels;
        // Change back wheels
        backWheels = characterData[(int)_character].backWheels;
        // Change steering wheel
        steeringWheel = characterData[(int)_character].steeringWheel;

        // Change wheel particles
        wheelParticles = characterData[(int)_character].wheelparticles;
        // Change flash particles
        flashParticles = characterData[(int)_character].flashParticles;

        // Change turbo particles 1
        turbo1 = characterData[(int)_character].turbo1;
        // Change turbo particles 2
        turbo2 = characterData[(int)_character].turbo2;


        SetWheelParticles();


        SetFlashParticles();
    }

    public void SetWheelParticles() {
        primaryParticles.Clear();

        for (int i = 0; i < wheelParticles.GetChild(0).childCount; i++) {
            primaryParticles.Add(wheelParticles.GetChild(0).GetChild(i).GetComponent<ParticleSystem>());
        }

        for (int i = 0; i < wheelParticles.GetChild(1).childCount; i++) {
            primaryParticles.Add(wheelParticles.GetChild(1).GetChild(i).GetComponent<ParticleSystem>());
        }
    }


    public void SetFlashParticles() {
        secondaryParticles.Clear();
        foreach (ParticleSystem p in flashParticles.GetComponentsInChildren<ParticleSystem>()) {
            secondaryParticles.Add(p);
        }
    }

    [Command]
    public void CmdPlayDriftSound() {
        RpcPlayDriftSound();
    }

    [ClientRpc]
    public void RpcPlayDriftSound() {
        soundManager.PlaySoundRandomPitch(baseObject.transform.position, soundManager.jumpSound, 0.9f, 1.1f);
        driftSound.PlayDelayed(0.15f);
    }

    [Command]
    public void CmdStopDriftSound() {
        RpcStopDriftSound();
    }

    [ClientRpc]
    public void RpcStopDriftSound() {
        driftSound.Pause();
    }


    [Command]
    public void CmdPlayDriftParticles() {
        RpcPlayDriftParticles();
    }

    [ClientRpc]
    public void RpcPlayDriftParticles() {
        foreach (ParticleSystem p in primaryParticles) {
            p.startColor = Color.clear;
            p.Play();
        }

        kartModel.parent.DOComplete();
        kartModel.parent.DOPunchPosition(baseObject.transform.up * .2f, .3f, 5, 1);
    }

    [Command]
    public void CmdStopDriftParticles() {
        RpcStopDriftParticles();
    }

    [ClientRpc]
    public void RpcStopDriftParticles() {
        foreach (ParticleSystem p in primaryParticles) {
            p.startColor = Color.clear;
            p.Stop();
        }
    }


    [Command]
    public void CmdDriftPower(int _power) {
        RpcDriftPower(_power);
    }

    [ClientRpc]
    public void RpcDriftPower(int _power) {
        if(_power == 0) {
            c = Color.clear;

            ChangeParticleColor();
        } else if(_power == 1) {
            first = true;
            c = turboColors[0];
            driftMode = 1;

            PlayFlashParticle(c);
            flashSound.pitch = 0.9f;
            flashSound.Play();

            ChangeParticleColor();
        } else if(_power == 2) {
            second = true;
            c = turboColors[1];
            driftMode = 2;

            PlayFlashParticle(c);

            flashSound.pitch = 1.0f;
            flashSound.Play();

            ChangeParticleColor();
        } else if(_power == 3) {
            third = true;
            c = turboColors[2];
            driftMode = 3;

            PlayFlashParticle(c);

            flashSound.pitch = 1.1f;
            flashSound.Play();

            ChangeParticleColor();
        }

    }

    [Command]
    public void CmdBoost() {
        RpcBoost();
    }

    [ClientRpc]
    public void RpcBoost() {
        turbo1.Play();
        turbo2.Play();
        if (driftMode == 1) {
            boostSound.pitch = Random.Range(0.8f, 0.9f);
        } else if (driftMode == 2) {
            boostSound.pitch = Random.Range(1.0f, 1.1f);
        } else if (driftMode == 3) {
            boostSound.pitch = Random.Range(1.2f, 1.3f);
        }
        boostSound.Play();

        driftPower = 0;
        driftMode = 0;
        first = false; second = false; third = false;
    }

}
