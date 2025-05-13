using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeCanvas : MonoBehaviour
{

    public void Reprendre()
    {
        if (SaveSystem.Instance.SaveExists())
        {
            ScenesManager.Instance.LoadSceneWithLoadingScreen("Juego", false);
        }
        else
        {
            Debug.LogWarning("No save file found to continue from!");
        }
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
        SaveSystem.Instance.DeleteSave();
        ScenesManager.Instance.LoadSceneWithLoadingScreen("Juego", true);
    }

}
