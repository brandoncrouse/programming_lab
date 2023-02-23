using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private HairAnchor hairAnchor;
    [SerializeField] private GameObject hairObject;
    public float yOffset, spriteRot;
    public float squashFactor, stretchFactor, ssLerpSpeed, camSpeed;
    public GameObject sprite, dashGhostPrefab;
    private GameObject dashGhost;

    //movement
    public float maxSpeed, coyoteTime = 0.125f;
    private float moveSpeed, originalGravity;
    private float horizontalMovement, horizontalDir, verticalDir;
    private bool canCoyote, coyoteJumping;

    //components
    private Rigidbody2D rb;
    private SpriteRenderer srender;
    public GameObject mainCam;

    //camera
    private Vector3 camPos;

    //wallslide
    private bool isWallSliding, canWallSlide, didWallSlide;
    public float wallSlidingSpeed, wallSlider;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;

    //jumping and landing
    public float jumpForce, jumpDash;
    private bool justJumped = false, tryJump = false;
    private bool canLand = true;
    private bool willFlip = false;

    //dash
    private bool canDash = true, isDashing;
    public float dashingPower = 24f, dashingTime = 0.2f, dashingCooldown = 1f;
    [SerializeField] private TrailRenderer tr;

    //audio
    public AudioSource sfxSource, stepSource;
    public AudioClip jumpSound, dashSound, zapSound;

    //collisions
    public bool onGround = false;
    public Collider2D floorCollider;
    public ContactFilter2D floorFilter;
    private float dirFacing = 1f;

    //particles
    public ParticleSystem jumpDust, landDust, flipDust, dashDust;

    //shaders for flash
    private Shader shaderGUItext, shaderSpritesDefault;

    private void Start()
    {

        //init shader for white flash
        shaderGUItext = Shader.Find("GUI/Text Shader");
        shaderSpritesDefault = Shader.Find("Sprites/Default");

        //init components
        rb = GetComponent<Rigidbody2D>();
        srender = sprite.GetComponent<SpriteRenderer>();
        moveSpeed = maxSpeed;
        mainCam = GameObject.Find("Main Camera");
        originalGravity = rb.gravityScale;

        //init camera pos
        camPos = mainCam.transform.position;
    }

    private void Update()
    {
        //update wallcheck position
        wallCheck.transform.position = new Vector3(transform.position.x + (dirFacing * 0.3125f), transform.position.y + 0.1875f, 0f);

        //camera movement
        camPos = Vector3.Lerp(camPos, new Vector3(transform.position.x,0f,-3f), Time.deltaTime * camSpeed);
        mainCam.transform.position = camPos;
        hairSpriteOffsets();

        //do not run following code if dashing
        if (isDashing)
        {
            return;
        }

        horizontalMovement = Input.GetAxis("Horizontal");
        horizontalDir = Input.GetAxisRaw("Horizontal");
        verticalDir = Input.GetAxisRaw("Vertical");

        sprite.transform.rotation = Quaternion.Euler(0.0f,0.0f,horizontalMovement * -spriteRot);

        onGround = floorCollider.IsTouching(floorFilter);

        bool wasWallSliding = isWallSliding;
        wallSlide();
        if (wasWallSliding && !isWallSliding)
        {
            wasWallSliding = false;
            StartCoroutine(coyoteJump(0.25f));
        }

        //kick up dust when landing
        if (onGround)
        {
            didWallSlide = false;
            canWallSlide = true;
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
            canCoyote = true;
        }
        else
        {
            if (canCoyote)
            {
                canCoyote = false;
                StartCoroutine(coyoteJump());
            }
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

        //coyote jump
        if (Input.GetKeyDown(KeyCode.Space) && coyoteJumping && rb.velocity.y <= 0f)
        {
            justJumped = true;
            sfxSource.PlayOneShot(jumpSound);
            if (didWallSlide)
            {
                canWallSlide = false;
            }
        }
        
        //dash
        if (canDash)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                StartCoroutine(Dash());
            }
        }
    }

    private void FixedUpdate()
    {
        //do none of this if dashing
        if (isDashing)
        {
            return;
        }

        //set velocity based on movement code
        rb.velocity = new Vector2(horizontalMovement * moveSpeed, rb.velocity.y);

        //lerp sprite squash n stretch
        sprite.transform.localScale = Vector3.Lerp(sprite.transform.localScale, new Vector3(1f, 1f, 1f), Time.deltaTime * ssLerpSpeed);

        //when jump
        if (justJumped)
        {
            sprite.transform.localScale = new Vector3(0.8f * stretchFactor,1.3f * stretchFactor,1f);
            moveSpeed = maxSpeed * jumpDash;
            jumpDust.Play();
            landDust.Play();
            justJumped = false;
            rb.velocity = new Vector2(rb.velocity.x,jumpForce);
        }

        rb.velocity = new Vector2(rb.velocity.x,Mathf.Max(rb.velocity.y,-14f));
        
        willFlip = srender.flipX;

        //update sprite and hair position when turning
        if (horizontalMovement > 0)
        {
            srender.flipX = false;
            hairAnchor.transform.position = new Vector3(transform.position.x-0.16f,transform.position.y+0.5f,transform.position.z+0f);
            dirFacing = 1f;
        }
        if (horizontalMovement < 0)
        {
            srender.flipX = true;
            hairAnchor.transform.position = new Vector3(transform.position.x+0.16f,transform.position.y+0.5f,transform.position.z+0f);
            dirFacing = -1f;
        }

        //create dust if player flipped
        if (srender.flipX != willFlip && onGround)
        {
            flipDust.Play();
        }
        
        
    }

    private IEnumerator jumpTimer()
    {
        yield return new WaitForSeconds(0.1f);
        tryJump = false;
    }

    //thanks to Bendux on youtube for part of the dash function
    private IEnumerator Dash()
    {
        coyoteJumping = false;
        sfxSource.PlayOneShot(dashSound);
        sfxSource.PlayOneShot(zapSound);
        canDash = false;
        isDashing = true;
        rb.gravityScale = 0;
        //rb.velocity = new Vector2(dirFacing * dashingPower, 0f);
        if (horizontalDir == 0f && verticalDir == 0f)
        {
            horizontalDir = dirFacing;
        }
        rb.velocity = new Vector2(horizontalDir, verticalDir).normalized * new Vector2(dirFacing * dashingPower, 0f).magnitude;
        dashDust.Play();
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        rb.velocity = rb.velocity.normalized * 3f;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
        StartCoroutine(blinkSprite());
    }

    private IEnumerator coyoteJump(float _cTime = 0.125f)
    {
        coyoteJumping = true;
        yield return new WaitForSeconds(_cTime);
        coyoteJumping = false;
    }

    private IEnumerator blinkSprite()
    {
        srender.material.shader = shaderGUItext;
        srender.material.color = Color.white;
        yield return new WaitForSeconds(0.07f);
        srender.material.shader = shaderSpritesDefault;
        srender.material.color = Color.white;
    }

    private void hairSpriteOffsets()
    {
        //set hair part offsets
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

    private void wallSlide()
    {
        if (isWalled() && !onGround && Input.GetAxisRaw("Horizontal") != 0f && canWallSlide)
        {
            isWallSliding = true;
            if (!didWallSlide)
            {
                wallSlidingSpeed = 0f;
                wallSlider = 0f;
            }
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
            wallSlidingSpeed = Mathf.Clamp(wallSlidingSpeed += wallSlider * Time.deltaTime, 0f, 14f);
            wallSlider += 7f * Time.deltaTime;
            Debug.Log(wallSlidingSpeed);
            coyoteJumping = true;
            didWallSlide = true;
        }
        else
        {
            isWallSliding = false;
        }
    }

    private bool isWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position,0.2f,wallLayer);
    }

    private Vector2 Abs(Vector2 _vec) {
        return new Vector2(Mathf.Abs(_vec.x), Mathf.Abs(_vec.y));
    }
}
