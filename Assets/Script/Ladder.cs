using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    enum LadderPart {complete, bottom, top};
    [SerializeField] LadderPart part = LadderPart.complete;

    
    void OnTriggerStay2D(Collider2D collision)
    {
        PlayerMove playerMove = collision.GetComponent<PlayerMove>();
            switch(part)
            {
                case LadderPart.complete:
                    if(Input.GetKeyDown(KeyCode.UpArrow))
                    {   
                        playerMove.isLadder = true;
                        playerMove.ladder = this;//자기 자신의 스크립트를 플레이어에 넘김
                    }
                    break;
        }
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<PlayerMove>())
        {
            PlayerMove playerMove = collision.GetComponent<PlayerMove>();
            switch(part)
            {
                case LadderPart.bottom://닿으면 동작을 취소 하는 기능
                    if(playerMove.isLadder == true){//사다리 오르는 동작이 켜져있어야 꺼지는 기능을 사용함
                        playerMove.bottomLadder = true;
                        playerMove.isLadder = false;
                        
                        collision.GetComponent<Rigidbody2D>().gravityScale = 4f; 
                    }
                    
                    break;

                case LadderPart.top:
                    playerMove.topLadder = true;
                    break;    
            }
        }

    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.GetComponent<PlayerMove>())
        {
            PlayerMove playerMove = collision.GetComponent<PlayerMove>();
            switch(part)
            {
                case LadderPart.complete:
                    playerMove.isLadder = false;
                    break;

                case LadderPart.bottom:
                    playerMove.bottomLadder = false;
                    break;

                case LadderPart.top:
                    playerMove.topLadder = false;
                    break;                
            }
        }
    }
}

