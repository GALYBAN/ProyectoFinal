using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePoolManager : MonoBehaviour
{
    public GameObject projectilePrefab;
    public int poolSize = 10;
    private List<GameObject> projectilePool;

    void Start()
    {
        projectilePool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject projectile = Instantiate(projectilePrefab);
            projectile.SetActive(false);
            projectilePool.Add(projectile);
        }
    }

    public GameObject GetProjectile(Vector3 position, Quaternion rotation)
    {
        foreach (GameObject projectile in projectilePool)
        {
            if (!projectile.activeInHierarchy)
            {
                projectile.transform.position = position;
                projectile.transform.rotation = rotation;
                projectile.SetActive(true);
                return projectile;
            }
        }
        return null; // O expandir la pool si se prefiere
    }

    public void ReturnToPool(GameObject projectile)
    {
        projectile.SetActive(false);
    }
}
