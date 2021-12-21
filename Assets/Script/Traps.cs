using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traps : MonoBehaviour
{
    [SerializeField] int trapTypes;

    [SerializeField] float curShotDelay;
    [SerializeField] float maxShotDelay;

    Animator anim;

    public GameObject bulletArrow;
    public ObjectManager objectManager;
    
    
    void Start()
    {
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        switch (trapTypes)
        {
            case 1://석궁
                if(!anim.GetBool("reload")){
                    curShotDelay += Time.deltaTime;

                    if(curShotDelay > maxShotDelay){
                        anim.SetBool("reload",true);
                        curShotDelay = 0;
                    }
                }
                else{
                    curShotDelay += Time.deltaTime;

                    if(curShotDelay > maxShotDelay){
                        GameObject bulletArrow = objectManager.MakeObj("BulletArrow");
                        Rigidbody2D rigid = bulletArrow.GetComponent<Rigidbody2D>();
                        SpriteRenderer spriteBullet = bulletArrow.GetComponent<SpriteRenderer>();

                        bulletArrow.transform.localScale = new Vector2(-1, 1);
                        bulletArrow.transform.position = new Vector2(transform.position.x + (1.5f * transform.localScale.x) , transform.position.y);
                        
                        anim.SetBool("reload",false);
                        curShotDelay = 0;
                    }
                }
                break;

            case 2:
                break;

            case 3:
                break;    
        }
    }
}
