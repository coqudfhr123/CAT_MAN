using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    Collider2D collider;
    Rigidbody2D rigid;
    Animator anim;
    
    bool isStop;

    void Start()
    {
        collider = GetComponent<Collider2D>();
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        rigid.velocity = new Vector2(transform.localScale.x * 30, rigid.velocity.y);
    }

    void OnTriggerEnter2D(Collider2D collision)//화살이 바닥에 떨어점
    {
        if(collision.gameObject.tag == "Enemy"){
            collision.GetComponent<Enemy>().ArrowDamage();//적의 화살 맞은것을 전달함
            
            if(collision.GetComponent<Enemy>().EnemyType == "DogShield" && collision.GetComponent<Enemy>().isArrowHunt == false){//방패병일때 화살이 튕기는것을 나타냄
                rigid.drag = 10000f;
                rigid.gravityScale = 0f;
                anim.SetInteger("state", 2);
                isStop = true;
                Debug.Log("Shield");
                return;
            }
        }
        if(collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Platform" || collision.gameObject.tag == "Box"){
            gameObject.SetActive(false);
        }
        // gameObject.tag = "Untagged"; //더이상 적이 맞지 않도록 설정함
        // transform.SetParent(collision.gameObject.transform);//화살이 벽에 밖힘
        // rigid.gravityScale = 0;//설정하지 않으면 화살이 떨어짐
        // rigid.mass = 0.0001f;
        // Invoke("Delete", 0.1f);
        // rigid.bodyType = RigidbodyType2D.Kinematic;
    }

    public void Delay()
    {
        Invoke("ArrowBreak", 0.09f);
    }

    void ArrowBreak()//Delay함수에서 실행됨
    {
        rigid.drag = 10000f;
        rigid.gravityScale = 0f;
        anim.SetInteger("state",1);
        isStop = true;
    }
    
    public void Delete()//화살 애니메이션이 끝나면 화살을 삭제함
    {
        gameObject.SetActive(false);
    }
}
