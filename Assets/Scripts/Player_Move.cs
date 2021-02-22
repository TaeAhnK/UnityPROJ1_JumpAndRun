using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Move : MonoBehaviour
{
    public float maxSpeed;          // Max Speed to Limit Velocity
    public float jumpPower;         // Jump Height
    public float monsterPower;      // How Much Player Moves when Hitted
    public GameManager gamemanager; // GameManager
    public AudioClip audioJump;     // Sound Jump
    public AudioClip audioAttack;   // Sound Attack
    public AudioClip audioDamaged;  // Sound Damage
    public AudioClip audioItem;     // Sound Item
    public AudioClip audioDie;      // Sound Die
    public AudioClip audioFinish;   // Sound Finish

    Rigidbody2D rigid;              // Rigidbody of Player
    SpriteRenderer spriteRenderer;  // SpriteRenderer of Player
    Animator anim;                  // Animator of Player
    CapsuleCollider2D collid;       // Collider of Player
    AudioSource audioSource;        // Audio Sorce of Game


    void Awake()
    {
        // Init
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        collid = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        FixedUpdate_Run();
    }

    void Update()
    {
        Update_Run();
        Update_Jump();
    }

///////////////////////////////////////////////////////////////////////////////////////////////////

    void FixedUpdate_Run()
    {
        // Horizontal Move Input & Pysics
        float h = Input.GetAxisRaw("Horizontal");
        
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if (rigid.velocity.x > maxSpeed) 
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }
        else if (rigid.velocity.x < maxSpeed * -1) 
        {
            rigid.velocity = new Vector2(maxSpeed * -1, rigid.velocity.y);
        }
    }

    void Update_Run() 
    {
        // Stop Speed by Time
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }
        
        // Direction Right
        if (rigid.velocity.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        // Direction Left
        else if (rigid.velocity.x > 0)
        {
            spriteRenderer.flipX = false;
        }

        // Running Motion
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
        {
            anim.SetBool("IsRunning", false);
        }
        else 
        {
            anim.SetBool("IsRunning", true);
        }
    }

///////////////////////////////////////////////////////////////////////////////////////////////////

    void Update_Jump() 
    {
        // Jump
        if (Input.GetButtonDown("Jump") && anim.GetBool("IsJumping") == false)
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("IsJumping", true);
            SoundPlay("JUMP");
        }

        // Stop Jumping Motion on Floor
        if (rigid.velocity.y <= 0)
        {
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Floor"));
            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.8f)
                {
                    anim.SetBool("IsJumping", false);
                }
            }            
        }
    }

///////////////////////////////////////////////////////////////////////////////////////////////////

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            // Stamping On Monster
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform);
            }
            // Hit by Monster
            else 
            {
                OnDamaged(collision.transform.position);
            }
        }
    }

    void OnDamaged(Vector2 targetPos)
    {
        SoundPlay("DAMAGED");
        // Set Layer
        gameObject.layer = 11;  // 11: PlayerDamaged
        
        // Change Color
        spriteRenderer.color = new Color(1,1,1, 0.4f);
        
        // Force Back
        int hitDirection = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(hitDirection, 1) * monsterPower,ForceMode2D.Impulse);
        
        // Minus Life
        gamemanager.LifeMinus();

        // Wait
        Invoke("OffDamage", 2);
    }

    void OffDamage()
    {
        // Set Layer Back
        gameObject.layer = 10;  // 10: Player
        
        // Change Color Back
        spriteRenderer.color = new Color(1,1,1,1);
    }

    void OnAttack(Transform enemy)
    {
        // Stumped Monster
        SoundPlay("ATTACK");
        // Jump Up
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        
        // Kill Monster
        Enemy_Move enemyMove = enemy.GetComponent<Enemy_Move>();
        enemyMove.OnDamaged();

        // Get Score : 300
        gamemanager.stagePoint += 300;
    }

///////////////////////////////////////////////////////////////////////////////////////////////////

    void OnTriggerEnter2D(Collider2D collision) {
        // Collecting Coin
        if (collision.gameObject.tag == "Coin")
        {
            SoundPlay("ITEM");
            Coin coin = collision.transform.GetComponent<Coin>();
            gamemanager.stagePoint += coin.coinValue;
            collision.gameObject.SetActive(false);
        }
        // Touch Finish Line
        else if (collision.gameObject.tag == "Finish")
        {
            SoundPlay("FINISH");
            gamemanager.NextStage();
        }
    }

///////////////////////////////////////////////////////////////////////////////////////////////////
    
    public void OnDeath()
    {
        // Death Animation
        SoundPlay("DIE");
        spriteRenderer.color = new Color(1,1,1, 0.4f);
        spriteRenderer.flipY = true;
        collid.enabled = false;
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
    }

///////////////////////////////////////////////////////////////////////////////////////////////////

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }

///////////////////////////////////////////////////////////////////////////////////////////////////
    void SoundPlay(string action)
    {
        switch (action)
        {
            case "JUMP":
                audioSource.clip = audioJump;
                break;
            case "ATTACK":
                audioSource.clip = audioAttack;
                break;
            case "DAMAGED":
                audioSource.clip = audioDamaged;
                break;
            case "ITEM":
                audioSource.clip = audioItem;
                break;
            case "DIE":
                audioSource.clip = audioDie;
                break;
            case "FINISH":
                audioSource.clip = audioFinish;
                break;
        }
        audioSource.Play();
    }

///////////////////////////////////////////////////////////////////////////////////////////////////

}