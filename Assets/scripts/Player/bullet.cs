using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 50f;
    public float lifetime = 5f;
    public int damage = 1; // Добавляем параметр урона

    [Header("Collision Settings")]
    public LayerMask collisionMask; // Настройте в инспекторе: Enemy, Environment

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = transform.forward * speed;
        }
        else
        {
            Debug.LogError("Rigidbody missing on projectile!");
        }

        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Пуля столкнулась с: {other.name} (Тег: {other.tag})");

        if (other.CompareTag("Enemy"))
        {
            EnemyOrbitAndShoot enemy = other.GetComponent<EnemyOrbitAndShoot>();
            if (enemy != null)
            {
                Debug.Log("Найден компонент EnemyOrbitAndShoot");
                enemy.TakeDamage(damage);
            }
            else
            {
                Debug.LogError("Не найден EnemyOrbitAndShoot на объекте с тегом Enemy");
            }
            Destroy(gameObject);
        }
    }

    public void SetDirection(Vector3 dir)
    {
        if (rb != null)
        {
            rb.velocity = dir.normalized * speed;
        }
    }
}