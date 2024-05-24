using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MiniGameTimer : MonoBehaviour
{
    // Start is called before the first frame update
    public float minigameTime = 10; // Time in seconds minigame will last
    protected float _timeRemaining; // Time left in minigame
    protected bool _timerRunning; // internal control

    private float _countdownTime;
    protected bool _countdownRunning = false;

    private const PlayerCharacterType _myCharacter = PlayerCharacterType.nobody; //Top level timer should target no one in particular.

    //Just for now I am calling this from the combat GUI!!
    public virtual void Start_Countdown()
    {
        _countdownRunning = true;
    }

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
    // In your Inherited class, set _myCharacter to who it needs to
    // be, and whatnot. See GridShooterTimer.cs for an example.
    //----------------------------------------------------------------
    protected virtual void Update_Time(int seconds_)
    {
        MinigameEventManager.Update_Timer(_myCharacter, seconds_);
    }
    protected virtual void Invoke_Start()
    {
        MinigameEventManager.Start_Minigame(_myCharacter);
    }
    protected virtual void Time_Over()
    {
        MinigameEventManager.Time_Over(_myCharacter);
    }

    public virtual void Start_Minigame(PlayerCharacterType whichCharacter_)
    {   
        if( whichCharacter_ == _myCharacter)
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

    //------------------------------------------------------------------------
    //  Event Subscriptions
    //------------------------------------------------------------------------
    protected virtual void Subscribe_Events()
    {
        MinigameEventManager.onStart += Start_Minigame;
    }
    protected virtual void Unsubscribe_Events()
    {
        MinigameEventManager.onStart -= Start_Minigame;
    }
}
