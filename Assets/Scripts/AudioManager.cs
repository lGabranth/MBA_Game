using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource source;
    public AudioClip fanfare;
    public AudioClip lostFanfare;

    public void PlayWonFanfare()
    {
        source.PlayOneShot(fanfare);
    }

    public void PlayLostFanfare()
    {
        source.PlayOneShot(lostFanfare);
    }
}
