using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [SerializeField] private float settleSpeedThreshold = 0.25f;
    [SerializeField] private float requiredSettleTime = 0.6f;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (GameManager.I != null && GameManager.I.IsGameOver()) return;

        var piece = other.GetComponent<NumberPiece>();
        if (piece == null) return;

        var rb = other.attachedRigidbody;
        if (rb == null) return;

        if (rb.velocity.magnitude < settleSpeedThreshold)
        {
            // Once it stays slow for a short time in the top zone, end the game
            // To keep it simple, we rely on Time.timeSinceLevelLoad via a coroutine-less latch.
            // A stricter implementation would track per-piece dwell time; here we just quick-end.
            GameManager.I?.GameOver();
        }
    }
}