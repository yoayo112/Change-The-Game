/*
Project: Change the Game
File: FadeHandler.cs
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

    [Header("The Name of THIS Scene")]
    public string scene;
    [Header("The object that triggers a transition")]
    public GameObject portal;
    [Header("The Scene we want to end up in")]
    public string goTo; //the name of the new scene
    [Header("The Spawn Point within that scene to start from (All Scenes Should have 'Main Spawn' as there is one inside this prefab)")]
    public string spawnName; // the name of the spawn within that scene.

    //unexposed
    private Animator animator_;
    private GameObject player_;
    private Transform body_;
    static string newSpawn = "";

    //Invoked by the fade out to broadcast the scene is ending
    public delegate void End();
    public static event End end;

    // Start is called before the first frame update
    void Start()
    {
        //Access the _GLOBAL_ undestroyed object to spawn a player at "MainSpawn"
        //Transform global = GameObject.Find("_GLOBAL_").GetComponent<Transform>();
        //player_ = Instantiate(global.GetChild(0).GetComponent<GlobalMain>().GetPlayer());
        //Transform global = GlobalService.Get_Global();

        player_ = GlobalService.Get_Player_Instance();
        body_ = player_.GetComponent<Transform>().GetChild(0).GetComponent<Transform>();

        //set fade transition render camera to main camera
        //transform.GetChild(0).GetComponent<Canvas>().worldCamera = global.GetChild(1).GetComponent<Camera>();
        transform.GetChild(0).GetComponent<Canvas>().worldCamera = GlobalService.Get_Camera().GetComponent<Camera>();

        //also we need the fade animator
        animator_ = transform.GetChild(0).GetComponent<Animator>();

        //move player (and party) to correct spawn location.
        List<GameObject> party = GlobalService.Get_Real_Party();
        if (newSpawn != "")
        {
            Vector3 spawnPosition_ = GameObject.Find(newSpawn).GetComponent<Transform>().position;
            player_.GetComponent<Transform>().position = spawnPosition_;
            int xOffset = 1;
            foreach(GameObject member in party)
            {
                member.GetComponent<Transform>().position = new Vector3(spawnPosition_.x + xOffset, spawnPosition_.y, spawnPosition_.z);
            }
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
        end?.Invoke();
        animator_.SetTrigger("fade-out");
        newSpawn = spawnName;
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadScene(name);
    }
}

