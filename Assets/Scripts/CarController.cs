using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private int motorTorque;
    [SerializeField] private int steeringAngle;
    [SerializeField] private int brakeTorqueValue;
    [SerializeField] private List<AxelInfo> AxelInfos;
    //[SerializeField] private TextMeshProUGUI speedText;
    private Rigidbody _rigidbody;
    private float currentMotor;
    private float currentSteering;
    private float currentBraking;
    

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        HandleMotor();
        AxelControler();
        //UpdateUI();
    }

    private void HandleMotor()
    {
        currentMotor = motorTorque * Input.GetAxis("Vertical");

        currentSteering = steeringAngle * Input.GetAxis("Horizontal");
    }

    private void AxelControler()
    {
        foreach (var axelInfo in AxelInfos)
        {
            if (axelInfo.motor)
            {
                axelInfo.leftWheel.motorTorque = currentMotor;
                axelInfo.rightWheel.motorTorque = currentMotor;
            }

            if (axelInfo.steering)
            {
                axelInfo.leftWheel.steerAngle = currentSteering;
                axelInfo.rightWheel.steerAngle = currentSteering;
            }

            if (axelInfo.braking)
            {
                axelInfo.leftWheel.brakeTorque = brakeTorqueValue;
                axelInfo.rightWheel.brakeTorque = brakeTorqueValue;
            }
        }
    }

    private void UpdateUI()
    {
        var speed = Mathf.RoundToInt(_rigidbody.velocity.magnitude * 3.6f);
        //speedText.SetText("Speed:" + speed + "kph");
    }
[Serializable]
    public class AxelInfo
    {
        public WheelCollider leftWheel;
        public WheelCollider rightWheel;
        public bool motor;
        public bool steering;
        public bool braking;

    }
}
