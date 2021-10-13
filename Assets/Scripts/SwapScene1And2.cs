using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class SwapScene1And2 : MonoBehaviour
{
    public CinemachineVirtualCamera cmv1;
    public CinemachineVirtualCamera cmv2;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            if (cmv1.gameObject.activeSelf)
            {
                cmv2.gameObject.SetActive(true);
                cmv1.gameObject.SetActive(false);
            }
            else
            {
                cmv1.gameObject.SetActive(true);
                cmv2.gameObject.SetActive(false);
            }
        }
    }
}
