using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinTracker : MonoBehaviour
{
    public TextMeshProUGUI tex, tex2;
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        tex.text = "Coins: " + player.GetComponent<Player_Inventory>().coins.ToString();
        tex2.text = "INVENTORY<br>" + player.GetComponent<PlayerInvSystem>().inv[0] + "<br>" + player.GetComponent<PlayerInvSystem>().inv[1];
    }
}
