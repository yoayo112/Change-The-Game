/*
Project: Change the Game
File: SceneListener.cs
Date Created: March 13, 2024
Author(s): Sky Vercauteren
Info:

Handles Events that happen in a scene. 
Base class to be extended by each scenes unique listener.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneListener : MonoBehaviour
{
    //unexposed
    protected GameObject player_;
    protected Door[] doors_;
    public Door[] Get_All_Doors()
    {
        if(doors_.Length > 0) { return doors_; }
        else { return null;  }
    }

    private void OnEnable()
    {
        FadeHandler.end += End;
    }
    private void OnDisable()
    {
        FadeHandler.end -= End;
    }


    //End is called by the End event in Fade Handler
    //triggered by the fade out when there is "DelayTime" seconds before the scene ends. usually ~ <2s.
    //use 'protected override void End(){}' in an extended listener.
    protected virtual void End()
    {
        //do stuff on before the scene ends
    }
}
