using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
  public static GameManager Instance { get; private set; }

  [Header("Lives")]
  public int maxHealth = 3;
  public int health;

  [Header("UI")]
  public TextMeshProUGUI healthText;

  [Header("Game Over UI")]
  public GameObject gameOverPanel;
  public TextMeshProUGUI gameOverText;

  [Header("Score")]
  public int score = 0;
  public TextMeshProUGUI scoreText;

  private bool isGameOver = false;

  private void Awake()
  {
    if (Instance != null && Instance != this)
    {
      Destroy(gameObject);
      return;
    }
    Instance = this;
  }

  private void Start()
  {
    health = maxHealth;
    isGameOver = false;

    if (gameOverPanel != null)
      gameOverPanel.SetActive(false);

    Time.timeScale = 1f;

    UpdateUI();
  }

  public void UpdateScore(int amount)
  {
    score += amount;
    if (scoreText != null)
      scoreText.text = $"SCORE: {score}";
  }

  public void TakeDamage(int amount)
  {
    if (isGameOver) return;

    health -= amount;
    health = Mathf.Clamp(health, 0, maxHealth);

    UpdateUI();

    if (health <= 0)
      GameOver();
  }

  public void Heal(int amount)
  {
    if (isGameOver) return;

    health += amount;
    health = Mathf.Clamp(health, 0, maxHealth);

    UpdateUI();
  }

  private void UpdateUI()
  {
    if (healthText != null)
    {
      string s = "";
      for (int i = 0; i < health; i++) s += "♥";
      for (int i = health; i < maxHealth; i++) s += "_";
      healthText.text = s;
    }
  }

  private void GameOver()
  {
    isGameOver = true;

    if (gameOverText != null)
      gameOverText.text = "GAME OVER";

    if (gameOverPanel != null)
      gameOverPanel.SetActive(true);

    Time.timeScale = 0f;
  }

  public void Restart()
  {
    Time.timeScale = 1f;
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }

  public void BackToMenu()
  {
    Time.timeScale = 1f;
    SceneManager.LoadScene("MenuScene");
  }

  public void QuitGame()
  {
    Application.Quit();
    #if UNITY_EDITOR
      UnityEditor.EditorApplication.isPlaying = false;
    #endif
  }
}
