using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    Collider2D collider;
    Rigidbody2D rigid;

    public float huntForce;

    void Start()
    {
        collider = GetComponent<Collider2D>();
        rigid = GetComponent<Rigidbody2D>();
    }

    void OnTriggerEnter2D(Collider2D collision)//화살이 바닥에 떨어점
    {
        if(collision.gameObject.tag == "Player"){

            collision.GetComponent<PlayerMove>().Hunt();//적의 화살 맞은것을 전달함
            Rigidbody2D playerRigid = collision.GetComponent<Rigidbody2D>();

            collision.GetComponent<PlayerMove>().isHunt = true;//플레이어가 공격 당함을 전달함

            if(transform.position.x < collision.transform.position.x){
                playerRigid.velocity = new Vector2(huntForce / 3, huntForce / 2);
                collision.transform.localScale = new Vector2(-1, 1); 
            }
            else{
                playerRigid.velocity = new Vector2(-huntForce / 3, huntForce / 2);
                collision.transform.localScale = new Vector2(1, 1); 
            }
        }
        if(collision.gameObject.tag == "Player" || collision.gameObject.tag == "PlayerDamaged" ||
        collision.gameObject.tag == "Platform" || collision.gameObject.tag == "Box"){
            gameObject.SetActive(false);
        }
        // gameObject.tag = "Untagged"; //더이상 적이 맞지 않도록 설정함
        // transform.SetParent(collision.gameObject.transform);//화살이 벽에 밖힘
        // rigid.gravityScale = 0;//설정하지 않으면 화살이 떨어짐
        // rigid.mass = 0.0001f;
        // Invoke("Delete", 0.1f);
        // rigid.bodyType = RigidbodyType2D.Kinematic;
    }
}
