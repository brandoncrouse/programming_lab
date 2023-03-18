using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_HPManager : MonoBehaviour
{
    private int health, maxHealth = 10;

    public int Health
    {
        get
        {
            return health;
        }

        set
        {
            health = Mathf.Clamp(health + value, 0, maxHealth);
        }
    }

    public string DisplayHealth
    {
        get
        {
            return (health * 10).ToString() + "%";
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
