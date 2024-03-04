using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameEventManager : MonoBehaviour
{
    public delegate void OnStart();
    public static event OnStart onStart;



    public static void Start_Minigame()
    {
        onStart?.Invoke();
    }
}
