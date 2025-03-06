using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashController : MonoBehaviour
{
    public float speed = 5f;
    public ProjectilePoolManager poolManager;
    private bool isMoving = false;

    void Awake()
    {

        poolManager = FindObjectOfType<ProjectilePoolManager>();
        // Asegurarse de tener Animator
        if (GetComponent<Animator>() == null)
        {
            Debug.LogError("SlashController no tiene Animator. Asegúrate de agregar uno en el prefab.");
        }
    }

    void Update()
    {
        if (isMoving)
        {
            transform.Translate(-Vector3.right * speed * Time.deltaTime); // Mueve en el eje X
        }
    }

    public void StartSlashMovement()
    {
        isMoving = true;
    }

    public void StopSlash()
    {
        isMoving = false;
        poolManager.ReturnToPool(gameObject);
    }

}