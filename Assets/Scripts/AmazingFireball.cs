using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmazingFireball : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    public AudioClip throwFireball;
    private PlayerController _player;
    public GameObject explosionPrefab;
    public GameObject projectilePrefab;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    public void Launch(Vector2 direction, float force)
    {
        _player.PlaySound(throwFireball);
        _rigidbody2D.AddForce(direction * force);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        EnemyController enemi1 = other.collider.GetComponent<EnemyController>();
        SuperEnemyController enemi2 = other.collider.GetComponent<SuperEnemyController>();

        if(enemi1 != null) enemi1.Damage(1000);
        if(enemi2 != null) enemi2.Damage(1000);

        Instantiate(explosionPrefab, _rigidbody2D.position, Quaternion.identity);
        _player.PlaySound(throwFireball);
        Destroy(gameObject);

        // Cercle autour de la zone d'explosion
        // Calculer les positions autour du cercle pour instantier des fireballs
        /*
         * x = R x Sin(angle)
         * y = R x Cos(angle)
         */
        // Partir du centre du cercle vers chaque point pour calculer le sens de poussée des projectiles
    }
}
