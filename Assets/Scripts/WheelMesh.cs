using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelMesh : MonoBehaviour
{
    private WheelCollider wheelCollider;
    private void Start()
    {
        // Once, at startup, search up the tree for the nearest wheel collider,
        // and grab a reference to it.
        wheelCollider = GetComponentInParent<WheelCollider>();
    }

    private void FixedUpdate()
    {
        // Each physics update, get the pose of the wheel in World space coordinates.
        wheelCollider.GetWorldPose(out var pos, out var rot);

        // Apply that pose to this object's transform.
        transform.position = pos;
        transform.rotation = rot;
    }
}
