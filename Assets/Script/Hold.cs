using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hold : MonoBehaviour
{
    Collider2D collider;

    void Start()
    {
        collider = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player"){
            other.gameObject.GetComponent<PlayerMove>().isHold = true;
            other.gameObject.GetComponent<PlayerMove>().isJump = false;
            other.gameObject.GetComponent<PlayerMove>().hold_y = transform.position.y;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player"){
            other.gameObject.GetComponent<PlayerMove>().isHold = false;
        }
    }


}
