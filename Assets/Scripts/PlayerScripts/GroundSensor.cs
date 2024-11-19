using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSensor : MonoBehaviour
{
    [SerializeField] private Transform sensorPosition;
    [SerializeField] private float sensorRadius = 0.5f;
    [SerializeField] private LayerMask groundLayer;

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
