using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager I;

    private BucketTrigger[] bucketTrigger;

    private UIManager uiManager;
    private int score;
    private int highScore;
    private bool isGameOver;
    private bool isWon;
    private bool buketComplete;

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
        if(uiManager==null) uiManager = FindObjectOfType<UIManager>();
        bucketTrigger = FindObjectsOfType<BucketTrigger>();
        isGameOver=false;
        isWon=false;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshReferences();
    }

    void RefreshReferences()
    {
        if(uiManager==null) uiManager = FindObjectOfType<UIManager>();
        bucketTrigger = FindObjectsOfType<BucketTrigger>();
        isGameOver=false;
        isWon=false;
    }

    private void Update()
    {
        if (!isGameOver && isWon == false)
        {
            
            foreach(var bucket in bucketTrigger)
            {
                if(bucket.IsComplete()==false)
                {
                    buketComplete = false;
                    break;
                }
                else
                {
                    buketComplete = true;
                }
            }
            if(buketComplete)
            {
                GameWon();
            }

        }
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
        uiManager.UpdateScoreUI(score);
        uiManager.UpdateHighScoreUI(highScore);
    }

    public void ResetScore()
    {
        score = 0;
        uiManager.UpdateScoreUI(score);
    }



    public void SetNextValue(int v)
    {
        uiManager.SetNextValue(v);
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        uiManager.GameOver();
        // Optionally stop time:
        // Time.timeScale = 0f;
    }

    public void GameWon()
    {
        if (isWon) return;
        isWon = true;
        uiManager.GameWon();
        int level = PlayerPrefs.GetInt("Level",1)+1;
        PlayerPrefs.SetInt("Level",level);
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