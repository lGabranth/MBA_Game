using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private EnemiesManager _enemies;

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

        Ui = GetComponent<UIManager>();
        Audio = GetComponent<AudioManager>();
        _enemies = GetComponent<EnemiesManager>();
    }

    private void Start()
    {
        _enemies.StartSpawning();
    }

    // Détruit les barils qui bloque l'entrée de la dernière salle
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
        StartCoroutine(StartAgain());
    }

    // Relance le jeu après la mort
    IEnumerator StartAgain()
    {
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
