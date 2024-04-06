/*
Project: Change the Game
File: StartMenu.cs
Date Created: March 06, 2024
Author(s): Sky Vercauteren
Info:

Handles Behavior during the initial moments of the game.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    //-------------
    //Start Menu
    //-------------
    private GlobalMain main_;
    private GameObject player_;

    void Awake()
    {
        //this is literally just so the cowboy is interactable in the start menu.      
        main_ = GlobalService.Get_Main();            // = GameObject.Find("_GLOBAL_").GetComponent<Transform>().GetChild(0).GetComponent<GlobalMain>();
        //TODO: Select from list - cowboy is just default.
        StartCoroutine(Select_Player(Resources.Load<GameObject>("COWBOY_PREFAB")));
    }

    public void Play()
    {
        //this is purely temporary, there should be a selection by the player.
        // Also, this should Resources.Load so we dont have to have all the prefabs in the scene.
        main_.Set_Party(new List<GameObject>());

        //This should actually probably be a cutscene.
        SceneManager.LoadScene("Church_inside"); //should probably start at family farm at some point but Oh well
    }

    public void Quit()
    {
        Application.Quit();
    }

    //Initializes a selected character as "Player"
    public IEnumerator Select_Player(GameObject o)
    {
        if (player_) { Destroy(player_); }
        player_ = Instantiate(o);
        GlobalService.Set_Player_Instance(player_);
        yield return new WaitWhile(() => player_.GetComponentInChildren<PlayerAction>().body_ == null);

        

        yield return null;
        //TODO:
        //Add playerMovement.cs
        // -- init all vars
        //remove partyMovement.cs
        //add playerAction
        // -- init all vars
        //tag object as Player

        //TODO: for each other *potential* party member.
        //Add partyMovement.cs
        // -- init all vars
        //remove playerMovement.cs
        //add partyAction (TODO: make partyAction.cs for dialog and stuff).
        //-- init all vars
        //tag object as NPC
    }
}
