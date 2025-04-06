using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimBridgeEvents : MonoBehaviour
{

    public Cagatió jefe;
    public Animator introAnimator;

    public delegate void ActivarJefe();
    public static event ActivarJefe OnActivarJefe;
    [SerializeField] private IntroCagatió cagatio;

    public void CagatioLights()
    {
        cagatio.ActivateCagatioLights();
    }

    public void SceneLights()
    {
        cagatio.ActivateSceneLights();
    }

    public void Cristals()
    {
        cagatio.ActivateCristals();
    }
    
    // Este método será llamado por un Animation Event
    public void FinalizarIntro()
    {
        OnActivarJefe?.Invoke();
    }

}
