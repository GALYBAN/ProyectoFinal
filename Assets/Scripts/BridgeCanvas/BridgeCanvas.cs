using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeCanvas : MonoBehaviour
{

    public void Reprendre()
    {
        ScenesManager.Instance.LoadSceneWithLoadingScreen("Juego");
    }

    public void PantallaInicial()
    {
        ScenesManager.Instance.LoadSceneWithLoadingScreen("MenuPrincipal");
    }

    public void Sortir()
    {
        ScenesManager.Instance.QuitGame();
    }

    public void NovaPartida()
    {
        SaveManager.DeleteSave();
    }

}
