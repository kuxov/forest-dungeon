using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    Vector2 movement;
    public Animator animator;

    public Joystick joystick;

  
    void Update()
    {
        movement.x = joystick.Horizontal;
        movement.y = joystick.Vertical;

        if (movement != Vector2.zero && !DialogManager.GetInstance().dialogPlaying)
        {
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
        }

        if (!DialogManager.GetInstance().dialogPlaying)
        {
            animator.SetFloat("Speed", movement.sqrMagnitude);
        }
    }

    void FixedUpdate()
    {
        if (DialogManager.GetInstance().dialogPlaying)
        {
            return;
        }
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
