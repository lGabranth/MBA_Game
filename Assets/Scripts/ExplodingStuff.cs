using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ExplodingStuff : MonoBehaviour
{
    private Animator _animator;
    [FormerlySerializedAs("Explosion")] public GameObject explosion;
    public AudioClip explode;
    private Rigidbody2D _rigidbody2D;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void Explode()
    {
        Instantiate(explosion, _rigidbody2D.position, Quaternion.identity);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().PlaySound(explode);
        Destroy(gameObject);
    }
}
