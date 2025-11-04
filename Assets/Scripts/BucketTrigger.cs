using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Collider2D))]
public class BucketTrigger : MonoBehaviour
{
    public int requiredValue;
    private bool isComplete = false;
    [SerializeField] private SpriteRenderer checkMark;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay2D(Collider2D other)
    {
        var piece = other.GetComponent<NumberPiece>();
        if(piece.value==requiredValue)
        {
            Debug.Log("Condition Complete");
            checkMark.enabled = true;
            isComplete = true;
        }
    }

    public bool IsComplete()
    {
        return isComplete;
    }
}
