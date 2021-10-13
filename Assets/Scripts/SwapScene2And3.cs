using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class SwapScene2And3 : MonoBehaviour
{
    public CinemachineVirtualCamera cmv2;
    public CinemachineVirtualCamera cmv3;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            if (cmv2.gameObject.activeSelf)
            {
                cmv3.gameObject.SetActive(true);
                cmv2.gameObject.SetActive(false);
            }
            else
            {
                cmv2.gameObject.SetActive(true);
                cmv3.gameObject.SetActive(false);
            }
        }
    }
}
