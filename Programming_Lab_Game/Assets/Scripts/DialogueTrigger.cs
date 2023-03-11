using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public GameObject dialoguebox, parent;
    public SpriteRenderer spriteRenderer;
    private GameObject player;
    private SpriteRenderer parentSprite;
    public bool turnToPlayer;
    private bool isInCollider;

    void Start()
    {
        player = GameObject.Find("Player");
        parentSprite = parent.GetComponent<SpriteRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }

    void Update()
    {
        if (isInCollider)
        {
            if (Input.GetKeyDown(KeyCode.E) && !dialoguebox.activeSelf)
        {
            dialoguebox.SetActive(true);
            
            if (player != null && turnToPlayer)
            {
                if (player.transform.position.x > transform.position.x)
                {
                    parentSprite.flipX = true;
                }
                else
                {
                    parentSprite.flipX = false;
                }
            }
        }
        }
    }

    void OnTriggerEnter2D()
    {
        isInCollider = true;
    }

    void OnTriggerExit2D()
    {
        isInCollider = false;
        dialoguebox.SetActive(false);
    }
}
