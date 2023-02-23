using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private bool paused = false;
    public AudioSource music, pauseMusic;
    public GameObject pauseCanvas;
    private GameObject targetCanvas;
    private CharacterMovement charMov;

    void Start()
    {
        targetCanvas = (GameObject) Instantiate(pauseCanvas, new Vector3(0, 0, 0), Quaternion.identity);
        targetCanvas.SetActive(false);
        targetCanvas.GetComponent<Canvas>().worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            paused = !paused;
            if (paused)
            {
                Time.timeScale = 0;
                music.Pause();
                pauseMusic.Play();
                targetCanvas.SetActive(true);
                targetCanvas.GetComponent<PauseParent>().blink.awoken = true;
            }
            else
            {
                Time.timeScale = 1;
                pauseMusic.Stop();
                music.UnPause();
                targetCanvas.SetActive(false);
                targetCanvas.GetComponent<PauseParent>().blink.CancelInvoke();
            }
        }
        
    }
}
