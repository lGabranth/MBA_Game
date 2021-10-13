using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    public AudioClip throwArrow;
    private PlayerController _player;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    public void Launch(Vector2 direction, float force)
    {
        _player.PlaySound(throwArrow);
        _rigidbody2D.AddForce(direction * force);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        EnemyController enemi1 = other.collider.GetComponent<EnemyController>();
        SuperEnemyController enemi2 = other.collider.GetComponent<SuperEnemyController>();

        if(enemi1 != null) enemi1.Damage(50);
        if(enemi2 != null) enemi2.Damage(50);
        Destroy(gameObject);
    }
}
