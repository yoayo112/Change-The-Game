using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridShooterTimer : MiniGameTimer
{
    private const PlayerCharacterType _myCharacter = PlayerCharacterType.cowboy;
    protected override void Update_Time(int seconds_)
    {
        MinigameEventManager.Update_Timer(_myCharacter, seconds_);
    }

    protected override void Invoke_Start()
    {
        Debug.Log("Specific Grid Shooter start is invoked");
        MinigameEventManager.Start_Minigame(_myCharacter);
    }
    protected override void Time_Over()
    {
        MinigameEventManager.Time_Over(_myCharacter);
    }

    public override void Start_Minigame(PlayerCharacterType whichCharacter_)
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
    
}
