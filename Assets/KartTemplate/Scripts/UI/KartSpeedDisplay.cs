using System;
using System.Collections;
using System.Collections.Generic;
using KartGame.KartSystems;
using UnityEngine;
using UnityEngine.UI;

public class KartSpeedDisplay : MonoBehaviour
{

    [SerializeField]
    public Slider forwardSpeed;
    public Slider turnSpeed;

    private KeyboardInput _kart;

    private void Start()
    {
        _kart = GameObject.Find("Kart").GetComponent<KeyboardInput>();
    }

    void Update()
    {
        Debug.Log(_kart.Acceleration);
        forwardSpeed.value = _kart.Acceleration;
        turnSpeed.value = _kart.Steering;
    }
}
