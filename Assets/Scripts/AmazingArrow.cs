using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmazingArrow : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    public AudioClip throwArrow;
    private PlayerController _player;
    public GameObject projectilePrefab;

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

        // Cercle autour de la zone d'explosion
        float radius = 2f;
        // Créé des projectiles tout autour de la zone d'explosion
        for (int i = 0; i < 8; i++)
        {
            float angle = i * Mathf.PI*2f / 8;
            var position = transform.position;
            Vector3 newPos = new Vector3(position.x + Mathf.Cos(angle)*radius, position.y + Mathf.Sin(angle)*radius, position.z);
            GameObject go = Instantiate(projectilePrefab, newPos, Quaternion.identity);
            Projectile projectile = go.GetComponent<Projectile>();

            projectile.Launch(new Vector2(Mathf.Cos(angle)*radius, Mathf.Sin(angle)*radius), 600);
        }
    }
}
