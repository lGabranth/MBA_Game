using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private EnemiesManager _enemies;

    public ScoreManager Score { get; private set; }

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
        _enemies = GetComponent<EnemiesManager>();
    }

    private void Start()
    {
        Score.Reset();
        _enemies.StartSpawning();
    }

    private void StopGame()
    {
        Score.SubmitScore(Score.Value);
    }
}
