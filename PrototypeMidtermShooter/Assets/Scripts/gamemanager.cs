using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gamemanager : MonoBehaviour
{
    public static gamemanager instance;
    public GameObject player;
    public playerController playerScript;


    public GameObject PauseMenu;
    public GameObject PlayerDeadMenu;
    public GameObject playerDamageFlash;

    public Image HPBar;

    public bool paused = false;




    void Awake()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<playerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") && !PlayerDeadMenu.activeSelf)
        {
            if (!paused)
            {
                paused = true;
                PauseMenu.SetActive(true);
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
            else
            {
                resume();
            }
        }
    }

    public void resume()
    {
        paused = false;
        PauseMenu.SetActive(false);
        PlayerDeadMenu.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void playerDead()
    {
        paused = true;
        PlayerDeadMenu.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
}


