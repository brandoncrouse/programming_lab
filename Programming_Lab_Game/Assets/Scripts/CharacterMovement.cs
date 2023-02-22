using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private HairAnchor hairAnchor;
    public float yOffset, spriteRot;
    public float squashFactor, stretchFactor, ssLerpSpeed;
    public GameObject sprite;

    public float maxSpeed;
    private float moveSpeed;
    private float horizontalMovement;
    private Rigidbody2D rb;
    private SpriteRenderer srender;

    public float jumpForce, jumpDash;
    private bool justJumped = false, tryJump = false;
    private bool canLand = true;
    private bool willFlip = false;

    public bool onGround = false;
    public Collider2D floorCollider;
    public ContactFilter2D floorFilter;

    public ParticleSystem jumpDust, landDust, flipDust;
    private TrailRenderer trail;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        trail = GetComponent<TrailRenderer>();
        srender = sprite.GetComponent<SpriteRenderer>();
        moveSpeed = maxSpeed;
    }

    private void Update()
    {
        horizontalMovement = Input.GetAxis("Horizontal");

        sprite.transform.rotation = Quaternion.Euler(0.0f,0.0f,horizontalMovement * -spriteRot);

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
                sprite.transform.localScale = new Vector3(1.3f * squashFactor,0.8f * squashFactor,1f);
                moveSpeed = maxSpeed;
                canLand = false;
                landDust.Play();
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

        
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontalMovement * moveSpeed, rb.velocity.y);

        //lerp sprite squash n stretch
        sprite.transform.localScale = Vector3.Lerp(sprite.transform.localScale, new Vector3(1f, 1f, 1f), Time.deltaTime * ssLerpSpeed);

        if (justJumped)
        {
            sprite.transform.localScale = new Vector3(0.8f * stretchFactor,1.3f * stretchFactor,1f);
            moveSpeed = maxSpeed * jumpDash;
            jumpDust.Play();
            justJumped = false;
            rb.velocity = new Vector2(rb.velocity.x,jumpForce);
        }

        rb.velocity = new Vector2(rb.velocity.x,Mathf.Max(rb.velocity.y,-7f));
        
        willFlip = srender.flipX;

        if (horizontalMovement > 0)
        {
            srender.flipX = false;
            hairAnchor.transform.position = new Vector3(transform.position.x-0.1f,transform.position.y+0.12f,transform.position.z+0f);
            hairAnchor.hairSprites[0].flipX = false;
            hairAnchor.hairSprites[1].flipX = false;
            hairAnchor.hairSprites[2].flipX = false;
            hairAnchor.hairSprites[3].flipX = false;
        }
        if (horizontalMovement < 0)
        {
            srender.flipX = true;
            hairAnchor.transform.position = new Vector3(transform.position.x+0.1f,transform.position.y+0.12f,transform.position.z+0f);
            hairAnchor.hairSprites[0].flipX = true;
            hairAnchor.hairSprites[1].flipX = true;
            hairAnchor.hairSprites[2].flipX = true;
            hairAnchor.hairSprites[3].flipX = true;
        }

        if (srender.flipX != willFlip && onGround)
        {
            flipDust.Play();
        }

        Vector2 currentOffset = Vector2.zero;
        currentOffset.x = -rb.velocity.x / 120;
        if (rb.velocity.y > 0)
        {
            currentOffset.y = yOffset + (-rb.velocity.y / 240);
        }
        else
        {
            currentOffset.y = yOffset + (-rb.velocity.y / 80);
        }
        currentOffset.y /= 1f - (-Mathf.Abs(rb.velocity.x) / 12);

        hairAnchor.partOffset = currentOffset;
    }

    IEnumerator jumpTimer()
    {
        yield return new WaitForSeconds(0.1f);
        tryJump = false;
    }

    static float roundTo(float value, float multipleOf) {
        return Mathf.Round(value/multipleOf) * multipleOf;
    }
}
