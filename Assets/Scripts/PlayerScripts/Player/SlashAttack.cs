using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SlashAttack : MonoBehaviour
{
    private PlayerInputs inputs;
    private Animator anim;
    private MovementController move;
    private GroundSensor groundSensor;
    private PlayerStats playerStats;
    public ProjectilePoolManager poolManager;
    public Transform firePoint;
    public Transform spawnPoint; // Nuevo punto de spawn para el slash
    public float cooldownTime = 1f;
    private bool isCooldown = false;

    void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        groundSensor = GetComponent<GroundSensor>();    
        move = GetComponent<MovementController>();
        anim = GetComponent<Animator>();
        inputs = GetComponent<PlayerInputs>(); 
    }

    void Update()
    {
        if (inputs.SlashInput && !isCooldown && playerStats.ConsumeManaSlot() && !move.Move() && groundSensor.IsGrounded())
        {
            anim.SetTrigger("Slash");
            GameObject slash = poolManager.GetProjectile(firePoint.position, firePoint.rotation);
            if (slash != null)
            {
                Animator slashAnimator = slash.GetComponent<Animator>();
                if (slashAnimator != null)
                {
                    slashAnimator.SetTrigger("IsSlashing");
                }
                else
                {
                    Debug.LogWarning("Slash prefab no tiene Animator asignado.");
                }
            }
        }
    }

    public void OnSlashAnimationEnd()
    {
        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        isCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        isCooldown = false;
    }

}
