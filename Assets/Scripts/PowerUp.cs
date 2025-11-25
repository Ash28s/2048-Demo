using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private bool isDelete = false;
    private bool isBlast = false;
    private bool isChangeColor = false;
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
                    }
                    else if(isChangeColor)
                    {
                        GameObject effect = Instantiate(chanageEffect,touchPos,transform.rotation);
                        Destroy(effect,0.5f);
                        hit.collider.gameObject.GetComponent<NumberPiece>().ChangeColor();
                        isChangeColor = false;
                    }
                    spawner.isPowerUpUse = false;
                    
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
        if(isBlast==false&&isChangeColor==false)
        {
            isDelete = true;
            spawner.isPowerUpUse = true;
        }
    }

    private void BlastDeleteBalls()
    {
        if(isDelete==false&&isChangeColor==false){
            isBlast = true;
            spawner.isPowerUpUse = true;
        }
    }

    private void ChangeColor()
    {
        if(isDelete==false&&isBlast==false)
        {
            isChangeColor=true;
            spawner.isPowerUpUse = true;
        }

    }


}
