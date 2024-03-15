/*
Project: Change the Game
File: SceneTransition.cs
Date Created: March 15, 2024
Author(s): Sky Vercauteren
Info:

Base class that handles the transition between one scene and another. 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    //exposed
#if UNITY_EDITOR
    [Help("Use: \n - Drag the portal object inside a doorway or location to trigger the scene. \n - Type the name of the scene you are going to. \n - Type the name of the spawn object *in the destination scene* you want to start from.", UnityEditor.MessageType.Info)]
#endif

    public float delayTime = 1f;
    //transition vars
    [Header("The object that triggers a transition")]
    public GameObject portal;
    [Header("The Scene we want to end up in")]
    public string goTo; //the name of the new scene
    [Header("Transition animation name (animator trigger)")]
    [Header("If you change this, you must also change the animator controller in the child component.")]
    public string transitionAnimation = "fade-out";

    //unexposed
    protected Animator animator_;
    protected GameObject player_;
    protected Transform body_;

    //Invoked by the fade out to broadcast the scene is ending
    public delegate void End();
    public static event End end;

    protected void Invoke_End()
    {
        end?.Invoke();
    }

    // Start is called before the first frame update
    void Start()
    {

        player_ = GlobalService.Get_Player_Instance();
        body_ = player_.GetComponent<Transform>().GetChild(0).GetComponent<Transform>();

        //set fade transition render camera to main camera
        transform.GetChild(0).GetComponent<Canvas>().worldCamera = GlobalService.Get_Camera().GetComponent<Camera>();

        //also we need the fade animator
        animator_ = transform.GetChild(0).GetComponent<Animator>();

        //NO DEFAULT SAPAWN BEHAVIOR: IMPLEMENT THAT IN EXTENDED CLASS 
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

    protected virtual IEnumerator fadeOut(string name)
    {
        Invoke_End();
        animator_.SetTrigger(transitionAnimation);
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadScene(name);
    }

    protected void Spawn_Party(string point, List<GameObject> party)
    {
        Vector3 spawnPosition_ = GameObject.Find(point).GetComponent<Transform>().position;
        player_.GetComponent<Transform>().position = spawnPosition_;
        int xOffset = 1;
        foreach (GameObject member in party)
        {
            member.GetComponent<Transform>().position = new Vector3(spawnPosition_.x + xOffset, spawnPosition_.y, spawnPosition_.z);
            xOffset++;
        }
    }
}

