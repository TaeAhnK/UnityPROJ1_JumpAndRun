using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Move : MonoBehaviour
{
    public int nextMove;            // Randomly Selected Direction
    Rigidbody2D rigid;              // Rigidbody of Enemy
    SpriteRenderer spriteRenderer;  // SpriteRenderer of Ememy
    Animator anim;                  // Animator of Ememy
    CapsuleCollider2D collid;       // Collider of Enemy
    
    void Awake()
    {
        // init
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        collid = GetComponent<CapsuleCollider2D>();

        // Set Direction
        Movement();
    }

    void FixedUpdate() {
        FixedUpdate_Run();
    }

///////////////////////////////////////////////////////////////////////////////////////////////////

    void FixedUpdate_Run()
    {
        // Set Velocity and Direction
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        // Check for Cliff
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.5f, rigid.position.y);
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 2, LayerMask.GetMask("Floor"));
        if (rayHit.collider == null)
        {
            Turn();
        } 
    }

    void Movement()
    {
        // Randomly & Recursively Decide Next Move
        nextMove = Random.Range(-1,2);
        
        // Direction Right
        if (nextMove == 1)
        {
            anim.SetBool("IsRunning", true);
            spriteRenderer.flipX = true;
        }
        // Direction Left
        else if (nextMove == -1)
        {
            anim.SetBool("IsRunning", true);
            spriteRenderer.flipX = false;
        }
        // Stop
        else 
        {  // nextMove == 0
            anim.SetBool("IsRunning", false);
        }   
        
        // Change Movement
        Invoke("Movement", Random.Range(2,6));
    }

    void Turn()
    {
        // Turn on Cliff
        nextMove *= -1;
        spriteRenderer.flipX = (nextMove == 1);
        CancelInvoke();
        Invoke("Movement", Random.Range(2,6));
    }

///////////////////////////////////////////////////////////////////////////////////////////////////

    public void OnDamaged()
    {
        // Stumped by Player
        spriteRenderer.color = new Color(1,1,1, 0.4f);
        spriteRenderer.flipY = true;
        collid.enabled = false;
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        Invoke("DeActive", 5);
    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }
}

///////////////////////////////////////////////////////////////////////////////////////////////////
