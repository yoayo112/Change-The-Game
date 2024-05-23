using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameEventManager : MonoBehaviour
{
    public delegate void OnStart(PlayerCharacterType whichCharacter);
    public static event OnStart onStart;

    public delegate void OnUpdateTimer(PlayerCharacterType whichCharacter, int seconds_);
    public static event OnUpdateTimer onUpdateTimer;

    public delegate void OnTimeOver(PlayerCharacterType whichCharacter);
    public static event OnTimeOver onTimeOver;



    public static void Start_Minigame(PlayerCharacterType whichCharacter_)
    {
        //Ok so this is being directly invoked by the grid shooter, which is also causing the typing game to start!
        onStart?.Invoke(whichCharacter_);
    }

    public static void Update_Timer(PlayerCharacterType whichCharacter_, int seconds_)
    {
        onUpdateTimer?.Invoke(whichCharacter_, seconds_);
    }

    public static void Time_Over(PlayerCharacterType whichCharacter_)
    {
        onTimeOver?.Invoke(whichCharacter_);
    }


}
