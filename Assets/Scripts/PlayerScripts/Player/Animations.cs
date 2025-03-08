using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animations : MonoBehaviour
{
    
    [SerializeField] private Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void SetBool(string paramName, bool value) => animator.SetBool(paramName, value);

    public void SetTrigger(string paramName) => animator.SetTrigger(paramName);

}
