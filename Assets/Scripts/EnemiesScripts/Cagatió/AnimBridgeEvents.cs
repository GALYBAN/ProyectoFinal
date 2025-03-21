using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimBridgeEvents : MonoBehaviour
{
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

}
