using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridShooterTimer : MiniGameTimer
{
    protected override void Update_Time(int seconds_)
    {
        GridShooterEventManager.Update_Timer(seconds_);
    }

    protected override void Invoke_Start()
    {
        Debug.Log("Specific Grid Shooter start is invoked");
        GridShooterEventManager.Start_Minigame();
    }
    protected override void Time_Over()
    {
        GridShooterEventManager.Time_Over();
    }

    protected override void Subscribe_Events()
    {
        GridShooterEventManager.onStart += Start_Minigame;
    }
    protected override void Unsubscribe_Events()
    {
        GridShooterEventManager.onStart -= Start_Minigame;
    }
}
