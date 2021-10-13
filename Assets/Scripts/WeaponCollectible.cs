using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollectible : MonoBehaviour
{

    public AudioClip obtained;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();

        if (player != null)
        {
            player.LevelUp();
            player.PlaySound(obtained);
        }
        Destroy(gameObject);
    }
}
