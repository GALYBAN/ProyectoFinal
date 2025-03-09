using UnityEngine;

public class AnimationEventBridge : MonoBehaviour
{
    private SlashAttack slashAttack;


    private void Start()
    {
        slashAttack = GetComponentInParent<SlashAttack>();
    }
    public void CallOnSlashAnimationEnd()
    {
        if (slashAttack != null)
        {
            slashAttack.OnSlashAnimationEnd();
        }
        else
        {
            Debug.LogWarning("SlashAttack no encontrado en el padre.");
        }
    }
}