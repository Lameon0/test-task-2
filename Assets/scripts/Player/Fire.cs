using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TurretShooting : MonoBehaviour
{
    [Header("Основная атака")]
    public GameObject bulletPrefab;
    public Transform[] firePoints;
    public float fireInterval = 0.2f;

    [Header("Ракетная атака")]
    public GameObject rocketPrefab;
    public float rocketCooldown = 3f;
    public float rocketSpeed = 15f;
    public float rocketTurnSpeed = 180f;
    public int rocketDamage = 10;
    public float rocketLifetime = 5f;

    [Header("UI Elements")]
    public Slider rocketCooldownSlider;
    public Image rocketCooldownFill;
    public Color readyColor = Color.green;
    public Color cooldownColor = Color.red;

    private int currentFirePointIndex = 0;
    private bool isShooting = false;
    private Coroutine shootingCoroutine;
    private float nextRocketTime;
    private bool isRocketReady = true;

    void Start()
    {
        // Инициализация UI
        if (rocketCooldownSlider != null)
        {
            rocketCooldownSlider.maxValue = rocketCooldown;
            rocketCooldownSlider.value = rocketCooldown;
            UpdateCooldownUI();
        }
    }

    void Update()
    {
        if (!isRocketReady)
        {
            UpdateCooldownUI();
        }
    }

    void UpdateCooldownUI()
    {
        if (rocketCooldownSlider == null) return;

        float remainingTime = nextRocketTime - Time.time;
        float cooldownProgress = rocketCooldown - Mathf.Clamp(remainingTime, 0, rocketCooldown);

        rocketCooldownSlider.value = cooldownProgress;

        if (rocketCooldownFill != null)
        {
            rocketCooldownFill.color = isRocketReady ? readyColor : cooldownColor;
        }

        if (remainingTime <= 0 && !isRocketReady)
        {
            isRocketReady = true;
            OnRocketReady();
        }
    }

    void OnRocketReady()
    {
        // Дополнительные эффекты при готовности
        if (rocketCooldownFill != null)
        {
            rocketCooldownFill.color = readyColor;
        }
        // Можно добавить анимацию или звук
    }

    public void StartShooting()
    {
        if (!isShooting)
        {
            isShooting = true;
            shootingCoroutine = StartCoroutine(ShootContinuously());
        }
    }

    public void StopShooting()
    {
        if (isShooting)
        {
            isShooting = false;
            if (shootingCoroutine != null)
            {
                StopCoroutine(shootingCoroutine);
            }
        }
    }

    private IEnumerator ShootContinuously()
    {
        while (isShooting)
        {
            Shoot();
            yield return new WaitForSeconds(fireInterval);
        }
    }

    public void Shoot()
    {
        if (bulletPrefab != null && firePoints.Length > 0)
        {
            Transform firePoint = firePoints[currentFirePointIndex];
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            currentFirePointIndex = (currentFirePointIndex + 1) % firePoints.Length;
        }
    }

    public void LaunchRocket()
    {
        if (!isRocketReady)
        {
            Debug.Log("Ракета в перезарядке!");
            return;
        }

        if (rocketPrefab != null && firePoints.Length > 0)
        {
            isRocketReady = false;
            nextRocketTime = Time.time + rocketCooldown;

            Transform firePoint = firePoints[0];
            GameObject rocket = Instantiate(rocketPrefab, firePoint.position, firePoint.rotation);

            RocketController rocketController = rocket.GetComponent<RocketController>();
            if (rocketController != null)
            {
                rocketController.Initialize(rocketSpeed, rocketTurnSpeed, rocketDamage, rocketLifetime);
            }

            // Запускаем визуализацию перезарядки
            StartCoroutine(RocketCooldownRoutine());
        }
    }

    private IEnumerator RocketCooldownRoutine()
    {
        while (Time.time < nextRocketTime)
        {
            UpdateCooldownUI();
            yield return null;
        }

        isRocketReady = true;
        UpdateCooldownUI();
    }
}