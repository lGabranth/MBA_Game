using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject parent;

    public void PlayGame()
    {
        parent.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
