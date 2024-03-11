using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameEventManager : MonoBehaviour
{
    public delegate void OnStart();
    public static event OnStart onStart;

    public delegate void OnUpdateTimer(int seconds_);
    public static event OnUpdateTimer onUpdateTimer;

    public delegate void OnTimeOver();
    public static event OnTimeOver onTimeOver;



    public static void Start_Minigame()
    {
        onStart?.Invoke();
    }

    public static void Update_Timer(int seconds_)
    {
        onUpdateTimer?.Invoke(seconds_);
    }

    public static void Time_Over()
    {
        onTimeOver?.Invoke();
    }


}
