using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destruction : MonoBehaviour
{
    [SerializeField] int hp;

    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void ObjectDamaged()
    {
        if(anim.GetBool("Destroy")){
            return;
        }

        hp--;
        if(hp <= 0)
        {
            anim.SetBool("Destroy",true);
        }
    }

}
