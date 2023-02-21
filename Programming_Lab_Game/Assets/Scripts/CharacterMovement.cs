using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float maxSpeed;
    private float moveSpeed;
    private float horizontalMovement;
    private Rigidbody2D rb;
    private SpriteRenderer srender;

    public float jumpForce, jumpDash;
    private bool justJumped = false, tryJump = false;
    private bool canLand = true;

    public bool onGround = false;
    public Collider2D floorCollider;
    public ContactFilter2D floorFilter;

    public ParticleSystem dust;
    private TrailRenderer trail;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        trail = GetComponent<TrailRenderer>();
        srender = GetComponent<SpriteRenderer>();
        moveSpeed = maxSpeed;
    }

    private void Update()
    {
        horizontalMovement = Input.GetAxis("Horizontal");
        onGround = floorCollider.IsTouching(floorFilter);

        //dont emit trail when not moving - prevents ugly long trails when jumping
        if (rb.velocity.x == 0 && rb.velocity.y == 0)
        {
            trail.emitting = false;
        }
        else
        {
            trail.emitting = true;
        }


        //kick up dust when landing
        if (onGround)
        {
            if (canLand)
            {
                moveSpeed = maxSpeed;
                canLand = false;
                createDust();
            }
        }
        else
        {
            canLand = true;
        }

        //jump
        if (!justJumped && (Input.GetKeyDown(KeyCode.Space) || tryJump))
        {
            if (onGround)
            {
                justJumped = true;
            }
            else
            {
                if (!tryJump)
                {
                    tryJump = true;
                    StartCoroutine(jumpTimer());
                }
            }
        }

        if (horizontalMovement > 0)
        {
            srender.flipX = false;
        }
        if (horizontalMovement < 0)
        {
            srender.flipX = true;
        }
        
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontalMovement * moveSpeed, rb.velocity.y);

        if (justJumped)
        {
            moveSpeed = maxSpeed * jumpDash;
            createDust();
            justJumped = false;
            rb.velocity = new Vector2(rb.velocity.x,jumpForce);
        }
    }

    IEnumerator jumpTimer()
    {
        yield return new WaitForSeconds(0.1f);
        tryJump = false;
    }

    void createDust()
    {
        dust.Play();
    }

    static float roundTo(float value, float multipleOf) {
        return Mathf.Round(value/multipleOf) * multipleOf;
    }
}
