using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInvSystem : MonoBehaviour
{
    public string[] inv = new string[2];

    void Start()
    {
        inv = CreateInventoryArray<string>("Starting Item");
    }

    public T[] CreateInventoryArray<T>(T firstItem)
    {
        return new T[2] {firstItem, default(T)};
    }

    public void AddToInventory(string item)
    {
        int i = 0;
        while (inv[i] != null && inv[i] != "")
        {
            if (i < 9)
            {
                i++;
            }
        }

        if (inv[i] == null || inv[i] != "")
        {
            inv[i] = item;
        }
    }
}
