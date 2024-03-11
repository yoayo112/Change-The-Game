using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MiniGameTimer : MonoBehaviour
{
    // Start is called before the first frame update
    public float minigameTime = 10; // Time in seconds minigame will last
    private float _timeRemaining; // Time left in minigame
    private bool _timerRunning; // internal control

    private float _countdownTime;
    private bool _countdownRunning;

    void OnEnable()
    {
        Subscribe_Events();
    }
    void OnDisable()
    {
        Unsubscribe_Events();
    }
    void Start()
    {
        _timeRemaining = minigameTime;
        _timerRunning = false;

        _countdownTime = 4f;
        _countdownRunning = true;
        Update_Time(3);
    }

    // Update is called once per frame
    public void Update()
    {
        if(_countdownRunning)
        {
            _countdownTime -= Time.deltaTime;
            if(_countdownTime < 0)
            {
                _countdownTime = 0f;
                _countdownRunning = false;
            }
            int seconds_ = Mathf.FloorToInt(_countdownTime);
            Update_Time(seconds_);

            if(!_countdownRunning)
            {
                Invoke_Start();
            }
        }

        if(_timerRunning)
        {
            if(_timeRemaining > 0)
            {
                _timeRemaining -= Time.deltaTime;
                if(_timeRemaining < 0)
                {
                    _timeRemaining = 0f;
                }
                int seconds_ = Mathf.FloorToInt(_timeRemaining);
                Update_Time(seconds_);
            }
            else
            {
                _timerRunning = false;
                Time_Over();
                Update_Time(0);
            }
        }
    }

    //----------------------------------------------------------------
    //  OVERRIDE THESE GUYS IN INHERITED CLASSES
    //----------------------------------------------------------------
    // Change MinigameEventManager to the name of whatever 
    // Event manager you created out of minigameeventmanager
    //----------------------------------------------------------------
    protected virtual void Update_Time(int seconds_)
    {
        MinigameEventManager.Update_Timer(seconds_);
    }
    protected virtual void Invoke_Start()
    {
        MinigameEventManager.Start_Minigame();
    }
    protected virtual void Time_Over()
    {
        MinigameEventManager.Time_Over();
    }

    protected virtual void Subscribe_Events()
    {
        MinigameEventManager.onStart += Start_Minigame;
    }
    protected virtual void Unsubscribe_Events()
    {
        MinigameEventManager.onStart -= Start_Minigame;
    }

    //------------------------------------------------------------------------
    //  Event Subscriptions
    //------------------------------------------------------------------------
    public void Start_Minigame()
    {   
        int time_ = (int)minigameTime;
        Update_Time(time_);
        if(!_timerRunning)
        {
            _timeRemaining = minigameTime;
            _timerRunning = true;
        }
    }
}
