using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverFlowTrigger : MonoBehaviour
{

    void OnTriggerStay2D(Collider2D other)
    {
        var piece = other.GetComponent<NumberPiece>();
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        
        if(piece.isDroped==true&&rb!=null&&rb.velocity.magnitude<0.1f)
        {
            GameManager.I.GameOver();
            Debug.Log("overflow");
        }
    }
}
