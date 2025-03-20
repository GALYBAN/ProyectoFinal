using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraTransitions : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private int newPriority = 15;
    [SerializeField] private int defaultPriority = 5;


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            virtualCamera.Priority = newPriority;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            virtualCamera.Priority = defaultPriority;
        }
    }
}
