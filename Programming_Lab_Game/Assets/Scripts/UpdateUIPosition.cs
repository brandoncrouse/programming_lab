using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateUIPosition : MonoBehaviour
{
    private GameObject mainCam;
    // Start is called before the first frame update
    void Start()
    {
        mainCam = GameObject.Find("Main Camera");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = mainCam.transform.position;
    }
}
