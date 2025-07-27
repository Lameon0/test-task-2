
using UnityEngine;

public class EnemyOrbitAndShoot : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform player;
    public float orbitRadius = 10f;
    [SerializeField] private float approachSpeed = 5f;
    [SerializeField] private float orbitSpeed = 30f;
    public float orbitHeight = 10f;
    public float bankingAngle = 15f;

    [Header("Combat Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireInterval = 2f;
    public int maxHealth = 20;
    public bool isArmored = false;
    public GameObject deathEffect;

    [Header("Audio")]
    public AudioClip hitSound;
    public AudioClip deathSound;

    [Header("Score Settings")]
    public int scoreValue = 20; // Базовые очки за убийство
    public int armoredBonus = 30; // Дополнительные очки за бронированного

    private Rigidbody rb;
    private float angle;
    private float fireTimer;
    private bool isInOrbit = false;
    private int currentHealth;
    private AudioSource audioSource;
    private bool isDead;

    void Die()
    {
        if (isDead) return;
        isDead = true;

        // Эффект разрушения
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Death Effect not assigned in Inspector!");
        }
        if (GameManagerIn.Instance != null)
        {
            GameManagerIn.Instance.RegisterEnemyDeath(gameObject);
        }
        // Начисление очков через ScoreSys
        if (ScoreSys.Instance != null)
        {
            ScoreSys.Instance.AddPointsForEnemy(isArmored);

        }
        else
        {
            Debug.LogError("ScoreSys.Instance is null!");
        }

        Destroy(gameObject);
    }
    void Start()
    {
        InitializeComponents();
        currentHealth = maxHealth;

        // Устанавливаем ценность врага в зависимости от типа
        if (isArmored)
        {
            scoreValue += armoredBonus;
        }
    }

    void InitializeComponents()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        angle = Random.Range(0f, 360f);
    }

    void FixedUpdate()
    {
        if (player == null || isDead) return;

        if (!isInOrbit)
        {
            ApproachOrbit();
        }
        else
        {
            OrbitPlayer();
            HandleShooting();
        }
    }

   public void TakeDamage(int damage) {
    if (isDead) return; 
        if (isDead) return;

        currentHealth -= damage;

        if (hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }



    void ApproachOrbit()
    {
        Vector3 targetOrbitPos = CalculateOrbitPosition();
        Vector3 moveDirection = (targetOrbitPos - transform.position).normalized;

        // Движение
        rb.MovePosition(transform.position + moveDirection * approachSpeed * Time.fixedDeltaTime);

        // Поворот ЧЕРЕЗ Quaternion.LookRotation (исправлено)
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.MoveRotation(Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                approachSpeed * Time.fixedDeltaTime
            ));
        }

        if (Vector3.Distance(transform.position, targetOrbitPos) < 0.5f)
        {
            isInOrbit = true;
        }
    }


    void OrbitPlayer()
    {
        angle += orbitSpeed * Time.fixedDeltaTime;
        if (angle > 360f) angle -= 360f;

        Vector3 orbitPos = CalculateOrbitPosition();
        rb.MovePosition(orbitPos);

        Vector3 tangent = new Vector3(
            -Mathf.Sin(angle * Mathf.Deg2Rad),
            0,
            Mathf.Cos(angle * Mathf.Deg2Rad)
        ).normalized;

        if (tangent.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(tangent);
            float bank = Mathf.Clamp(orbitSpeed, -1f, 1f) * bankingAngle;
            targetRotation *= Quaternion.Euler(0, 0, -bank);
            rb.MoveRotation(targetRotation);
        }
    }

    void HandleShooting()
    {
        fireTimer += Time.fixedDeltaTime;
        if (fireTimer >= fireInterval)
        {
            ShootAtPlayer();
            fireTimer = 0f;
        }
    }

    void ShootAtPlayer()
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerProjectile"))
        {
            ProjectileMovement projectile = other.GetComponent<ProjectileMovement>();
            if (projectile != null)
            {
                TakeDamage(projectile.damage);
            }
            Destroy(other.gameObject);
        }
    }

Vector3 CalculateOrbitPosition()
    {
        return new Vector3(
            player.position.x + orbitRadius * Mathf.Cos(angle * Mathf.Deg2Rad),
            orbitHeight,
            player.position.z + orbitRadius * Mathf.Sin(angle * Mathf.Deg2Rad)
        );
    }
}