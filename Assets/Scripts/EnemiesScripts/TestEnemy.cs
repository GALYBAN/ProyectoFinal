using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : MonoBehaviour
{

    private HealthSystem health;

    void Start()    
    {
        health = FindObjectOfType<HealthSystem>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            health.TakeDamage();
        }
    }

}
