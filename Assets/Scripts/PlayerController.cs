using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    public GameObject prefabDeath;
    public GameObject prefabFireplaceDeath;
    public GameObject projectilePrefab;
    public GameObject ultimateProjectilePrefab;

    private Vector2 _translation = new Vector2();
    Vector2 lookDirection = new Vector2(1,0);

    public CinemachineVirtualCamera cmv1;
    public CinemachineVirtualCamera cmv2;
    public CinemachineVirtualCamera cmv3;

    [Range(0, 10)] public float speed = 4f;

    private bool _hasImprovedWeapon = false;
    private bool _hasUltimatePower = false;

    private Dictionary<string, bool> keyItems = new Dictionary<string, bool>();
    public GameObject cheeseImage;
    public GameObject algaeImage;
    public GameObject meatImage;
    public GameObject swordImage;
    public GameObject bowImage;
    public GameObject magicImage;
    public GameObject wonImage;
    public GameObject lostImage;
    public GameObject deathCount;
    public GameObject menuPause;

    private AudioSource _audioSource;
    public AudioClip deathScream;
    public AudioClip normalAttack;
    public AudioClip fanfare;
    public AudioClip lostFanfare;

    private TextMeshProUGUI _deathCounter;
    public TextMeshProUGUI scoreDisplay;
    private int _deathNumber;
    private int _score;

    private bool _hasCheese = false;
    private bool _hasAlgae = false;
    private bool _hasMeat = false;
    private bool hasFinishedTheGame = false;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();

        _score = 100;
        keyItems.Add("cheese", false);

        menuPause.SetActive(false);
        cheeseImage.SetActive(false);
        algaeImage.SetActive(false);
        meatImage.SetActive(false);
        bowImage.SetActive(false);
        magicImage.SetActive(false);
        wonImage.SetActive(false);
        lostImage.SetActive(false);

        _deathCounter = deathCount.GetComponent<TextMeshProUGUI>();
        _deathNumber = 0;
    }

    private void Update()
    {
        float translationX = Input.GetAxis("Horizontal");
        float translationY = Input.GetAxis("Vertical");

        _translation.x = translationX;
        _translation.y = translationY;
        _rigidbody2D.velocity = _translation * speed;

        if(!Mathf.Approximately(_translation.x, 0.0f) || !Mathf.Approximately(_translation.y, 0.0f))
        {
            lookDirection.Set(_translation.x, _translation.y);
            lookDirection.Normalize();
        }

        _animator.SetFloat("translationX", translationX);
        _animator.SetFloat("translationY", translationY);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_hasUltimatePower) ThrowFireball();
            else if (_hasImprovedWeapon) ThrowArrow();
            else ThrowFist();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift)) speed = 8f;
        if (Input.GetKeyUp(KeyCode.LeftShift)) speed = 4f;

        if(Input.GetKeyDown(KeyCode.Escape)) menuPause.SetActive(true);

        if (_hasCheese && _hasAlgae && _hasMeat) YouWon();
    }

    private void YouWon()
    {
        GameObject.Find("BackgroundMusic").SetActive(false);
        wonImage.SetActive(true);
        GameObject.Find("Enemies").SetActive(false);
        PlaySound(fanfare);
    }

    private void YouLost()
    {
        GameObject.Find("BackgroundMusic").SetActive(false);
        lostImage.SetActive(true);
        GameObject.Find("Enemies").SetActive(false);
        PlaySound(lostFanfare);
    }

    private void ThrowFireball()
    {
        GameObject projectileObject = Instantiate(ultimateProjectilePrefab, (_rigidbody2D.position + Vector2.up * 0.5f),
            Quaternion.identity);

        Fireball projectile = projectileObject.GetComponent<Fireball>();
        projectile.Launch(lookDirection, 600);
        _animator.SetTrigger("Attack");
    }

    private void ThrowArrow()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, (_rigidbody2D.position + Vector2.up * 0.5f),
            Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);
        _animator.SetTrigger("Attack");
    }

    private void ThrowFist()
    {
        RaycastHit2D[] hitList = Physics2D.CircleCastAll(_rigidbody2D.position, 3f, new Vector2(1, 1));

        foreach (var hit in hitList)
        {
            EnemyController enemi1 = hit.transform.gameObject.GetComponent<EnemyController>();
            SuperEnemyController enemi2 = hit.transform.gameObject.GetComponent<SuperEnemyController>();
            if (enemi1 != null ) enemi1.Damage(50);
            if (enemi2 != null) enemi2.Damage(50);
        }
        PlaySound(normalAttack);
        _animator.SetTrigger("Attack");
    }

    public void Kill(bool fireplace = false)
    {
        speed = 0;
        if (fireplace) Instantiate(prefabFireplaceDeath, _rigidbody2D.position, Quaternion.identity);
        else Instantiate(prefabDeath, _rigidbody2D.position, quaternion.identity);

        PlaySound(deathScream);

        _deathNumber++;
        _deathCounter.SetText($"Death : {_deathNumber}");
        UpdateScore(-50);

        if (_score <= 0) YouLost();

        _translation.x = 0;
        _translation.y = -3.44f;
        _rigidbody2D.position = _translation;

        if(cmv2.gameObject.activeSelf) cmv2.gameObject.SetActive(false);
        if(cmv3.gameObject.activeSelf) cmv3.gameObject.SetActive(false);
        cmv1.gameObject.SetActive(true);
        speed = 4f;
    }

    public void PlaySound(AudioClip clip)
    {
        _audioSource.PlayOneShot(clip);
    }

    public void LevelUp()
    {
        if (_hasImprovedWeapon)
        {
            _hasUltimatePower = true;
            bowImage.SetActive(false);
            magicImage.SetActive(true);
        }
        else
        {
            _hasImprovedWeapon = true;
            swordImage.SetActive(false);
            bowImage.SetActive(true);
        }
    }

    public void ObtainedCheese()
    {
        _hasCheese = true;
        cheeseImage.SetActive(true);
        if (_hasAlgae)
        {
            foreach (Transform barrel in GameObject.Find("BlockingBridge").transform)
            {
                barrel.GetComponent<ExplodingStuff>().Explode();
            }
        }
    }

    public void ObtainedAlgae()
    {
        _hasAlgae = true;
        algaeImage.SetActive(true);
        if (_hasCheese)
        {
            foreach (Transform barrel in GameObject.Find("BlockingBridge").transform)
            {
                barrel.GetComponent<ExplodingStuff>().Explode();
            }
        }
    }

    public void ObtainedMeat()
    {
        _hasMeat = true;
        meatImage.SetActive(true);
    }

    public void UpdateScore(int score)
    {
        _score += score;
        UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay()
    {
        scoreDisplay.SetText($"Score : {_score}");
    }
}
