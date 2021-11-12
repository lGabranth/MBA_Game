using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Slider progressBar;
    private bool _hasClickedStart;
    private AsyncOperation _loadingGame;
    private float _progress;
    public TextMeshProUGUI progressPercentage;

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        _hasClickedStart = true;
        _loadingGame = SceneManager.LoadSceneAsync("SampleScene");
    }

    private void FixedUpdate()
    {
        if (_hasClickedStart)
        {
            _progress = Mathf.Clamp01(_loadingGame.progress / 0.9f);
            progressBar.value = _progress;
            progressPercentage.text = $"Loading : {Math.Round(_progress * 100)}%";
        }
    }
}
