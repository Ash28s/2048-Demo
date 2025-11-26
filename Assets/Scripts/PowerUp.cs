using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private Button deleteButton;
    [SerializeField] private Button blastButton;
    [SerializeField] private Button changeColorButton;
    [SerializeField] private Spawner spawner;
    public LayerMask numberPieceLayer;

    [SerializeField] private GameObject deleteEffect;
    [SerializeField] private GameObject blastEffect;
    [SerializeField] private GameObject chanageEffect;
    
    [SerializeField] private TMP_Text deleteAmountText;
    [SerializeField] private TMP_Text blastAmountText;
    [SerializeField] private TMP_Text changeColorAmountText;

    [SerializeField] private Image deleteImg;
    [SerializeField] private Image blastImg;
    [SerializeField] private Image chanageImg;

    public Color unSelected;
    public Color selected;
    private bool isDelete = false;
    private bool isBlast = false;
    private bool isChangeColor = false;
    private int deleteAmount;
    private int blastAmount;
    private int changeColorAmount;
    // Start is called before the first frame update
    void Start()
    {
        if(deleteButton!=null)
            deleteButton.onClick.AddListener(DeleteBall);
        if(blastButton!=null)
            blastButton.onClick.AddListener(BlastDeleteBalls); 
        if(changeColorButton!=null)
            changeColorButton.onClick.AddListener(ChangeColor);       
        if(spawner==null)
            spawner = FindObjectOfType<Spawner>();    

        if(deleteImg!=null)
            deleteImg.color = unSelected;
        if(blastImg!=null)
            blastImg.color = unSelected;
        if(chanageImg!=null)
            chanageImg.color = unSelected;    

        int level = PlayerPrefs.GetInt("Level",1);
        deleteAmount = Mathf.Max(1,(level-1)*(int)(Random.Range(0.1f,2f)));
        blastAmount=Mathf.Max(1,(level-1)*(int)(Random.Range(0.1f,2f)));
        changeColorAmount = Mathf.Max(1,(level-1)*(int)(Random.Range(0.05f,1.1f)));
        deleteAmountText.text = "x"+deleteAmount.ToString("0");
        blastAmountText.text = "x"+blastAmount.ToString("0");
        changeColorAmountText.text = "x"+changeColorAmount.ToString("0");

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
                RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero, Mathf.Infinity,numberPieceLayer);
                if(hit.collider!=null && hit.collider.tag=="NumberPiece")
                {
                    if(hit.collider.gameObject.GetComponent<NumberPiece>().isDroped==false)
                            return;
                    Debug.Log(hit.collider.name);
                    if(isDelete)
                    {
                        GameObject effect = Instantiate(deleteEffect,touchPos,transform.rotation);
                        Destroy(effect,0.5f);
                        Destroy(hit.collider.gameObject);
                        isDelete = false;
                        deleteAmount--;
                        deleteImg.color = unSelected;
                    }   
                    else if(isBlast)
                    {
                        GameObject effect = Instantiate(blastEffect,touchPos,transform.rotation);
                        Destroy(effect,0.5f);
                        Collider2D[] colliders = Physics2D.OverlapCircleAll(touchPos, 1f);
                        foreach (Collider2D col in colliders)
                        {
                            if(col.tag=="NumberPiece"){
                                Destroy(col.gameObject);}
                        }      
                        isBlast = false;
                        blastAmount--;
                        blastImg.color = unSelected;
                    }
                    else if(isChangeColor)
                    {
                        GameObject effect = Instantiate(chanageEffect,touchPos,transform.rotation);
                        Destroy(effect,0.5f);
                        hit.collider.gameObject.GetComponent<NumberPiece>().ChangeColor();
                        isChangeColor = false;
                        changeColorAmount--;
                        chanageImg.color = unSelected;
                    }
                    spawner.isPowerUpUse = false;
                    deleteAmountText.text = "x"+deleteAmount.ToString("0");
                    blastAmountText.text = "x"+blastAmount.ToString("0");
                    changeColorAmountText.text = "x"+changeColorAmount.ToString("0");
                    
                }
                else
                {
                    isBlast = false;
                    isDelete = false;
                    spawner.isPowerUpUse = false;
                }
            }
        }
        
    }

    private void DeleteBall()
    {
        if(isBlast==false&&isChangeColor==false&&deleteAmount>0)
        {
            isDelete = true;
            spawner.isPowerUpUse = true;
            deleteImg.color = selected;
        }
    }

    private void BlastDeleteBalls()
    {
        if(isDelete==false&&isChangeColor==false&&blastAmount>0){
            isBlast = true;
            spawner.isPowerUpUse = true;
            blastImg.color = selected;
        }
    }

    private void ChangeColor()
    {
        if(isDelete==false&&isBlast==false&&changeColorAmount>0)
        {
            isChangeColor=true;
            spawner.isPowerUpUse = true;
            chanageImg.color = selected;
        }

    }


}
