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
            PlayerAction playerAction = player_.GetComponent<PlayerAction>();
            if (playerAction.isDialogActive()) { playerAction.Dialog(); }
            if (priestess_.GetComponent<PartyMovement>().Is_Close_To_Player(3) && inParty == false) // three is totally arbitrary lol
            {
                //trigger initial dialog and add priestess to party!
                string[] words = new string[] {"Oh! you don't seem locked like everyone else. ",
                    "You're the first person I've encountered who doesn't just keep repeating the same inane phrases. ",
                    "..I've been pouring over this book for an answer to my sudden conciousness, but I haven't found anything. ",
                    "Oh well. If the gods wanted me to understand they would have layed it down differently. ",
                    "Seeing as there isn't much we can learn in here, why don't we go check out the town and give it a second look." };
                playerAction.Set_dialogWords(words);
                playerAction.Set_dialogPage(0);
                playerAction.Dialog();
                GlobalService.Get_Main().Party_Push(priestess_.gameObject, "PRIESTESS_PREFAB");
                inParty = true;
            }
        }
    }
}
