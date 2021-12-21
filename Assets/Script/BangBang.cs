using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BangBang : MonoBehaviour
{
    Animator anim;

    [SerializeField] Transform bangBangTransform;

    [SerializeField] GameObject playerGameobject;
    [SerializeField] Transform playerTransform;
    Rigidbody2D playerRigid;

    [SerializeField] bool bangBang = false;
    [SerializeField] float speed = 0.6f;
    [SerializeField] float jumpForce = 30f;

    [SerializeField] Transform desPos;
    [SerializeField] Transform upPos;
    [SerializeField] Transform downPos;

    void Start()
    {
        anim = GetComponent<Animator>();

        playerRigid = playerGameobject.GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        bangBangTransform.position = Vector2.MoveTowards(bangBangTransform.position, desPos.position, Time.deltaTime * speed);

        if(!bangBang){
            desPos = upPos;
            speed = 3f;
            return;
        }
        else if(Vector2.Distance(bangBangTransform.position, desPos.position) <= 0f){     
            if(desPos == downPos){
                desPos = upPos;
                speed = 3f;
            } 
            else{
                desPos = downPos;
                speed = 0.6f;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {   
        if(other.gameObject.tag == "Player"){
            if(playerTransform.position.y - transform.position.y < 1.4f && playerTransform.position.y - transform.position.y > 0.9f){
                bangBang = true;
                anim.SetBool("isBangBang",true);
                other.transform.SetParent(transform);//발판에 닿으면 발판 움직임을 플레이어가 따라감
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player"){
            bangBang = false;
            anim.SetBool("isBangBang",false);
            other.transform.SetParent(null);//발판에 닿으면 발판 움직임을 플레이어가 따라감            
        }
    }

    public void JumpUp()
    {
        if(bangBang){//점프업 함수가 실행되기전 방방이를 벗어나면 높은 점프가 실행되지 않음
            playerRigid.velocity = new Vector2(playerRigid.velocity.x , jumpForce);//방방이로 인한 점프력 증가
            playerGameobject.GetComponent<PlayerMove>().isJump = true;
            //점프 애니메이션은 따로 하지 않을것임
        }
    }
}
