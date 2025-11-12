using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
[RequireComponent(typeof(Collider2D))]
public class BucketTrigger : MonoBehaviour
{
    public int requiredValue;
    [SerializeField] private BallColor requiredColor;
    private bool isComplete = false;
    [SerializeField] private SpriteRenderer checkMark;
    [SerializeField] private SpriteRenderer bgSprite;
    [SerializeField] private TMP_Text requiredValueTxt;
    int level = 1;

    void Start()
    {
        level = PlayerPrefs.GetInt("Level",1);
        SelectRandomValue();
        requiredColor = BallColorUtility.GetRandomColor();
        Color reqColor = BallColorUtility.GetUnityColor(requiredColor);
        bgSprite.color = new Color(reqColor.r,reqColor.g,reqColor.b,0.5f);
        requiredValueTxt.text = requiredValue.ToString("0");
    }

    void OnTriggerStay2D(Collider2D other)
    {
        var piece = other.GetComponent<NumberPiece>();
        if(piece.value==requiredValue && piece.ballColor==requiredColor)
        {
            Debug.Log("Condition Complete:"+requiredColor+" "+requiredValue);
            checkMark.enabled = true;
            isComplete = true;
        }
    }

    public bool IsComplete()
    {
        return isComplete;
    }

    void SelectRandomValue()
    {
        if(level<7)
        {
        int rand = Random.Range(0,4);
        switch(rand)
        {
            case 0:requiredValue = 16;
                    break;
            case 1:requiredValue = 32;
                    break;
            case 2:requiredValue = 64;
                    break;
            case 3:requiredValue = 128;
                    break;                        
        }
        }
        else
        {
            requiredValue = (int)(Mathf.Pow(2,Random.Range(level-3,level)));
            requiredValue = Mathf.Clamp(requiredValue,2,2048);
        }
    }
}
