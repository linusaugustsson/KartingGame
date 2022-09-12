using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    public static InputManager Instance;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    public float steerVal;
    public float gas;
    public float drift;
    public float attack;

    //public KartController kartController;

    //public PlayerInput playerInput;



    public void OnGas(InputValue _value) {
        gas = _value.Get<float>();
        
        //Debug.Log(gas);
    }


    public void OnSteer(InputValue _value) {
        steerVal = _value.Get<float>();//_value.Get<Vector2>();
        //Debug.Log(steerVal);
    }

    public void OnDrift(InputValue _value) {
        if(_value.Get<float>() < 0.5f) {
            drift = 0.0f;
        } else {
            drift = 1.0f;
        }

        //drift = _value.Get<float>();



    }

    public void OnAttack(InputValue _value)
    {
        attack = _value.Get<float>();
    }



}
