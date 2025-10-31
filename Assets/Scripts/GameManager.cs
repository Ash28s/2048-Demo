using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager I;

    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private TMP_Text nextText;
    [SerializeField] private GameObject gameOverPanel;

    private int score;
    private int highScore;
    private bool isGameOver;

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        highScore = PlayerPrefs.GetInt("HIGH_SCORE", 0);
        UpdateScoreUI();
        UpdateHighScoreUI();
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    public void AddScore(int value)
    {
        if (isGameOver) return;
        score += value;
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HIGH_SCORE", highScore);
        }
        UpdateScoreUI();
        UpdateHighScoreUI();
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = $"Score: {score}";
    }

    private void UpdateHighScoreUI()
    {
        if (highScoreText != null) highScoreText.text = $"Best: {highScore}";
    }

    public void SetNextValue(int v)
    {
        if (nextText != null) nextText.text = $"Next: {v}";
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        // Optionally stop time:
        // Time.timeScale = 0f;
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    public void Restart()
    {
        // Simple reload
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}