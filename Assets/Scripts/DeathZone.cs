using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [SerializeField] private float settleSpeedThreshold = 0.25f;
    [SerializeField] private float requiredSettleTime = 0.6f;

    private readonly Dictionary<Rigidbody2D, float> dwell = new();

    private void OnTriggerStay2D(Collider2D other)
    {
        if (GameManager.I != null && GameManager.I.IsGameOver()) return;

        var rb = other.attachedRigidbody;
        if (rb == null) return;

        if (!dwell.ContainsKey(rb))
            dwell[rb] = 0f;

        if (rb.velocity.magnitude < settleSpeedThreshold)
        {
            dwell[rb] += Time.deltaTime;
            if (dwell[rb] >= requiredSettleTime)
            {
                GameManager.I?.GameOver();
            }
        }
        else
        {
            // Reset if it’s still moving fast
            dwell[rb] = 0f;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var rb = other.attachedRigidbody;
        if (rb != null && dwell.ContainsKey(rb))
        {
            dwell.Remove(rb);
        }
    }
}