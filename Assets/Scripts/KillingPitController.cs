using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillingPitController : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();

        if (player != null)
            player.Kill();
    }

    // Active le KillingPit après avoir été trigger, permet au premier ennemi de passer
    private void OnTriggerExit2D(Collider2D other)
    {
        GetComponent<BoxCollider2D>().isTrigger = false;
    }
}
