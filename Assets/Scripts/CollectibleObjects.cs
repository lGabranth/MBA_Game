using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleObjects : MonoBehaviour
{
    public bool isCheese;
    public bool isAlgae;
    public bool isMeat;

    public AudioClip obtained;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();

        if (player != null)
        {
            if (isCheese) player.ObtainedCheese();
            if (isAlgae) player.ObtainedAlgae();
            if (isMeat) player.ObtainedMeat();

            player.PlaySound(obtained);
            Destroy(gameObject);
        }
    }
}
