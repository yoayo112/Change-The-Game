using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypingGameTimer : MiniGameTimer
{
    protected override void Update_Time(int seconds_)
    {
        TypingGameEvents.Update_Timer(seconds_);
    }

    protected override void Invoke_Start()
    {
        TypingGameEvents.Start_Minigame();
    }
    protected override void Time_Over()
    {
        TypingGameEvents.Time_Over();
    }

    protected override void Subscribe_Events()
    {
        TypingGameEvents.onStart += Start_Minigame;
    }
    protected override void Unsubscribe_Events()
    {
        TypingGameEvents.onStart -= Start_Minigame;
    }
}
