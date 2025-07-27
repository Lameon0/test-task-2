using UnityEngine;
using UnityEngine.UI;

public class ScoreSys : MonoBehaviour
{
    public static ScoreSys Instance;

    public int score = 0;
    public Text scoreText;
    public float pointsPerSecond = 5f;

    private float survivalTimer = 0f;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        // Очки за выживание
        survivalTimer += Time.deltaTime;
        if (survivalTimer >= 1f)
        {
            AddPoints((int)pointsPerSecond);
            survivalTimer = 0f;
        }
    }

    public void AddPoints(int points)
    {
        score += points;
        UpdateUI();
    }

    public void LosePointsOnDamage()
    {
        score = Mathf.Max(0, score - 5);
        UpdateUI();
    }

    public void AddPointsForEnemy(bool isArmored)
    {
        int points = isArmored ? 50 : 20;
        AddPoints(points);
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Очки: " + score;
    }
}