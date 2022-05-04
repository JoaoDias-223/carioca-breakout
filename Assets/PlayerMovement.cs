using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 5f;
    public Vector2 movement;
    
    public Rigidbody2D rb2D;
    public Animator animator;

    private const string ANIM_HORIZONTAL_PARAM = "Horizontal";
    private const string ANIM_VERTICAL_PARAM = "Vertical";
    private const string ANIM_SPEED_PARAM = "Speed";
    
    private void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        
        animator.SetFloat(ANIM_HORIZONTAL_PARAM, movement.x);
        animator.SetFloat(ANIM_VERTICAL_PARAM, movement.y);
        animator.SetFloat(ANIM_SPEED_PARAM, movement.sqrMagnitude);
    }

    private void FixedUpdate()
    {
        rb2D.MovePosition(rb2D.position + Time.fixedDeltaTime * movementSpeed * movement);
    }
}
