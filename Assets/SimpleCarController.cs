using System.Collections.Generic;
using UnityEngine;

public class SimpleCarController : MonoBehaviour
{
                        /*Serialize*/

    //Car Settings
    [Header("Car Settings")]
    [SerializeField] private float motorTorque;

    [SerializeField] private float brakeForce;

    [Range(0, 180)] [SerializeField] private float steeringAngle;

    [Range(500, 4000)] [SerializeField] private int carWeight;

    //Axles
    [Space(20)] [SerializeField] private List<AxleInfo> axleInfos;

    //Camera Settings
    [Header("Camera Settings")] [Space(10)] [SerializeField]
    private Camera camera;

    [Range(1, 50)] [SerializeField] private float cameraDistance;

    [SerializeField] private float stepMouseWheel;

                        /*Private*/
                        
    //Car variable
    private float currentBrakeForce;
    private float currentMotorTorque;
    private float currentSteeringAngle;

    private Rigidbody rigidbody;

    //Input keyboard
    private float verticalAxle;
    private float horizontalAxle;
    private bool isBraking;
    private bool isRestart;

    //Camera
    private Vector3 previousPosition;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void SetCarSettings()
    {
        //weight
        rigidbody.mass = carWeight;

        //spring
        foreach (var axle in axleInfos)
        {
            var leftWheel = axle.leftWheel;
            var leftSpring = leftWheel.suspensionSpring;
            leftSpring.spring = axle.springForce;
            leftSpring.damper = axle.damperValue;
            leftSpring.targetPosition = axle.suspensionSpringPosition;
            leftWheel.suspensionSpring = leftSpring;
            leftWheel.suspensionDistance = axle.suspensionSpringDistance;


            var rightWheel = axle.rightWheel;
            var rightSpring = rightWheel.suspensionSpring;
            rightSpring.spring = axle.springForce;
            rightSpring.damper = axle.damperValue;
            rightSpring.targetPosition = axle.suspensionSpringPosition;
            rightWheel.suspensionSpring = rightSpring;
            rightWheel.suspensionDistance = axle.suspensionSpringDistance;
        }
    }

    private void GetInput()
    {
        verticalAxle = Input.GetAxis("Vertical");
        horizontalAxle = Input.GetAxis("Horizontal");
        isBraking = Input.GetKey(KeyCode.Space);
        isRestart = Input.GetKeyDown(KeyCode.R);
    }

    private void CameraSetup()
    {
        cameraDistance += Input.GetAxis("Mouse ScrollWheel") * -stepMouseWheel;
        if (Input.GetMouseButtonDown(0))
        {
            previousPosition = camera.ScreenToViewportPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 newPosition = camera.ScreenToViewportPoint(Input.mousePosition);
            Vector3 direction = previousPosition - newPosition;

            float rotationAroundYAxis = -direction.x * 180; // camera moves horizontally
            float rotationAroundXAxis = direction.y * 180; // camera moves vertically

            camera.transform.Rotate(new Vector3(1, 0, 0), rotationAroundXAxis);
            camera.transform.Rotate(new Vector3(0, 1, 0), rotationAroundYAxis, Space.World);

            previousPosition = newPosition;
        }

        camera.transform.position = transform.position;
        camera.transform.Translate(new Vector3(0, 0, -cameraDistance));
    }

    private void HandleMotor()
    {
        currentMotorTorque = motorTorque * verticalAxle;
        currentSteeringAngle = steeringAngle * horizontalAxle;
        currentBrakeForce = isBraking ? brakeForce : 0f;
    }

    private void AxleController()
    {
        foreach (var axle in axleInfos)
        {
            if (axle.steering)
            {
                axle.leftWheel.steerAngle = currentSteeringAngle;
                axle.rightWheel.steerAngle = currentSteeringAngle;
            }

            if (axle.motor)
            {
                axle.leftWheel.motorTorque = currentMotorTorque;
                axle.rightWheel.motorTorque = currentMotorTorque;
            }

            if (axle.braking)
            {
                axle.leftWheel.brakeTorque = currentBrakeForce;
                axle.rightWheel.brakeTorque = currentBrakeForce;
            }
        }
    }

    private void Restart()
    {
        if (!isRestart) return;
        var tr = transform;
        var pos = tr.position;
        tr.position = pos + new Vector3(0, 5, 0);
        tr.rotation = Quaternion.Euler(tr.rotation.x, 0f, tr.rotation.y);
        rigidbody.isKinematic = true;
        rigidbody.isKinematic = false;
    }

    private void UpdateMeshes()
    {
        foreach (var axle in axleInfos)
        {
            UpdateSingleMesh(axle.leftWheel, axle.leftWheelMesh);
            UpdateSingleMesh(axle.rightWheel, axle.rightWheelMesh);
        }
    }

    private static void UpdateSingleMesh(WheelCollider wheelCollider, Transform wheelTransform)
    {
        wheelCollider.GetWorldPose(out var pos, out var rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    private void FixedUpdate()
    {
        GetInput();
        SetCarSettings();
        HandleMotor();
        AxleController();
        Restart();
        CameraSetup();
        UpdateMeshes();
    }

    [System.Serializable]
    public class AxleInfo
    {
        [Header("Wheels")] public WheelCollider leftWheel;
        public WheelCollider rightWheel;
        public Transform leftWheelMesh;
        public Transform rightWheelMesh;
        [Header("Suspension Spring")] public int springForce;
        public int damperValue;
        [Range(0, 2)] public float suspensionSpringDistance;
        [Range(0, 1)] public float suspensionSpringPosition;
        [Header("Car controls")] public bool motor;
        public bool steering;
        public bool braking;
    }
}