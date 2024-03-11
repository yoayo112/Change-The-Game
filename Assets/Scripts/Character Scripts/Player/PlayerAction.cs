/*
Project: Change the Game
File: PlayerAction.cs
Date Created: February 2, 2024
Author(s): Sky Vercauteren
Info:

Used to control the actions that are unique to the cowboy according to player input.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerAction : MonoBehaviour
{
    //internal vars
    private Animator animator_;
    private Door[] doors_;
    private Transform player_;
    private Transform body_;
    private bool inCombat_;
    public void SetCombat(bool t)
    {
       inCombat_ = t;
    }

    //exposed vars
    public  RuntimeAnimatorController movementController;
    


    // Start is called before the first frame update
    void Start()
    {
        Transform global = GameObject.Find("_GLOBAL_").GetComponent<Transform>();
        player_ = global.GetChild(0).GetComponent<GlobalMain>().Get_Player().GetComponent<Transform>();
        animator_ = player_.GetChild(0).GetComponent<Animator>();
        animator_.runtimeAnimatorController = movementController;
        body_ = player_.GetChild(0).GetComponent<Transform>();
        doors_ = FindObjectsOfType<Door>();
        inCombat_ = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Main interact switch
        if (Input.GetButtonDown("Interact"))
        {
            //determine which action to do!
            
            //for scene specific events:
            //condition that determines a scene trigger?
            //call scene event manager function for specific behavior
            //TODO: make a scene event manager for hometown.

            //Door open/close
            Door d_ = GetClosestDoor(doors_);
            if (doors_.Length > 0 && d_ != null)
            {
                d_.SetSwing(true);
            }
        }

        
    }

    //finds which door is closest to the cowboy.
    private Door GetClosestDoor(Door[] all_)
    {
        //according to some folks online, the most resource friendly way to find the real distance between points is by using squares.
        Door closestDoor_ = null;
        float closestDistance_ = Mathf.Infinity;
        Vector3 location_ = body_.position;
        foreach (Door d_ in all_)
        {
            Transform door_ = d_.GetComponent<Transform>();
            Vector3 direction_ = door_.position - location_;
            float dSqrToTarget_ = direction_.sqrMagnitude;
            if (dSqrToTarget_ < closestDistance_)
            {
                closestDistance_ = dSqrToTarget_;
                closestDoor_ = d_;
            }
        }
        return closestDoor_;
    }
}
