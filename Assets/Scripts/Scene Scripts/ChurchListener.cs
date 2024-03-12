/*
Project: Change the Game
File: ChurchListener.cs
Date Created: March 10, 2024
Author(s): Sky Vercauteren
Info:

A Script that listens for unique events within the scene.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class ChurchListener : MonoBehaviour
{
    //unexposed
    private GameObject player_;
    private Transform body_;
    private Transform priestess_;
    bool inParty;

    // Start is called before the first frame update
    void Start()
    {
        player_ = GlobalService.Get_Player_Instance();
        body_ = player_.GetComponent<Transform>();
        priestess_ = GameObject.Find("PRIESTESS_PREFAB").GetComponent<Transform>();
        inParty = false;
    }

    // Update is called once per frame
    void Update()
    {

        //Check conditions for unique events that happen in the scene.
        //i.e. special dialog -> even if all other dialog happens elsewhere, I am choosing to put this here because it is what triggers the priestess to join the party.
        if (inParty == false && Input.GetButtonDown("Interact"))
        {
            
            //find distance
            float xd = Math.Abs(body_.position.x - priestess_.position.x);
            float zd = Math.Abs(body_.position.z - priestess_.position.z);
            if (xd <= 3 && zd <= 3) // three is totally arbitrary lol
            {
                //trigger initial dialog and add priestess to party!
                string[] words = new string[] { "I am in your party now uwu", "Seanster the Meaunster", "Everyone Tell Sam He's Gay, Ok?", "OMG! Sam is the cutest boy in town!", "Hey There Cowboy ;)" };
                player_.GetComponent<PlayerAction>().Dialog(words[Random.Range(0, words.Length -1)]);
                GlobalService.Get_Main().Party_Push(priestess_.gameObject);
                PartyMovement.set_inParty(true);
            }
        }
    }
        
}