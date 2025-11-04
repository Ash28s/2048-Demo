using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class NumberPiece : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private TMP_Text label;
    [SerializeField] private SpriteRenderer sr;

    [Header("Tuning")]
    [SerializeField] private float mergeCheckSpeedThreshold = 2.2f;
    [SerializeField] private float mergeCooldownAfterSpawn = 0.12f;
    [SerializeField] private float chainMergeDelay = 0.06f;
    [SerializeField] private float mergeAttractionTime = 0.06f;

    [SerializeField] private float baseScale = 0.9f;
    [SerializeField] private float scalePerPower = 0.06f;
    [SerializeField] private float maxScale = 1.6f;

    private Rigidbody2D rb;
    private Collider2D col;

    public int value { get; private set; }
    private bool held;
    private bool isMerging;
    private float spawnTime;
    private int lastMergeFrame;

    private void Reset()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (label == null)
        {
            label = GetComponentInChildren<TMP_Text>(true);
        }
    }

    private void Start()
    {
        spawnTime = Time.time;
        UpdateVisuals();
        ApplyHeldState();
    }

    public void InitializeValue(int v)
    {
        value = Mathf.Max(2, v);
        UpdateVisuals();
    }

    public void SetHeld(bool h)
    {
        held = h;
        ApplyHeldState();
    }

    private void ApplyHeldState()
    {
        if (rb == null) return;
        rb.isKinematic = held;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        if (held)
        {
            transform.rotation = Quaternion.identity;
        }
    }

    private void UpdateVisuals()
    {
        if (label != null) label.text = value.ToString();
        if (sr != null) sr.color = ValueColors.GetColor(value);
        float scale = Mathf.Min(maxScale, baseScale + scalePerPower * Mathf.Log(value, 2f));
        transform.localScale = Vector3.one * scale;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isMerging || held) return;
        if (Time.time - spawnTime < mergeCooldownAfterSpawn) return;

        // Check relative speed – if too fast, wait until it settles a bit
        if (rb.velocity.sqrMagnitude > mergeCheckSpeedThreshold * mergeCheckSpeedThreshold) return;

        var other = collision.collider.GetComponent<NumberPiece>();
        if (other == null || other == this) return;
        if (other.isMerging || other.held) return;

        if (other.value != this.value) return;

        // Deterministic master selection to avoid double merges
        bool iAmMaster = GetInstanceID() > other.GetInstanceID();

        if (iAmMaster)
        {
            // Avoid re-merge same frame
            if (lastMergeFrame == Time.frameCount || other.lastMergeFrame == Time.frameCount) return;
            StartCoroutine(CoMergeWith(other));
        }
    }

    private IEnumerator CoMergeWith(NumberPiece other)
    {
        if (other == null) yield break;
        if (isMerging || other.isMerging) yield break;

        isMerging = true;
        other.isMerging = true;
        lastMergeFrame = Time.frameCount;
        other.lastMergeFrame = Time.frameCount;

        // Temporarily disable collisions/physics on other and pull it towards us
        Rigidbody2D orb = other.rb;
        Collider2D ocol = other.col;
        var otherOrigKinematic = orb.isKinematic;
        orb.isKinematic = true;
        orb.velocity = Vector2.zero;
        orb.angularVelocity = 0f;

        if (ocol != null) ocol.enabled = false;

        Vector3 target = (transform.position + other.transform.position) * 0.5f;

        float t = 0f;
        Vector3 a0 = transform.position;
        Vector3 b0 = other.transform.position;
        while (t < mergeAttractionTime)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / mergeAttractionTime);
            transform.position = Vector3.Lerp(a0, target, u * 0.6f);
            other.transform.position = Vector3.Lerp(b0, target, u);
            yield return null;
        }

        // Perform merge: this piece survives, other is destroyed
        int newValue = this.value * 2;
        GameManager.I?.AddScore(newValue);

        // Pop animation
        Vector3 originalScale = transform.localScale;
        transform.localScale = originalScale * 1.1f;
        yield return null;

        // Update value and visuals
        this.value = newValue;
        UpdateVisuals();

        // Restore, then destroy other
        orb.isKinematic = otherOrigKinematic;
        if (ocol != null) ocol.enabled = true;
        Destroy(other.gameObject);

        // Small settle delay, then check for chain merges
        yield return new WaitForSeconds(chainMergeDelay);
        isMerging = false;

        // Chain: find overlapping same-value pieces
        TryChainMerge();
    }

    private void TryChainMerge()
    {
        if (isMerging || held) return;
        float radius = 0.55f * transform.localScale.x;
        var hits = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (var h in hits)
        {
            if (h == null || h.attachedRigidbody == rb) continue;
            var np = h.GetComponent<NumberPiece>();
            if (np == null || np == this) continue;
            if (np.held || np.isMerging) continue;
            if (np.value == this.value)
            {
                // Deterministic master selection again
                bool iAmMaster = GetInstanceID() > np.GetInstanceID();
                if (iAmMaster)
                {
                    StartCoroutine(CoMergeWith(np));
                    break;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize chain radius
        Gizmos.color = new Color(1f, 1f, 0f, 0.25f);
        float radius = 0.55f * (transform.localScale == Vector3.zero ? 1f : transform.localScale.x);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}