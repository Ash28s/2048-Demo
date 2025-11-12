using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private TMP_Text nextText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameWonPanel;


    void Start()
    {
        int highScore = PlayerPrefs.GetInt("HIGH_SCORE", 0);
        UpdateScoreUI(0);
        UpdateHighScoreUI(highScore);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        levelText.text = "Level "+PlayerPrefs.GetInt("Level",1).ToString("0");
    }

    public void UpdateScoreUI(int score)
    {
        if (scoreText != null) scoreText.text = $"Score: {score}";
    }

    public void UpdateHighScoreUI(int highScore)
    {
        if (highScoreText != null) highScoreText.text = $"Best: {highScore}";
    }
        
    public void SetNextValue(int v)
    {
        if (nextText != null) nextText.text = $"Next: {v}";
    }

    public void GameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    public void GameWon()
    {
        if (gameWonPanel != null) gameWonPanel.SetActive(true);
    }
}
