using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableWallController : MonoBehaviour
{
    private Animator _animator;
    public GameObject Explosion;
    public AudioClip explode;
    private Rigidbody2D _rigidbody2D;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void Explode()
    {
        Instantiate(Explosion, _rigidbody2D.position, Quaternion.identity);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().PlaySound(explode);
        Destroy(gameObject);
    }
}
