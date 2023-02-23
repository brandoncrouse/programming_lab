using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public GameObject[] wanderObject;
    public float walkSpeed, idleTime;
    private Vector3 wanderPoint1, wanderPoint2, targetPoint;
    private string state = "walking";
    private float dirFacing = 1f, horizontalMovement = 0f;
    private bool startedIdling = false;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    // Start is called before the first frame update
    void Start()
    {
        wanderPoint1 = wanderObject[0].transform.position;
        wanderPoint2 = wanderObject[1].transform.position;
        targetPoint = wanderPoint1;
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMovement = 0f;
        if (state.Equals("walking"))
        {
            if (targetPoint == wanderPoint1)
            {
                sprite.flipX = true;
                dirFacing = -1f;
                if (transform.position.x > targetPoint.x)
                {
                    horizontalMovement = -1f;
                }
            }
            if (targetPoint == wanderPoint2)
            {
                sprite.flipX = false;
                dirFacing = 1f;
                if (transform.position.x < targetPoint.x)
                {
                    horizontalMovement = 1f;
                }
            }
            if (horizontalMovement == 0f)
            {
                state = "idle";
                startedIdling = false;
            }
            Debug.Log(horizontalMovement);
        }
        if (state.Equals("idle"))
        {
            if (!startedIdling)
            {
                startedIdling = true;
                StartCoroutine(nextTarget());
            }
        }
    }

    void FixedUpdate()
    {
        rb.AddForce(new Vector3((horizontalMovement * walkSpeed) / 3, 0f, 0f), ForceMode2D.Impulse);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, walkSpeed);
    }

    IEnumerator nextTarget()
    {
        yield return new WaitForSeconds(idleTime);
        if (targetPoint == wanderPoint1)
        {
            targetPoint = wanderPoint2;
        }
        else
        {
            targetPoint = wanderPoint1;
        }
        state = "walking";
    }
}
