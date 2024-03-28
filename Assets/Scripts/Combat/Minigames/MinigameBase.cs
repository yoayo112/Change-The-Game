/*
Project: Change the Game
File: MinigameBaseg.cs
Date Created: March 26, 2024
Author(s): Sky Vercauteren
Info:

Base Class that all minigames should extend:
-allows scripts to handle minigames by type.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameBase : MonoBehaviour
{
    //------------------------------------------------------------------------------------
    //Event Variables
    //------------------------------------------------------------------------------------
    protected bool _isRunning = false;
    public bool Get_isRunning() => _isRunning;

    //------------------------------------------------------------------------------------
    //Combat Variables
    //------------------------------------------------------------------------------------
    protected float _effectiveness;
    public float Get_effectiveness() => _effectiveness;
    public GameObject targeting_GUI;

    //-------------------------------------------------------------------------------------
    //  Audio handling
    //-------------------------------------------------------------------------------------
    [Header("Audio Members and Settings")]
    public AudioSource audioSource;
    public AudioClip[] audioClipArray;
    public float volume = 0.5f;


    //TODO: Refactor both the typing game and the grid shooter to inherit common stuff
}
