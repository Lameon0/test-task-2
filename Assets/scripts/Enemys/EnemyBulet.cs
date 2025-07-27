using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyBullet : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 20f; // �������� ������ ����
    public int damageAmount = 40; // ���� � �����

    private Transform target;
    private Rigidbody rb;

    void Start()
    {
        // ������������� �����������
        rb = GetComponent<Rigidbody>();
        GetComponent<Collider>().isTrigger = true;

        // ����� ������
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) target = player.transform;

        // ����������� ��������
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            rb.velocity = direction * speed;
        }

        // ��������������� ����� 5 ������
        Destroy(gameObject, 5f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ��������� �����
            if (ScoreSys.Instance != null)
            {
                ScoreSys.Instance.LosePointsOnDamage();
            }
            Destroy(gameObject);
        }
        else if (!other.CompareTag("Enemy") && !other.CompareTag("Projectile"))
        {
            // ����������� ��� ������������ � ����������
            Destroy(gameObject);
        }
    }
}