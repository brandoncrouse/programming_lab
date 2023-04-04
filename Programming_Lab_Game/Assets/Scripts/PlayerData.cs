using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int coins;
    public float[] position;

    public PlayerData(Player_Inventory inv)
    {
        coins = inv.coins;

        position = new float[3];
        position[0] = inv.transform.position.x;
        position[1] = inv.transform.position.y;
    }

}