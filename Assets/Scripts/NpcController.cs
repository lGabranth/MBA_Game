using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour
{
    private bool _isAvailable;
    private PlayerController _player;
    private SpriteRenderer _renderer;
    public Sprite availableNpc;

    private void Awake()
    {
        _player = GameObject.FindWithTag("Player").gameObject.GetComponent<PlayerController>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void ToggleAvailability()
    {
        _isAvailable = true;
        _renderer.sprite = availableNpc;
    }

    public void Talk()
    {
        if (_isAvailable) _player.EnableAoeWeapon();
    }
}
