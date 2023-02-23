using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseParent : MonoBehaviour
{
    public PauseBlink blink;
    // Start is called before the first frame update
    void Start()
    {
        blink.parent = gameObject;
    }
}
