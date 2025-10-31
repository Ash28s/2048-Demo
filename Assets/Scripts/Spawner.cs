using UnityEngine;
using TMPro;

public class Spawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private NumberPiece numberPiecePrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float topY = 4.5f;
    [SerializeField] private float horizontalMargin = 0.6f;
    [SerializeField] private float holdFollowSpeed = 20f;

    [Header("Next Preview (Optional)")]
    [SerializeField] private SpriteRenderer previewSprite;
    [SerializeField] private TMP_Text previewLabel;

    private Camera cam;
    private NumberPiece heldPiece;
    private int nextValue;
    private float minX, maxX;

    private void Start()
    {
        cam = Camera.main;
        ComputeHorizontalBounds();
        nextValue = GetRandomStartValue();
        UpdatePreview(nextValue);
        SpawnHeldPiece();
    }

    private void ComputeHorizontalBounds()
    {
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;
        minX = -halfW + horizontalMargin;
        maxX = +halfW - horizontalMargin;
    }

    private void Update()
    {
        if (GameManager.I != null && GameManager.I.IsGameOver()) return;

        // Recompute bounds if aspect changes (rare at runtime)
        // ComputeHorizontalBounds();

        float targetX = GetPointerWorldX();
        targetX = Mathf.Clamp(targetX, minX, maxX);

        Vector3 spawnerPos = transform.position;
        spawnerPos.x = targetX;
        spawnerPos.y = topY;
        transform.position = spawnerPos;

        if (heldPiece != null)
        {
            // Smooth follow to reduce jitter
            Vector3 hp = heldPiece.transform.position;
            hp.x = Mathf.Lerp(hp.x, targetX, Time.deltaTime * holdFollowSpeed);
            hp.y = topY;
            heldPiece.transform.position = hp;

            if (DidPress())
            {
                DropHeldPiece();
                Invoke(nameof(SpawnHeldPiece), 0.15f);
            }
        }
    }

    private float GetPointerWorldX()
    {
        Vector3 p = Input.mousePosition;
        if (Input.touchCount > 0)
        {
            p = Input.GetTouch(0).position;
        }
        Vector3 world = cam.ScreenToWorldPoint(p);
        return world.x;
    }

    private bool DidPress()
    {
        bool mouse = Input.GetMouseButtonDown(0);
        bool touch = false;
        if (Input.touchCount > 0)
        {
            var t = Input.GetTouch(0);
            touch = t.phase == TouchPhase.Began;
        }
        return mouse || touch;
    }

    private void SpawnHeldPiece()
    {
        if (numberPiecePrefab == null) return;

        heldPiece = Instantiate(numberPiecePrefab, new Vector3(transform.position.x, topY, 0f), Quaternion.identity);
        heldPiece.SetHeld(true);
        int v = nextValue;
        heldPiece.InitializeValue(v);
        nextValue = GetRandomStartValue();
        UpdatePreview(nextValue);
        GameManager.I?.SetNextValue(nextValue);
    }

    private void DropHeldPiece()
    {
        if (heldPiece == null) return;
        heldPiece.SetHeld(false);
        heldPiece = null;
    }

    private void UpdatePreview(int value)
    {
        if (previewSprite != null)
        {
            previewSprite.color = ValueColors.GetColor(value);
        }
        if (previewLabel != null)
        {
            previewLabel.text = value.ToString();
        }
    }

    // Tune starting values distribution here
    private int GetRandomStartValue()
    {
        // Weights: 2 (60%), 4 (25%), 8 (10%), 16 (5%)
        int r = Random.Range(0, 100);
        if (r < 60) return 2;
        if (r < 85) return 4;
        if (r < 95) return 8;
        return 16;
    }
}