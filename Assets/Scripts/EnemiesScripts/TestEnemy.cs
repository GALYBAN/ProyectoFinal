using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : MonoBehaviour
{
    public PlayerStats playerStats;

    void Awake()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerStats.TakeDamage();
        }
    }

}
