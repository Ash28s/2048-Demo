
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private NumberPiece numberPiecePrefab;

    [Header("Spawn Points")]
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    [Header("UI Controls")]
    [SerializeField] private Button dropButton;

    [Header("Settings")]
    [SerializeField] private float pieceMoveSpeed = 15f;
    [SerializeField] private float spawnDelay = 1.0f; // 1 second delay
    [SerializeField] private float swipeThreshold = 50f;

    [Header("Next Preview")]
    [SerializeField] private SpriteRenderer previewSprite;
    [SerializeField] private TMP_Text previewLabel;


    private Camera cam;
    private NumberPiece heldPiece;
    private int currentSpawnIndex = 0;
    private int nextValue;
    private BallColor nextColor; // NEW: Store next color

    // Swipe tracking
    private Vector2 lastTouchPos;
    private bool isTouching = false;
    private bool isSpawning = false; // Prevent overlap

    private void Start()
    {
        cam = Camera.main;

        // === VALIDATION ===
        if (spawnPoints.Count == 0)
        {
            Debug.LogError("Spawner: No spawn points assigned!");
            return;
        }
        if (dropButton == null)
        {
            Debug.LogError("Spawner: Drop Button not assigned!");
            return;
        }

        // === BUTTON SETUP ===
        dropButton.onClick.RemoveAllListeners();
        dropButton.onClick.AddListener(TryDropPiece);

        // === INITIAL SPAWN ===
        nextValue = GetRandomStartValue();
        nextColor = BallColorUtility.GetRandomColor(); // NEW: Random color
        UpdatePreview(nextValue, nextColor);
        SpawnHeldPiece();
    }

    private void Update()
    {
        if (GameManager.I != null && GameManager.I.IsGameOver()) return;

        HandleSwipeInput();

        if (heldPiece != null)
            MoveHeldPieceToCurrentPoint();
    }

    #region Swipe Navigation
    private void HandleSwipeInput()
    {
         if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                lastTouchPos = t.position;
                isTouching = true;
            }
            else if (t.phase == TouchPhase.Moved && isTouching)
            {
                //StartCoroutine(ProcessSwipe(t.position));
                
            }
            else if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
            {
                ProcessSwipe(t.position);
                isTouching = false;
            }
        }
    }

   
    private void ProcessSwipe(Vector2 currentPos)
    {
        Vector2 delta = currentPos - lastTouchPos;
        if (Mathf.Abs(delta.x) > swipeThreshold)
        {
            if (delta.x < 0)
                currentSpawnIndex = Mathf.Clamp(currentSpawnIndex-1,0,spawnPoints.Count-1);
                //currentSpawnIndex = (currentSpawnIndex - 1 + spawnPoints.Count) % spawnPoints.Count;
            else
                currentSpawnIndex = Mathf.Clamp(currentSpawnIndex+1,0,spawnPoints.Count-1);
                //currentSpawnIndex = (currentSpawnIndex + 1) % spawnPoints.Count;
           
            lastTouchPos = currentPos;
        }
        else
        {
            lastTouchPos = currentPos;
        }
    }
    #endregion

    private void MoveHeldPieceToCurrentPoint()
    {
        if (heldPiece == null || spawnPoints[currentSpawnIndex] == null) return;

        
        Vector3 target = spawnPoints[currentSpawnIndex].position;
        heldPiece.transform.position = Vector3.Lerp(
            heldPiece.transform.position,
            target,
            Time.deltaTime * pieceMoveSpeed
        );
    }

    // Called by UI Drop Button
    private void TryDropPiece()
    {
        if (heldPiece == null || isSpawning) return;

        heldPiece.SetHeld(false);
        heldPiece.isDroped = true;
        heldPiece = null;

        // Start 1-second delay with coroutine
        StartCoroutine(SpawnNextPieceAfterDelay());
    }

    // 1-second delay using Coroutine
    private IEnumerator SpawnNextPieceAfterDelay()
    {
        isSpawning = true;
        yield return new WaitForSeconds(spawnDelay);
        SpawnHeldPiece();
        isSpawning = false;
    }

    private void SpawnHeldPiece()
    {
        if (numberPiecePrefab == null || spawnPoints.Count == 0) return;

        Transform spawnPoint = spawnPoints[currentSpawnIndex];
        
        heldPiece = Instantiate(numberPiecePrefab, spawnPoint.position, Quaternion.identity);
        heldPiece.SetHeld(true);
        heldPiece.InitializeValue(nextValue, nextColor); // NEW: Pass color

        // Prepare next preview
        nextValue = GetRandomStartValue();
        nextColor = BallColorUtility.GetRandomColor(); // NEW: Random color
        UpdatePreview(nextValue, nextColor);
        GameManager.I?.SetNextValue(nextValue);
    }

    private void UpdatePreview(int value, BallColor color)
    {
        if (previewSprite != null)
            previewSprite.color = BallColorUtility.GetUnityColor(color); // NEW: Show color
        if (previewLabel != null)
            previewLabel.text = value.ToString();
    }

    private int GetRandomStartValue()
    {
        int r = Random.Range(0, 100);
        if (r < 60) return 2;
        if (r < 85) return 4;
        if (r < 95) return 8;
        return 16;
    }

    // Visual debug in Scene view
    private void OnDrawGizmosSelected()
    {
        if (spawnPoints.Count == 0) return;

        for (int i = 0; i < spawnPoints.Count; i++)
        {
            if (spawnPoints[i] == null) continue;
            Gizmos.color = (i == currentSpawnIndex) ? Color.green : Color.yellow;
            Gizmos.DrawWireSphere(spawnPoints[i].position, 0.3f);
        }
    }
}