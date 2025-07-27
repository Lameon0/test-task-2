using UnityEngine;

public class RocketController : MonoBehaviour
{
    [Header("Параметры")]
    public float speed = 15f;
    public float turnSpeed = 180f;
    public int damage = 10;
    public float lifetime = 5f;
    public GameObject explosionEffect;
    public float explosionRadius = 3f;

    private Transform target;
    private Rigidbody rb;

    public void Initialize(float speed, float turnSpeed, int damage, float lifetime)
    {
        this.speed = speed;
        this.turnSpeed = turnSpeed;
        this.damage = damage;
        this.lifetime = lifetime;

        rb = GetComponent<Rigidbody>();
        FindNearestEnemy();
        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        if (target == null)
        {
            FindNearestEnemy();
            if (target == null)
            {
                rb.velocity = transform.forward * speed;
                return;
            }
        }

        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        rb.MoveRotation(Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.fixedDeltaTime
        ));

        rb.velocity = transform.forward * speed;
    }

    void FindNearestEnemy()
    {
        EnemyOrbitAndShoot[] enemies = FindObjectsOfType<EnemyOrbitAndShoot>();
        float closestDist = Mathf.Infinity;

        foreach (EnemyOrbitAndShoot enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                target = enemy.transform;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Explode();
        }
        else if (!other.CompareTag("Player"))
        {
            Explode();
        }
    }

    void Explode()
    {
        // АОЕ повреждение
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            EnemyOrbitAndShoot enemy = hit.GetComponent<EnemyOrbitAndShoot>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}