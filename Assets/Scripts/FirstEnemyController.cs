using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstEnemyController : MonoBehaviour
{
    public float speed = 7f;
    public float lineOfSight = 0.98f;
    private Transform _player;
    public int direction = 1;

    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private static readonly int MoveX = Animator.StringToHash("MoveX");
    private static readonly int MoveY = Animator.StringToHash("MoveY");

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        float distanceFromPlayer = Vector2.Distance(_player.position, transform.position);
        if (distanceFromPlayer < lineOfSight)
        {
            speed = 14f;
            transform.position = Vector2.MoveTowards(transform.position, _player.position, speed * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        Vector2 position = _rigidbody2D.position;

        position.y += Time.deltaTime * speed * direction;
        _animator.SetFloat(MoveX, 0);
        _animator.SetFloat(MoveY, direction);

        _rigidbody2D.MovePosition(position);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController playerTouched = collision.gameObject.GetComponent<PlayerController>();

        if (playerTouched != null)
        {
            playerTouched.Kill();
            Destroy(gameObject);
        }

        DestroyableWallController wall = collision.gameObject.GetComponent<DestroyableWallController>();
        if (wall != null)
        {
            Destroy(gameObject);
            wall.Explode();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, lineOfSight);
    }
}
