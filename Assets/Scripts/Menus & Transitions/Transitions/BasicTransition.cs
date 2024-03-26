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

    [Header("The Spawn Point within that scene to start from (Default if left blank is MainSpawn)")]
    public string spawnName; // the name of the spawn within that scene.
    static string newSpawn = "";


    // Update is called once per frame
    void Update()
    {
        //conditionlogic for scene transitions.
        if (portal.GetComponent<Collider>().bounds.Contains(body_.position))
        {
            StartCoroutine(fadeOut(goTo));
        }
    }

    protected override void Spawn()
    {
        //move player (and party) to correct spawn location.
        if (newSpawn != "")
        {
            Spawn_Party(newSpawn, party_);
        }
        else
        {
            Spawn_Party("MainSpawn", party_);
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

