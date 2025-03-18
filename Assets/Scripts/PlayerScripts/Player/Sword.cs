using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Daño");
            EnemyStats enemy = other.GetComponent<EnemyStats>();
            ComboController combo = GetComponentInParent<ComboController>();

            if (combo != null && enemy != null)
            {
                combo.ApplyDamage(enemy);
            }
        }
    }
}
