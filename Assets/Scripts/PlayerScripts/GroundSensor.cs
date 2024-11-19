using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSensor : MonoBehaviour
{
    public Transform sensorPosition;
    public float sensorRadius = 0.5f;
    public LayerMask groundLayer;

    public bool IsGrounded()
    {
        return Physics.CheckSphere(sensorPosition.position, sensorRadius, groundLayer);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(sensorPosition.position, sensorRadius);
    }
}
