using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChangeSun : MonoBehaviour
{
    [SerializeField] private GameObject sun1;
    [SerializeField] private GameObject sun2;

    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            sun1.SetActive(false);
            sun2.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            sun1.SetActive(true);
            sun2.SetActive(false);
        }
    }
}
