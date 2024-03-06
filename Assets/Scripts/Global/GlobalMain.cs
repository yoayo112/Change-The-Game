/*
Project: Change the Game
File: GlobalMain.cs
Date Created: March 06, 2024
Author(s): Sky Vercauteren
Info:

A global object, that protects itself from being destroyed on load.
Used to:
- initialize gameplay
- store player and session information
- pass information between scenes
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalMain : MonoBehaviour
{
    //we can put global information here that can be both dynamic and static between scenes.
    private GameObject player_;
    public GameObject getPlayer()
    {
        return player_;
    }
    public void setPlayer(GameObject p)
    {
        player_ = p;
        //initialize prefab as player object
        //remove NPC movement script and anything else.
        //add playermovement.cs, free look camera, etc
        
    }
    private List<GameObject> party_;
    public void setParty(List<GameObject> others)
    {
        //set all characters as NPC's
        foreach (GameObject character in others)
        {
            //initialize prefab as NPC in party
            //assign NPC movement script.
            //assign anything else specific to NPCs
        }
        party_ = others;
    }
    public void partyPush(GameObject npc)
    {
        //some logic here to make sure the npc is a valid party member and has the correct components.
        //if components ? do nothing : add npc movement script etc.
        party_.Add(npc);
    }

    // Start is called before the first frame update
    void Start()
    {
        //Make sure that the _Global_ Object is never destroyed.
        DontDestroyOnLoad(gameObject.transform.root);
    }

    //-------------
    //Start Menu
    //-------------
    public void Play()
    {
        //this is purely temporary, there should be a selection by the player.
        // Also, this should Resources.Load so we dont have to have all the prefabs in the scene.
        setPlayer(Resources.Load<GameObject>("COWBOY_PREFAB")); 
        setParty(new List<GameObject>());

        //This should actually probably be a cutscene.
        SceneManager.LoadScene("HomeTown_v2"); //should probably start at family farm at some point but Oh well
    }

    public void Quit()
    {
        Application.Quit();
    }
}
