using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private EnemiesManager _enemies;

    public ScoreManager Score { get; private set; }
    public UIManager Ui { get; private set; }
    public AudioManager Audio { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else if (Instance != this)
        {
            Destroy(gameObject);
        }

        Score = GetComponent<ScoreManager>();
        Ui = GetComponent<UIManager>();
        Audio = GetComponent<AudioManager>();
        _enemies = GetComponent<EnemiesManager>();
    }

    private void Start()
    {
        Score.Reset();
        _enemies.StartSpawning();
    }

    public void DestroyBridgeBlocking()
    {
        foreach (Transform barrel in GameObject.Find("BlockingBridge").transform)
        {
            barrel.GetComponent<ExplodingStuff>().Explode();
        }

        GameObject.Find("DarkWizard").gameObject.GetComponent<NpcController>().ToggleAvailability();
    }

    public void GameWon()
    {
        Ui.DisplayWon();
        Audio.PlayWonFanfare();
    }

    public void GameLost()
    {
        Ui.DisplayLost();
        Audio.PlayLostFanfare();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
