using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyBullet : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 20f; // Скорость полета пули
    public int damageAmount = 40; // Урон в очках

    private Transform target;
    private Rigidbody rb;

    void Start()
    {
        // Инициализация компонентов
        rb = GetComponent<Rigidbody>();
        GetComponent<Collider>().isTrigger = true;

        // Поиск игрока
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) target = player.transform;

        // Направление движения
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            rb.velocity = direction * speed;
        }

        // Автоуничтожение через 5 секунд
        Destroy(gameObject, 5f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Нанесение урона
            if (ScoreSys.Instance != null)
            {
                ScoreSys.Instance.LosePointsOnDamage();
            }
            Destroy(gameObject);
        }
        else if (!other.CompareTag("Enemy") && !other.CompareTag("Projectile"))
        {
            // Уничтожение при столкновении с окружением
            Destroy(gameObject);
        }
    }
}