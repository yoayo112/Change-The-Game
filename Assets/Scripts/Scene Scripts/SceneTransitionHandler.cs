/*
Project: Change the Game
File: SceneTransitionHandler.cs
Date Created: March 04, 2024
Author(s): Sky Vercauteren
Info:

Handles the transition between one scene and another. 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeHandler : MonoBehaviour
{
    //exposed
    public float delayTime = 1f;
    [Header("Geographic Objects")]
    [Help("Use: \n - Drag the portal object inside a doorway or location to trigger the scene. \n - Type the name of the scene you are going to. \n - Type the name of the spawn object *in the destination scene* you want to start from.", UnityEditor.MessageType.Info)]
    public GameObject portal;
    public string goTo; //the name of the new scene
    public string spawnName; // the name of the spawn within that scene.

    //unexposed
    private Animator animator_;
    private GameObject player_;
    private Transform body_;
    static string newSpawn = "";
    // Start is called before the first frame update
    void Start()
    {
        //Access the _GLOBAL_ undestroyed object to spawn a player at "MainSpawn"
        Transform global = GameObject.Find("_GLOBAL_").GetComponent<Transform>();
        player_ = Instantiate(global.GetChild(0).GetComponent<GlobalMain>().getPlayer());
        body_ = player_.GetComponent<Transform>().GetChild(0).GetComponent<Transform>();

        //set fade transition render camera to main camera
        transform.GetChild(0).GetComponent<Canvas>().worldCamera = global.GetChild(1).GetComponent<Camera>();

        //also we need the fade animator
        animator_ = transform.GetChild(0).GetComponent<Animator>();
        
        //move player to correct spawn location.
        if (newSpawn != "")
        {
            player_.GetComponent<Transform>().position = GameObject.Find(newSpawn).GetComponent<Transform>().position;
        }
        else
        {
            player_.GetComponent<Transform>().position = GameObject.Find("MainSpawn").GetComponent<Transform>().position;
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

    IEnumerator fadeOut(string name)
    {
        animator_.SetTrigger("fade-out");
        newSpawn = spawnName;
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadScene(name);
    }
}

