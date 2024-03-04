using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class MiniGameTimer : MonoBehaviour
{
    // Start is called before the first frame update
    public float minigameTime = 10; // Time in seconds minigame will last
    private float _timeRemaining; // Time left in minigame
    public TMP_Text timeDisplay; //Text box for writing time to screen
    private bool _timerRunning; // internal control
    public UnityEvent timeOver; // This event gets invoked when time runs out, used to call methods in other objects.

    void OnEnable()
    {
        MinigameEventManager.onStart += Start_Minigame;
    }
    void OnDisable()
    {
        MinigameEventManager.onStart -= Start_Minigame;
    }
    void Start()
    {
        _timeRemaining = minigameTime;
        _timerRunning = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(_timerRunning)
        {
            if(_timeRemaining > 0)
            {
                _timeRemaining -= Time.deltaTime;
                if(_timeRemaining < 0) {_timeRemaining = 0f;}
                int secondsLeft = Mathf.FloorToInt(_timeRemaining);
                timeDisplay.text = "Time: " + secondsLeft;
            }
            else
            {
                _timerRunning = false;
                timeOver.Invoke();
            }
        }
    }

    //------------------------------------------------------------------------
    //  Event Subscriptions
    //------------------------------------------------------------------------
    public void Start_Minigame()
    {   
        if(!_timerRunning)
        {
            _timeRemaining = minigameTime;
            _timerRunning = true;
        }
    }
}
