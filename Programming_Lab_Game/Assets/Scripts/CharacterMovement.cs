using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private HairAnchor hairAnchor;
    [SerializeField] private GameObject hairObject;
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

    private bool canDash = true, isDashing;
    public float dashingPower = 24f, dashingTime = 0.2f, dashingCooldown = 1f;
    [SerializeField] private TrailRenderer tr;

    public AudioSource sfxSource, stepSource;
    public AudioClip jumpSound, dashSound;

    public bool onGround = false;
    public Collider2D floorCollider;
    public ContactFilter2D floorFilter;
    private float dirFacing = 1f;

    public ParticleSystem jumpDust, landDust, flipDust;
    private TrailRenderer trail;

    private Shader shaderGUItext, shaderSpritesDefault;

    private void Start()
    {
        //init shader for white flash
        shaderGUItext = Shader.Find("GUI/Text Shader");
        shaderSpritesDefault = Shader.Find("Sprites/Default");

        //init components
        rb = GetComponent<Rigidbody2D>();
        trail = GetComponent<TrailRenderer>();
        srender = sprite.GetComponent<SpriteRenderer>();
        moveSpeed = maxSpeed;
    }

    private void Update()
    {

        if (isDashing)
        {
            return;
        }

        horizontalMovement = Input.GetAxis("Horizontal");

        sprite.transform.rotation = Quaternion.Euler(0.0f,0.0f,horizontalMovement * -spriteRot);

        onGround = floorCollider.IsTouching(floorFilter);


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
            if (rb.velocity.x != 0)
            {
                if (!stepSource.isPlaying)
                {
                    stepSource.Play();
                }
            }
            else
            {
                stepSource.Stop();
            }
        }
        else
        {
            stepSource.Stop();
            canLand = true;
        }

        //jump
        if (!justJumped && (Input.GetKeyDown(KeyCode.Space) || tryJump))
        {
            if (onGround)
            {
                sfxSource.PlayOneShot(jumpSound);
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
        
        //dash
        if (canDash && Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }

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
            dirFacing = 1f;
        }
        if (horizontalMovement < 0)
        {
            srender.flipX = true;
            hairAnchor.transform.position = new Vector3(transform.position.x+0.1f,transform.position.y+0.12f,transform.position.z+0f);
            dirFacing = -1f;
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

    private IEnumerator jumpTimer()
    {
        yield return new WaitForSeconds(0.1f);
        tryJump = false;
    }

    //thanks to Bendux on youtube for the dash script
    private IEnumerator Dash()
    {
        sfxSource.PlayOneShot(dashSound);
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        rb.velocity = new Vector2(dirFacing * dashingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
        StartCoroutine(blinkSprite());
    }

    private IEnumerator blinkSprite()
    {
        srender.material.shader = shaderGUItext;
        srender.material.color = Color.white;
        yield return new WaitForSeconds(0.07f);
        srender.material.shader = shaderSpritesDefault;
        srender.material.color = Color.white;
    }
}
