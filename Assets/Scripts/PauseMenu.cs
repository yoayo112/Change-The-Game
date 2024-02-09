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

    void Start()
    {
        music = GetComponent<AudioSource>();
        music.ignoreListenerPause = true;
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

        //first is a label for the slider
        GUI.Label(new Rect(gridx*2, gridy, gridx*4, gridy), "Music Volume");
        //grab current volume and add a slider
        float volume = music.volume;
        volume = GUI.HorizontalSlider(new Rect(gridx*2, gridy+30, gridx*2, 25), volume, 0f, 1.0f);
        music.volume = volume;

        //provide an exit option
        if (GUI.Button(new Rect(gridx*2, gridy * 2, gridx *2, 40), "Exit"))
        {
            Application.Quit();
        }

        //or resume game
        if(GUI.Button(new Rect(gridx*2, gridy *3, gridx*2, 25), "Resume"))
        {
            togglePause();
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
