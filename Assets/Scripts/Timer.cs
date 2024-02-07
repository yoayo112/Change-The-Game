using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer 
{
    private Action action;
    private float time;
    private bool isDestroyed;

    public Timer(Action a, float t)
    {
        action = a;
        time = t;
        isDestroyed = false;
    }

    public void Update()
    {
        if(!isDestroyed){
            time -= Time.deltaTime;
            if (time < 0)
            {
                action();
                Destroy();
            }

        }
    }

    public void Destroy()
    {
        isDestroyed = true;
    }
}
