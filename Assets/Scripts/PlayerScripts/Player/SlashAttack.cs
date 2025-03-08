using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashAttack : MonoBehaviour
{
    private PlayerInputs inputs;
    private Animations anim;
    public ProjectilePoolManager poolManager;
    public ManaSystem manaSystem;
    public Transform firePoint;
    public Transform spawnPoint; // Nuevo punto de spawn para el slash
    public float cooldownTime = 1f;
    private bool isCooldown = false;

    void Start()
    {
        anim = GetComponent<Animations>();
        inputs = GetComponent<PlayerInputs>(); 
    }

    void Update()
    {
        if (inputs.SlashInput && !isCooldown && manaSystem.ConsumeManaSlot())
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
