/*
Project: Change the Game
File: HometownListener.cs
Date Created: March 12, 2024
Author(s): Sky Vercauteren
Info:

A Script that listens for unique events within the scene.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HometownListener : MonoBehaviour
{
    //unexposed
    private GameObject player_;
    private Transform body_;
    private Door[] doors_;
    public Door[] Get_All_Doors()
    {
        return doors_;
    }

    // Start is called before the first frame update
    void Start()
    {
        player_ = GlobalService.Get_Player_Instance();
        body_ = player_.GetComponent<Transform>();

        doors_ = FindObjectsOfType<Door>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
