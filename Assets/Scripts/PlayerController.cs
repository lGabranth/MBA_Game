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

    /* SECTION GAMEOBJECT */
    public GameObject prefabDeath;
    public GameObject prefabFireplaceDeath;
    public GameObject projectilePrefab;
    public GameObject ultimateProjectilePrefab;
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
    private bool _hasMeat;
    private bool _AoeAttacks;

    /* SECTION AUDIO */
    private AudioSource _audioSource;
    public AudioClip deathScream;
    public AudioClip normalAttack;
    public AudioClip fanfare;
    public AudioClip lostFanfare;

    /* SECTION TEXTMESHPRO */
    private TextMeshProUGUI _deathCounter;
    public TextMeshProUGUI scoreDisplay;
    public TextMeshProUGUI lifeDisplay;

    /* SECTION NUMBERS */
    private int _deathNumber;
    private int _score; // Score (100 = 1 vie)
    private int _lives = 2; // Nombre de vies
    private float _radiusOfAttack = 2f; // Taille des AOE

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();
        _deathCounter = deathCount.GetComponent<TextMeshProUGUI>();

        _score = 0;
        _deathNumber = 0;

        ToggleActive(
            new[] { menuPause, cheeseImage, algaeImage, meatImage, bowImage, magicImage, wonImage, lostImage }
        );
    }

    private void ToggleActive(GameObject[] elem)
    {
        foreach (GameObject gO in elem)
        {
            gO.SetActive(!gO.activeSelf);
        }
    }

    private void ToggleActive(GameObject elem)
    {
        elem.SetActive(!elem.activeSelf);
    }

    private void Update()
    {
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
        _animator.SetFloat("translationX", translationX);
        _animator.SetFloat("translationY", translationY);

        // Gérer l'attaque
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_hasUltimatePower) ThrowFireball();
            else if (_hasImprovedWeapon) ThrowArrow();
            else ThrowFist();
        }

        // Gérer le sprint
        if (Input.GetKeyDown(KeyCode.LeftShift)) speed = 8f;
        if (Input.GetKeyUp(KeyCode.LeftShift)) speed = 4f;

        // Afficher le menu "pause"
        if(Input.GetKeyDown(KeyCode.Escape)) menuPause.SetActive(true);

        // Vérifier si tous les aliments ont étés ramassés
        if (_hasCheese && _hasAlgae && _hasMeat) YouWon();
    }

    /**
     * Fonction pour gérer la victoire.
     * Arrête la musique de fond, affiche l'écran de victoire et déclenche la fanfare
     */
    private void YouWon()
    {
        ToggleActive(new [] { GameObject.Find("BackgroundMusic"), wonImage, GameObject.Find("Enemies") });
        /*GameObject.Find("BackgroundMusic").SetActive(false);
        wonImage.SetActive(true);
        GameObject.Find("Enemies").SetActive(false);*/
        PlaySound(fanfare);
    }

    /**
     * Fonction pour gérer la défaite.
     * Arrête la musique de fond, affiche l'écran de défaute et déclenche la musique de défaite
     */
    private void YouLost()
    {
        ToggleActive(new [] { GameObject.Find("BackgroundMusic"), lostImage, GameObject.Find("Enemies") });
        /*GameObject.Find("BackgroundMusic").SetActive(false);
        lostImage.SetActive(true);
        GameObject.Find("Enemies").SetActive(false);*/
        PlaySound(lostFanfare);
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
        _animator.SetTrigger("Attack");
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
        _animator.SetTrigger("Attack");
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
        _animator.SetTrigger("Attack");
    }

    /**
     * Afficher la zone AOE autour du personnage, sur la Scène Unity
     */
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _radiusOfAttack);
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
        _deathCounter.SetText($"Death : {_deathNumber}");
        UpdateLife(-1);

        if (_lives < 0) YouLost();

        // On retéléporte le joueur à l'entrée du niveau
        _translation.x = 0;
        _translation.y = -3.44f;
        _rigidbody2D.position = _translation;

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

    private void DestroyBridgeBlocking()
    {
        foreach (Transform barrel in GameObject.Find("BlockingBridge").transform)
        {
            barrel.GetComponent<ExplodingStuff>().Explode();
        }

        GameObject.Find("DarkWizard").gameObject.GetComponent<NpcController>().ToggleAvailability();
    }

    // Vérifie si on obtient le fromage
    public void ObtainedCheese()
    {
        _hasCheese = true;
        cheeseImage.SetActive(true);
        if (_hasAlgae) DestroyBridgeBlocking();
    }

    // Vérifie si on obtient l'algue
    public void ObtainedAlgae()
    {
        _hasAlgae = true;
        algaeImage.SetActive(true);
        if (_hasCheese) DestroyBridgeBlocking();
    }

    // Vérifie si on obtient la viande de zombie
    public void ObtainedMeat()
    {
        _hasMeat = true;
        meatImage.SetActive(true);
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
        UpdateScoreDisplay();
    }

    // Met à jour les PV
    private void UpdateLife(int life)
    {
        _lives += life;
        UpdateLivesDisplay();
    }

    // Met à jour le score visuellement
    private void UpdateScoreDisplay()
    {
        scoreDisplay.SetText($"Score : {_score}");
    }

    // Met à jour le nombre de vies visuellement
    private void UpdateLivesDisplay()
    {
        lifeDisplay.SetText($"Lives : {_lives}");
    }

    public void EnableAoeWeapon()
    {
        _AoeAttacks = true;
    }
}
