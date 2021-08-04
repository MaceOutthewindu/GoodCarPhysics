using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheels : MonoBehaviour
{
    [SerializeField] private WheelCollider wheel;

    private void FixedUpdate()
    {
        wheel.motorTorque = Input.GetAxis("Vertical") * 100;
    }
}
