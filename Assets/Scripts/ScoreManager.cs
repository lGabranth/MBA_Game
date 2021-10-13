using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private GameManager _game;
    private const string BEST = "bestScore";
    public int Value { get; private set; }

    public int Best
    {
        get => PlayerPrefs.GetInt(BEST, 0);
        set => PlayerPrefs.SetInt(BEST, value);
    }

    private void Awake()
    {
        _game = GameManager.Instance;
    }

    public void SubmitScore(int score)
    {
        if (score > Best) Best = score;
    }

    public void Reset()
    {
        Value = 0;
    }
}
