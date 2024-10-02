/*
Project: Change the Game
File: PlayerAction.cs
Date Created: March 16, 2024
Author(s): Sky Vercauteren
Info:

a basic class to define movement for NPC's and enemies.
*/

using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicNPCMovement : MonoBehaviour
{
    //exposed
    public Animator animator;

    //unexposed
    private CharacterController controller_;
    private bool inCombat = false;
    public void Set_Combat(bool b)
    {
        inCombat = b;
        animator.SetBool("Combat", inCombat);
    }

    // Start is called before the first frame update
    void Start()
    {
        controller_ = gameObject.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(inCombat == false)
        {
            //overworld movement
        }
        else
        {
            //for now, there's no movement, but still gravity!
            controller_.Move(new Vector3(0f, -9.8f * Time.deltaTime, 0f));//this is just for gravity when stopped.
        }
    }

}
