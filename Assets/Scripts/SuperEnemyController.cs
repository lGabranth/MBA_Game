using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperEnemyController : MonoBehaviour
{
    public float speed = 4f;
    public float lineOfSight = 6f;
    private Transform _player;
    public bool vertical;
    public float timerMax = 3.0f;
    private float _timer;

    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    public int direction = 1;
    private PlayerController _playerController;

    private int _random;
    private int _healthPoint = 200;

    public AudioClip hit;
    private static readonly int MoveX = Animator.StringToHash("MoveX");
    private static readonly int MoveY = Animator.StringToHash("MoveY");

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        GameObject tmpPlayer = GameObject.FindGameObjectWithTag("Player");
        _playerController = tmpPlayer.GetComponent<PlayerController>();
        _player = tmpPlayer.transform;
    }

    private void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0)
        {
            direction = -direction;
            _timer = timerMax;
            SwapAxes();
        }

        // Si le joueur entre dans la range du monstre, celui-ci rush sur le joueur pour le tuer
        float distanceFromPlayer = Vector2.Distance(_player.position, transform.position);
        if (distanceFromPlayer < lineOfSight)
        {
            speed = 7f;
            transform.position = Vector2.MoveTowards(transform.position, _player.position, speed * Time.deltaTime);
        }
    }

    private void SwapAxes()
    {
        _random = Random.Range(0, 100);
        vertical = _random % 2 == 0;
    }

    private void FixedUpdate()
    {
        Vector2 position = _rigidbody2D.position;

        if (vertical)
        {
            position.y += Time.deltaTime * speed * direction;
            _animator.SetFloat(MoveX, 0);
            _animator.SetFloat(MoveY, direction);
        }
        else
        {
            position.x += Time.deltaTime * speed * direction;
            _animator.SetFloat(MoveX, direction);
            _animator.SetFloat(MoveY, 0);
            if (position.x != 0)
            {
                _spriteRenderer.flipX = position.x < 0;
            }
        }

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
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, lineOfSight);
    }

    public void Damage(int damages)
    {
        _healthPoint -= damages;
        _playerController.PlaySound(hit);
        if (_healthPoint <= 0)
        {
            _playerController.UpdateScore(33);
            Destroy(gameObject);
        }
    }
}
