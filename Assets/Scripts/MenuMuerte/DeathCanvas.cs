using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCanvas : MonoBehaviour
{

    public void Reprendre()
    {
        ScenesManager.Instance.LoadScene(1);
    }

    public void PantallaInicial()
    {
        ScenesManager.Instance.LoadScene(0);
    }

    public void Sortir()
    {
        ScenesManager.Instance.QuitGame();
    }

}
