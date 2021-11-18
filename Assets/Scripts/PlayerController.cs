using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private GameManager _gameManager;

    /* SECTION GAMEOBJECT */
    public GameObject gameManager;
    public GameObject prefabDeath;
    public GameObject prefabFireplaceDeath;
    public GameObject projectilePrefab;
    public GameObject aoeProjectilePrefab;
    public GameObject ultimateProjectilePrefab;
    public GameObject aoeUltimateProjectilePrefab;

    /* SECTION VECTORS */
    private Vector2 _translation; // Pour gérer les déplacements
    private Vector2 _lookDirection = new Vector2(1,0); // Pour gérer la direction dans laquelle lancer les projectiles

    // Permet d'avoir une trace des Caméra pour gérer le swap quand on meurt et qu'on est tp au spawn
    public CinemachineVirtualCamera cmv1;
    public CinemachineVirtualCamera cmv2;
    public CinemachineVirtualCamera cmv3;

    [Range(0, 10)] public float speed = 4f;

    /* SECTION BOOLEANS */
    private bool _hasImprovedWeapon;
    private bool _hasUltimatePower;
    private bool _hasCheese;
    private bool _hasAlgae;
    private bool _aoeAttacks;

    /* SECTION AUDIO */
    private AudioSource _audioSource;
    public AudioClip deathScream;
    public AudioClip normalAttack;

    /* SECTION NUMBERS */
    private int _deathNumber;
    private int _score; // Score (100 = 1 vie)
    private int _lives = 2; // Nombre de vies
    private int _maxLives = 20;
    private int _livesObtained;
    private float _radiusOfAttack = 2f; // Taille des AOE
    private int _aoeCount = 5;
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int TranslationX = Animator.StringToHash("translationX");
    private static readonly int TranslationY = Animator.StringToHash("translationY");

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();
        _gameManager = gameManager.GetComponent<GameManager>();

        _score = 0;
        _deathNumber = 0;
    }

    private void Update()
    {
        // Afficher la touche à l'écran + indicateur

        // Gérer les déplacements
        float translationX = Input.GetAxis("Horizontal");
        float translationY = Input.GetAxis("Vertical");
        _translation.x = translationX;
        _translation.y = translationY;
        _rigidbody2D.velocity = _translation * speed;

        // Calculer dans quel sens on regarde
        if(!Mathf.Approximately(_translation.x, 0.0f) || !Mathf.Approximately(_translation.y, 0.0f))
        {
            _lookDirection.Set(_translation.x, _translation.y);
            _lookDirection.Normalize();
        }

        // Animation de déplacement
        _animator.SetFloat(TranslationX, translationX);
        _animator.SetFloat(TranslationY, translationY);

        // Gérer l'attaque
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_hasUltimatePower) ThrowFireball();
            else if (_hasImprovedWeapon) ThrowArrow();
            else ThrowFist();
        }

        // Gérer les grenades
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (_aoeAttacks && _aoeCount > 0)
            {
                if (_hasUltimatePower) ThrowAmazingFireball();
                else ThrowAmazingProjectile();
                UpdateAmmoCount();
            }
        }

        // Gérer le sprint
        if (Input.GetKeyDown(KeyCode.LeftShift)) speed = 8f;
        if (Input.GetKeyUp(KeyCode.LeftShift)) speed = 4f;

        // Afficher le menu "pause"
        if(Input.GetKeyDown(KeyCode.Escape)) _gameManager.Ui.ToggleMenuPause();
    }

    /**
     * Permet de lancer une boule de feu
     */
    private void ThrowFireball()
    {
        GameObject projectileObject = Instantiate(ultimateProjectilePrefab, (_rigidbody2D.position + Vector2.up * 0.5f),
            Quaternion.identity);
        Fireball projectile = projectileObject.GetComponent<Fireball>();
        projectile.Launch(_lookDirection, 600);
        _animator.SetTrigger(Attack);
    }

    /**
     * Permet de lancer une flèche
     */
    private void ThrowArrow()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, (_rigidbody2D.position + Vector2.up * 0.5f),
            Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(_lookDirection, 300);
        _animator.SetTrigger(Attack);
    }

    /**
     * Permet de mettre un coup en AOE
     */
    private void ThrowFist()
    {
        // Récupère la liste des ennemies dans une hitbox autour du personnage
        RaycastHit2D[] hitList = Physics2D.CircleCastAll(_rigidbody2D.position, _radiusOfAttack, new Vector2(1, 1));

        foreach (var hit in hitList)
        {
            EnemyController enemi1 = hit.transform.gameObject.GetComponent<EnemyController>();
            SuperEnemyController enemi2 = hit.transform.gameObject.GetComponent<SuperEnemyController>();
            if (enemi1 != null ) enemi1.Damage(300);
            if (enemi2 != null) enemi2.Damage(300);
        }
        PlaySound(normalAttack);
        _animator.SetTrigger(Attack);
    }

    /**
     * Gère la mort
     */
    public void Kill(bool fireplace = false)
    {
        speed = 0; // Set la speed à 0 pour éviter qu'on se déplace plus que prévu quand on meurt

        // Gérer le prefab laissé derrière le personnage à la mort
        if (fireplace) Instantiate(prefabFireplaceDeath, _rigidbody2D.position, Quaternion.identity);
        else Instantiate(prefabDeath, _rigidbody2D.position, quaternion.identity);

        PlaySound(deathScream);

        _deathNumber++;
        _gameManager.Ui.UpdateDeathScore(_deathNumber);
        UpdateLife(-1);

        if (_lives < 0) _gameManager.GameLost();

        // On retéléporte le joueur à l'entrée du niveau
        ResetPosition();

        // On désactive les caméra et on active la première, pour reprendre le focus sur le joueur
        if(cmv2.gameObject.activeSelf) cmv2.gameObject.SetActive(false);
        if(cmv3.gameObject.activeSelf) cmv3.gameObject.SetActive(false);
        cmv1.gameObject.SetActive(true);
        speed = 4f;
    }

    /**
     * Permet de jouer une musique en OneShot
     */
    public void PlaySound(AudioClip clip)
    {
        _audioSource.PlayOneShot(clip);
    }

    /**
     * Permet de faire level up le joueur en fonction des objets ramassés
     */
    public void LevelUp()
    {
        // Si le joueur a déjà ramassé un upgrade, il obtient l'arme ultime
        // Sinon il obtient juste les attaques à distance
        if (_hasImprovedWeapon)
        {
            _hasUltimatePower = true;
            _gameManager.Ui.UiMagic();
        }
        else
        {
            _hasImprovedWeapon = true;
            _gameManager.Ui.UiBow();
        }
    }

    // Vérifie si on obtient le fromage
    public void ObtainedCheese()
    {
        _hasCheese = true;
        UpdateLife(1);
        _gameManager.Ui.ToggleCheeseImage();
        if (_hasAlgae) _gameManager.DestroyBridgeBlocking();
    }

    // Vérifie si on obtient l'algue
    public void ObtainedAlgae()
    {
        _hasAlgae = true;
        UpdateLife(1);
        _gameManager.Ui.ToggleAlgaeImage();
        if (_hasCheese) _gameManager.DestroyBridgeBlocking();
    }

    // Vérifie si on obtient la viande de zombie
    public void ObtainedMeat()
    {
        _gameManager.Ui.ToggleMeatImage();
        ResetPosition();
        _gameManager.GameWon();
        _audioSource.mute = true;
    }

    private void ResetPosition()
    {
        _translation.x = 0;
        _translation.y = -3.44f;
        _rigidbody2D.position = _translation;
    }

    // Met à jour le score
    public void UpdateScore(int score)
    {
        _score += score;
        if (_score >= 100)
        {
            UpdateLife(_score / 100);
            _score -= 100;
        }
        _gameManager.Ui.UpdateScoreDisplay(_score);
    }

    // Met à jour les PV
    private void UpdateLife(int life)
    {
        if (_livesObtained < _maxLives)
        {
            if (life > 0) _livesObtained += life;
            _lives += life;
            _gameManager.Ui.UpdateLivesDisplay(_lives);
        }
    }

    public void EnableAoeWeapon()
    {
        _aoeAttacks = true;
        UpdateAmmoCount();
    }

    private void ThrowAmazingFireball()
    {
        GameObject projectileObject = Instantiate(aoeUltimateProjectilePrefab, (_rigidbody2D.position + Vector2.up * 0.5f),
                Quaternion.identity);
        AmazingFireball projectile = projectileObject.GetComponent<AmazingFireball>();
        projectile.Launch(_lookDirection, 600);
        _animator.SetTrigger(Attack);
    }

    private void ThrowAmazingProjectile()
    {
        GameObject projectileObject = Instantiate(aoeProjectilePrefab, (_rigidbody2D.position + Vector2.up * 0.5f),
                Quaternion.identity);
        AmazingArrow projectile = projectileObject.GetComponent<AmazingArrow>();
        projectile.Launch(_lookDirection, 600);
        _animator.SetTrigger(Attack);
    }

    private void UpdateAmmoCount()
    {
        --_aoeCount;
        _gameManager.Ui.UpdateAmmoCount(_aoeCount);
    }
}
