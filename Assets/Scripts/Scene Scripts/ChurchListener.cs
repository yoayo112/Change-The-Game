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


public class ChurchListener : SceneListener
{
    //unexposed
    private Transform priestess_;
    bool inParty;

    // Start is called before the first frame update
    void Start()
    {
        player_ = GlobalService.Get_Player_Instance();
        priestess_ = GameObject.Find("PRIESTESS_PREFAB(Original)").GetComponent<Transform>();
        inParty = GameObject.Find("PRIESTESS_PREFAB") != null;
        if (inParty) { Destroy(priestess_.gameObject); }
    }

    // Update is called once per frame
    void Update()
    {

        //Check conditions for unique events that happen in the scene.
        //i.e. special dialog -> even if all other dialog happens elsewhere, I am choosing to put this here because it is what triggers the priestess to join the party.
        if (Input.GetButtonDown("Interact"))
        {
            if (player_.GetComponent<PlayerAction>().isDialogActive()) { player_.GetComponent<PlayerAction>().Dialog(""); }
            if (priestess_.GetComponent<PartyMovement>().Is_Close_To_Player(3) && inParty == false) // three is totally arbitrary lol
            {
                //trigger initial dialog and add priestess to party!
                string[] words = new string[] { "I am in your party now uwu", "Seanster the Meaunster", "Everyone Tell Sam He's Gay, Ok?", "OMG! Sam is the cutest boy in town!", "Hey There Cowboy ;)" };
                player_.GetComponent<PlayerAction>().Dialog(words[Random.Range(0, words.Length -1)]);
                priestess_.gameObject.name = "PRIESTESS_PREFAB";
                GlobalService.Get_Main().Party_Push(priestess_.gameObject);
                inParty = true;
                priestess_.GetComponent<PartyMovement>().set_inParty(true);
                GlobalService.Add_Global_Object(priestess_.gameObject);
            }
        }
    }
}
