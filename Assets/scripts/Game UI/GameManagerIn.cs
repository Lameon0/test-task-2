using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerIn : MonoBehaviour
{
    public static GameManagerIn Instance;

    [Header("UI Panels")]
    public GameObject panelPause;
    public GameObject panelGame;
    public GameObject panelReward;

    [Header("Wave Settings")]
    public Transform[] spawnPoints;
    public GameObject[] enemyPrefabs;
    public float timeBetweenWaves = 3f;
    public int baseEnemiesPerWave = 5;
    public int waveMultiplier = 2;
    public int totalWaves = 10;
    public float delayBeforeRewardPanel = 1f;

    [Header("UI Elements")]
    public Slider waveProgressSlider;
    public Text waveInfoText;
    public Text enemiesLeftText;

    [Header("Money System")]
    public int totalMoney = 0;
    public int currentWaveMoney = 0;
    public Text moneyText;
    public Text waveMoneyText;

    private int currentWave = 0;
    private int totalEnemiesInWave = 0;
    private int enemiesKilledInWave = 0;
    private List<GameObject> activeEnemies = new List<GameObject>();
    private bool isSpawningWave = false;
    private bool isGameActive = false;
    public bool IsPaused { get; private set; }

    private const string MoneyKey = "PlayerMoney";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Убрали DontDestroyOnLoad чтобы избежать проблем при перезагрузке сцены
            LoadMoney();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeGame();
        StartGame(); // Запускаем игру только когда всё готово
    }

    public void StartGame()
    {
        if (isGameActive) return;

        isGameActive = true;
        currentWave = 0;
        totalMoney = 0;
        SaveMoney();

        InitializeGame();
        StartCoroutine(WaveRoutine());
    }

    private void InitializeGame()
    {
        panelPause.SetActive(false);
        panelGame.SetActive(true);
        panelReward.SetActive(false);
        IsPaused = false;
        Time.timeScale = 1f;

        if (waveProgressSlider != null)
        {
            waveProgressSlider.minValue = 0;
            waveProgressSlider.maxValue = 1;
            waveProgressSlider.value = 0;
        }

        UpdateMoneyUI();
    }

    private IEnumerator WaveRoutine()
    {
        // Очищаем оставшихся врагов
        foreach (var enemy in activeEnemies)
        {
            if (enemy != null) Destroy(enemy);
        }
        activeEnemies.Clear();

        yield return new WaitForSeconds(1f);

        while (currentWave < totalWaves && isGameActive)
        {
            currentWave++;
            totalEnemiesInWave = CalculateEnemiesInWave();
            enemiesKilledInWave = 0;
            UpdateUI();

            // Спавн врагов
            isSpawningWave = true;
            for (int i = 0; i < totalEnemiesInWave; i++)
            {
                if (IsPaused || !isGameActive) yield return new WaitWhile(() => IsPaused || !isGameActive);
                SpawnEnemy();
                UpdateUI();
                yield return new WaitForSeconds(0.5f);
            }
            isSpawningWave = false;

            // Ожидаем завершения волны
            yield return new WaitUntil(() => activeEnemies.Count == 0 || !isGameActive);

            if (!isGameActive) yield break;

            // Награда за волну
            currentWaveMoney = CalculateWaveMoney();
            totalMoney += currentWaveMoney;
            SaveMoney();
            UpdateMoneyUI();

            // Показываем награду с задержкой
            yield return new WaitForSeconds(delayBeforeRewardPanel);
            ShowRewardPanel();

            // Ждем продолжения (кроме последней волны)
            if (currentWave < totalWaves)
            {
                yield return new WaitUntil(() => !panelReward.activeSelf || !isGameActive);
                if (!isGameActive) yield break;
                yield return new WaitForSeconds(timeBetweenWaves);
            }
        }

        if (isGameActive)
        {
            // Финальная награда
            ShowFinalReward();
        }
    }

    private void ShowFinalReward()
    {
        currentWaveMoney = CalculateWaveMoney();
        totalMoney += currentWaveMoney;
        SaveMoney();
        UpdateMoneyUI();

        ShowRewardPanel();
    }

    public void ContinueFromReward()
    {
        if (currentWave >= totalWaves)
        {
            // Возвращаемся в меню после последней волны
            ReturnToMenu();
        }
        else
        {
            // Продолжаем игру
            panelReward.SetActive(false);
            panelGame.SetActive(true);
            Time.timeScale = 1f;
            IsPaused = false;
        }
    }

    public void ReturnToMenu()
    {
        isGameActive = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(0); // Загружаем меню
    }

    private int CalculateEnemiesInWave()
    {
        return baseEnemiesPerWave + (currentWave * waveMultiplier);
    }

    private int CalculateWaveMoney()
    {
        return enemiesKilledInWave * 2;
    }

    private void SpawnEnemy()
    {
        if (!isGameActive || enemyPrefabs.Length == 0 || spawnPoints.Length == 0) return;

        int enemyIndex = Random.Range(0, enemyPrefabs.Length);
        int spawnIndex = Random.Range(0, spawnPoints.Length);

        GameObject enemy = Instantiate(
            enemyPrefabs[enemyIndex],
            spawnPoints[spawnIndex].position,
            Quaternion.identity
        );

        activeEnemies.Add(enemy);
    }

    public void RegisterEnemyDeath(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            enemiesKilledInWave++;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (waveInfoText != null)
        {
            waveInfoText.text = $"Волна: {currentWave}/{totalWaves}";
        }

        if (enemiesLeftText != null)
        {
            enemiesLeftText.text = $"Осталось: {activeEnemies.Count}";
        }

        if (waveProgressSlider != null)
        {
            if (totalEnemiesInWave > 0)
            {
                float progress = (float)enemiesKilledInWave / totalEnemiesInWave;
                waveProgressSlider.value = progress;
            }
            else
            {
                waveProgressSlider.value = 0;
            }
        }

        UpdateMoneyUI();
    }

    private void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            moneyText.text = $"Монеты: {totalMoney}";
        }

        if (waveMoneyText != null)
        {
            waveMoneyText.text = $"Заработано: {currentWaveMoney}";
        }
    }

    public void ShowRewardPanel()
    {
        IsPaused = true;
        Time.timeScale = 0f;
        panelReward.SetActive(true);
        panelGame.SetActive(false);
        UpdateMoneyUI();
    }

    public void TogglePause()
    {
        if (!isGameActive) return;

        IsPaused = !IsPaused;
        Time.timeScale = IsPaused ? 0f : 1f;
        panelPause.SetActive(IsPaused);
        panelGame.SetActive(!IsPaused && !panelReward.activeSelf);
    }

    public void SaveMoney()
    {
        PlayerPrefs.SetInt(MoneyKey, totalMoney);
        PlayerPrefs.Save();
    }

    private void LoadMoney()
    {
        totalMoney = PlayerPrefs.HasKey(MoneyKey) ? PlayerPrefs.GetInt(MoneyKey) : 0;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // При загрузке сцены инициализируем игру
        if (scene.buildIndex != 0) // Если это не меню
        {
            StartGame();
        }
    }
}