/*
Project: Change the Game
File: BasicTransition.cs
Date Created: March 04, 2024
Author(s): Sky Vercauteren
Info:

Handles the specific fade transition between one scene and another. 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasicTransition : SceneTransition
{
    //exposed

    [Header("The Spawn Point within that scene to start from (All Scenes Should have 'Main Spawn' as there is one inside this prefab)")]
    public string spawnName; // the name of the spawn within that scene.
    static string newSpawn = "";

    // Start is called before the first frame update
    void Start()
    {

        player_ = GlobalService.Get_Player_Instance();
        body_ = player_.GetComponent<Transform>().GetChild(0).GetComponent<Transform>();

        //set fade transition render camera to main camera
        transform.GetChild(0).GetComponent<Canvas>().worldCamera = GlobalService.Get_Camera().GetComponent<Camera>();

        //also we need the fade animator
        animator_ = transform.GetChild(0).GetComponent<Animator>();

        //move player (and party) to correct spawn location.
        List<GameObject> party = GlobalService.Get_Party_Instances();
        if (newSpawn != "")
        {
            Spawn_Party(newSpawn, party);
        }
        else
        {
            Spawn_Party("MainSpawn", party);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //conditionlogic for scene transitions.
        if (portal.GetComponent<Collider>().bounds.Contains(body_.position))
        {
            StartCoroutine(fadeOut(goTo));
        }
    }

    protected override IEnumerator fadeOut(string name)
    {
        Invoke_End();
        animator_.SetTrigger(transitionAnimation);
        newSpawn = spawnName;
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadScene(name);
    }
}

