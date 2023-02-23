using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PauseBlink : MonoBehaviour
{
    public float bpm = 178f;
    private float rate;
    private bool textBlink = true;
    public bool awoken = true;
    public TextMeshProUGUI textDisplay;
    public GameObject parent;

    void Start()
    {
        rate = 60f / bpm;
    }

    void Update()
    {
        if (awoken)
        {
            awoken = false;
            textBlink = true;
            Invoker.InvokeDelayed(blink, rate);
        }
        if (textBlink)
        {
            textDisplay.text = "Paused";
        }
        else
        {
            textDisplay.text = "- Paused -";
        }
        if (!parent.activeSelf)
        {
            textBlink = true;
        }
    }

    void blink()
    {
        if (parent.activeSelf)
        {
            textBlink = !textBlink;
            Invoker.InvokeDelayed(blink, rate);
        }
    }
}
