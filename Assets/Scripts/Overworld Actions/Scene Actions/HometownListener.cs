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

public class HometownListener : SceneListener
{
    [Header("Destroy these conditionally")]
    public GameObject fish;
    public GameObject squid;
    public GameObject squidPortal;
    public GameObject squidZoom;


    // Start is called before the first frame update
    void Start()
    {
        //this is the trigger when you come back out of the church with the priestess
        if(GlobalService.Get_Party().Count > 0) 
        { 
            Destroy(fish); 
            //trigger cutscene (zoom on squid)
        }
        else { Destroy(squid); Destroy(squidPortal); Destroy(squidZoom); }
        player_ = GlobalService.Get_Player_Instance();
        doors_ = FindObjectsOfType<Door>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void End()
    {
        
    }
}
