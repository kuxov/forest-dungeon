using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    Rigidbody2D body;
    Animator animator;
    public int hp = 300;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hp <= 200)
        {
            animator.SetTrigger("hit");
        }
        else if (hp <= 100)
        {
            animator.SetTrigger("dead");
        }
    }
}
