using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class MiniGameTimer : MonoBehaviour
{
    // Start is called before the first frame update
    public float minigameTime = 10; // Time in seconds minigame will last
    private float timeRemaining; // Time left in minigame
    public TMP_Text timeDisplay; //Text box for writing time to screen
    private bool timerRunning; // internal control
    public UnityEvent timeOver; // This event gets invoked when time runs out, used to call methods in other objects.

    void Start()
    {
        timeRemaining = minigameTime;
        timerRunning = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(timerRunning)
        {
            if(timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                if(timeRemaining < 0) {timeRemaining = 0f;}
                int secondsLeft = Mathf.FloorToInt(timeRemaining);
                timeDisplay.text = "Time: " + secondsLeft;
            }
            else
            {
                timerRunning = false;
                timeOver.Invoke();
            }
        }
    }

    //------------------------------------------------------------------------
    //  Event Subscriptions
    //------------------------------------------------------------------------
    public void OnStartClicked()
    {   
        if(!timerRunning)
        {
            timeRemaining = minigameTime;
            timerRunning = true;
        }
    }
}
