using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public GameObject[] wanderObject;
    public float walkSpeed, idleTime, idleTimeMin, idleTimeMax;
    public bool randomWaitTime;
    private Vector3 wanderPoint1, wanderPoint2, targetPoint;
    private string state = "walking";
    private float horizontalMovement = 0f;
    private bool startedIdling = false;
    private int movingTo;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    // Start is called before the first frame update
    void Start()
    {
        movingTo = Random.Range(1,3);
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
            if (movingTo == 1 && transform.position.x > targetPoint.x)
            {
                sprite.flipX = true;
                horizontalMovement = -1f;
            }
            if (movingTo == 2 && transform.position.x < targetPoint.x)
            {
                sprite.flipX = false;
                horizontalMovement = 1f;
            }
            checkIdleSwitch();
            
            Debug.Log(horizontalMovement);
            Debug.Log(state);
        }
        if (state.Equals("idle"))
        {
            if (!startedIdling)
            {
                startedIdling = true;
                StartCoroutine(nextTargetRandom());
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
        if (!randomWaitTime)
        {
            yield return new WaitForSeconds(idleTime);
        }
        else
        {
            yield return new WaitForSeconds(Random.Range(1f, idleTime));
        }
        
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

    IEnumerator nextTargetRandom()
    {
        if (!randomWaitTime)
        {
            yield return new WaitForSeconds(idleTime);
        }
        else
        {
            yield return new WaitForSeconds(Random.Range(idleTimeMin, idleTimeMax));
        }

        if (transform.position.x < targetPoint.x)
        {
            targetPoint = new Vector3(Random.Range(transform.position.x,wanderPoint2.x),wanderPoint2.y,wanderPoint2.z);
        }
        else
        {
            targetPoint = new Vector3(Random.Range(wanderPoint1.x,transform.position.x),wanderPoint1.y,wanderPoint1.z);
        }
        if (targetPoint.x > transform.position.x)
        {
            movingTo = 2;
        }
        else
        {
            movingTo = 1;
        }
        state = "walking";
    }

    void checkIdleSwitch()
    {
        if (horizontalMovement == 0f && state.Equals("walking"))
        {
            state = "idle";
            startedIdling = false;
        }
    }
}
