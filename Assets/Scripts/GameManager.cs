using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private int startingHealth = 3;
    [SerializeField] private int scorePerCollectible = 10;

    private int currentScore;
    private int currentHealth;
    private bool gameOver;

    private Text scoreText;
    private Text healthText;
    private GameObject gameOverPanel;
    private Button restartButton;
    private PlayerController player;

    private AudioSource musicSource;
    private AudioSource sfxSource;
    private AudioClip collectClip;

    public int ScorePerCollectible => scorePerCollectible;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        var audioContainer = new GameObject("Audio");
        audioContainer.transform.SetParent(transform);

        musicSource = audioContainer.AddComponent<AudioSource>();
        musicSource.loop = true;
        sfxSource = audioContainer.AddComponent<AudioSource>();
    }

    private void Start()
    {
        ResetState();
    }

    public void RegisterUI(Text score, Text health, GameObject gameOverObject, Button restart)
    {
        scoreText = score;
        healthText = health;
        gameOverPanel = gameOverObject;
        restartButton = restart;
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(Restart);
        }

        UpdateUI();
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    public void RegisterPlayer(PlayerController playerController)
    {
        player = playerController;
    }

    public void ResetState()
    {
        currentScore = 0;
        currentHealth = startingHealth;
        gameOver = false;
        Time.timeScale = 1f;
        UpdateUI();
    }

    public void SetCollectSound(AudioClip clip)
    {
        collectClip = clip;
    }

    public void SetMusic(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void AddScore(int amount)
    {
        if (gameOver)
        {
            return;
        }

        currentScore += amount;
        UpdateUI();
        if (collectClip != null)
        {
            sfxSource.PlayOneShot(collectClip);
        }
    }

    public void ApplyDamage(int amount)
    {
        if (gameOver)
        {
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - amount);
        UpdateUI();

        if (currentHealth <= 0)
        {
            TriggerGameOver();
        }
    }

    public void TriggerGameOver()
    {
        gameOver = true;
        Time.timeScale = 0f;
        if (player != null)
        {
            player.enabled = false;
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore}";
        }

        if (healthText != null)
        {
            healthText.text = $"Health: {currentHealth}";
        }
    }

    public void LoadGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public static AudioClip CreateChime(float frequency = 880f)
    {
        const int sampleRate = 44100;
        const float duration = 0.2f;
        int sampleLength = Mathf.RoundToInt(sampleRate * duration);
        float[] data = new float[sampleLength];

        for (int i = 0; i < sampleLength; i++)
        {
            float time = i / (float)sampleRate;
            data[i] = Mathf.Sin(2 * Mathf.PI * frequency * time) * Mathf.Exp(-5f * time);
        }

        var clip = AudioClip.Create("CollectChime", sampleLength, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    public static AudioClip CreateLoopingDrone(float frequency = 220f)
    {
        const int sampleRate = 44100;
        int sampleLength = Mathf.RoundToInt(sampleRate * 2f);
        float[] data = new float[sampleLength];

        for (int i = 0; i < sampleLength; i++)
        {
            float time = i / (float)sampleRate;
            data[i] = 0.15f * Mathf.Sin(2 * Mathf.PI * frequency * time);
        }

        var clip = AudioClip.Create("BackgroundDrone", sampleLength, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }
}