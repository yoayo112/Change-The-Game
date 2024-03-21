/*
Project: Change the Game
File: PauseMenu.cs
Date Created: Feb 26, 2024
Author(s): Sky Vercauteren
Info:

A menu accessible by clicking a floating icon in the upper left corner. 
contains:
- music volume
- quit
- resume 

TODO: visual design.
TODO: add player preference buttons/functions.
TODO: add inventory.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private Rect pauseRect = new Rect(10, 10, 24, 28);

    private Rect menuRect = new Rect((Screen.width/6f)*2, Screen.height/6f, (Screen.width / 6)*2, (Screen.height / 6) * 4);
    public bool PAUSED = false;
    private string buttonText = "||";
    private AudioSource music;
    private AudioSource[] allSounds;
    private bool starting = true;
    private IEnumerator First_Moments()
    {
        yield return new WaitForSeconds(8);
        starting = false;
        music.Play();
    }
    private float[] targetVolumes;

    void Start()
    {
        StartCoroutine(First_Moments());
        music = GetComponent<AudioSource>();
        music.ignoreListenerPause = true;
        music.Stop();
        allSounds = FindObjectsOfType<AudioSource>();
        targetVolumes = new float[allSounds.Length];
        for(int i = 0; i < allSounds.Length; i++)
        {
            targetVolumes[i] = allSounds[i].volume;
            allSounds[i].volume = 0;
        }
    }

    void Update()
    {
        if(starting)
        {
            for(int i = 0; i < allSounds.Length; i++)
            {
                allSounds[i].volume = Mathf.Lerp(allSounds[i].volume, targetVolumes[i], 0.01f);
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            togglePause();

            if (Cursor.lockState == CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    void OnGUI()
    {
        pauseRect = GUI.Window(0, pauseRect, doPause, "");
        if(PAUSED)
        {
            menuRect = GUI.Window(0, menuRect, doMenu, "MENU");
        }
    }

    void doMenu(int WindowID)
    {
        //thinking of it as a grid with 6 peices
        float gridx = menuRect.width / 6;
        float gridy = menuRect.height / 4;

        //First is some labels for the controls.
        int b = 17;
        GUI.Label(new Rect(gridx*2, b+10, gridx*3,20),"CONTROLS:" );
        GUI.Label(new Rect(gridx * 2, b*3, gridx * 3, 20),"Move - W,A,S,D" );
        GUI.Label(new Rect(gridx * 2, b*4, gridx * 3, 20), "Camera - Mouse position");
        GUI.Label(new Rect(gridx * 2, b*5, gridx * 3, 20), "Zoom - Mouse Scroll Wheel");
        GUI.Label(new Rect(gridx * 2, b*6, gridx * 3, 20), "Run - Left Shift");
        GUI.Label(new Rect(gridx * 2, b*7, gridx * 3, 20), "Interact - 'E' and Left Mouse Click");
        GUI.Label(new Rect(gridx * 2, b*8, gridx * 3, 20), "Tip Hat - Left Ctrl.");
        GUI.Label(new Rect(gridx * 2, b*9, gridx * 3, 20), "Pause - Esc.");

        //Next is a label for the slider
        GUI.Label(new Rect(gridx*2, gridy*2, gridx*2, 25), "Music Volume");
        //grab current volume and add a slider
        float volume = music.volume;
        volume = GUI.HorizontalSlider(new Rect(gridx*2, gridy*2 + b, gridx*2, 25), volume, 0f, 1.0f);
        music.volume = volume;

        //provide an exit option
        if (GUI.Button(new Rect(gridx*2, gridy * 3, gridx *2, 40), "Exit"))
        {
            Application.Quit();
        }

        //or resume game
        if(GUI.Button(new Rect(gridx*2, gridy *3 + b*3, gridx*2, 25), "Resume"))
        {
            togglePause();

            if(Cursor.lockState == CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        
    }

    void doPause(int windowID)
    {
        if(GUI.Button(new Rect(3, 5, 18, 18), buttonText))
        {
            togglePause();
        }
    }

    public void togglePause()
    {
        if(PAUSED == false)
        {
            Time.timeScale = 0;
            PAUSED = true;
            buttonText = ">";
            AudioListener.pause = true;
        }
        else if(PAUSED == true)
        {
            Time.timeScale = 1;
            PAUSED = false;
            buttonText = "||";
            AudioListener.pause = false;
        }
    }
}
