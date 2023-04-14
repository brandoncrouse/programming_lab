using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public Collectable coin;
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        coin = new Collectable();
        coin.coinValue = 1;
        coin.rarity = (int) Collectable.eRar.Common;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D()
    {
        player.GetComponent<Player_Inventory>().coins += coin.coinValue;
        player.GetComponent<PlayerInvSystem>().AddToInventory("Coin");
        Debug.Log(coin.rarity);
        Destroy(gameObject);
    }
}
