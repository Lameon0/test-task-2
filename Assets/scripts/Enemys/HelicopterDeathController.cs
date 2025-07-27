using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HelicopterDeathController : MonoBehaviour
{
    // Добавляем статический флаг
    private static bool isSpawning = false;

    [Header("Physics Settings")]
    public float fallSpeed = 25f;
    public float minRotationSpeed = 30f;
    public float maxRotationSpeed = 120f;
    public float stability = 0.3f;

    [Header("Effects")]
    public ParticleSystem crashSmoke;
    public ParticleSystem fireTrail;
    public float destroyDelay = 3f;
    public AudioClip crashSound;

    private Rigidbody rb;
    private bool isInitialized;

    // Публичный метод для проверки состояния
    public static bool IsSpawning() => isSpawning;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Initialize(Vector3 inheritedVelocity)
    {
        if (isInitialized || isSpawning) return;

        isSpawning = true;
        isInitialized = true;

        rb.velocity = inheritedVelocity.normalized * fallSpeed;
        rb.angularVelocity = CalculateRotation();
        rb.drag = 0.5f;

        StartEffects();
        isSpawning = false;
    }

    Vector3 CalculateRotation()
    {
        return new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(0.5f, 1f),
            Random.Range(-0.5f, 0.5f)
        ).normalized * Random.Range(minRotationSpeed, maxRotationSpeed);
    }

    void StartEffects()
    {
        if (crashSmoke != null)
        {
            Instantiate(crashSmoke, transform.position, Quaternion.identity, transform);
        }

        if (fireTrail != null)
        {
            Instantiate(fireTrail, transform.position, Quaternion.identity, transform);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (crashSound != null)
        {
            AudioSource.PlayClipAtPoint(crashSound, transform.position);
        }
        Destroy(gameObject, destroyDelay);
    }
}