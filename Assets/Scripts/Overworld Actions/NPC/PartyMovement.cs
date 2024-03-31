/*
Project: Change the Game
File: PartyMovement.cs
Date Created: March 06, 2024
Author(s): Sky Vercauteren
Info:

Defines the movement behavior of a given NPC in a players party.
Includes Behavior that would be in PartyAction.cs too, since party members only respond to the player, their actions are limited.
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEditor;
using UnityEngine.UI;


public class PartyMovement : MonoBehaviour
{
    //Exposed:
    [Header("Movement Settings")]
    
    public float followDistance = 5f;
    [Header("Audio Members")]
    public AudioSource audio;
    public AudioClip runSound;
    public AudioClip walkSound;

    //Hidden:
    //player
    private Transform player_;

    //this npc
    private bool inParty_;
    public void set_inParty(bool b)
    {
        inParty_ = b;
    }
    public bool get_inParty()
    {
        return inParty_;
    }
    private Animator animator_;
    private CharacterController controller_;
    public RuntimeAnimatorController movementController;
    public RuntimeAnimatorController combatController;

    //movement
    private float walkSpeed_;
    private float runSpeed_;
    private float fallSpeed_;
    private bool falling_;
    private bool moving_;
    private bool running_;
    private Vector3 direction_;
    private float normalizedSpeed_;
    private float NPCSpeed_;
    public float Get_NPC_Speed() { return NPCSpeed_; }
    private Vector3 moveDir_;

    //combat
    private Canvas combatGUI_;
    private bool inCombat = false;
    public void Set_Combat(bool b)
    {
        inCombat = b;
        if(b == true)
        {
            animator_.runtimeAnimatorController = combatController;
            combatGUI_.renderMode = RenderMode.ScreenSpaceCamera;
            Camera camera = GameObject.Find("Main Camera").GetComponent<Camera>();
            combatGUI_.worldCamera = camera;
            combatGUI_.planeDistance = 2;
        }
        else
        {
            animator_.runtimeAnimatorController = movementController;
        }
    }
    

    // Start is called before the first frame update
    void OnEnable()
    {
        controller_ = gameObject.GetComponent<CharacterController>();
        player_ = GlobalService.Get_Player_Instance().GetComponent<Transform>();
        inParty_ = GlobalService.Get_Main().Is_In_Party(transform.gameObject);
        animator_ = transform.GetChild(0).GetComponent<Animator>();
        running_ = false;
        moving_ = false;
        //copycat run and walk speeds
        runSpeed_ = player_.gameObject.GetComponent<PlayerMovement>().runSpeed -1.5f;
        walkSpeed_ = player_.gameObject.GetComponent<PlayerMovement>().walkSpeed -1f;

        //combat
        combatGUI_ = GlobalService.Find_Canvas_In_Children(gameObject, "CombatGUI");
        combatGUI_.GetComponent<GraphicRaycaster>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(inCombat == false)
        {
            //trigger animations
            moving_ = direction_.magnitude >= 0.1f;
            animator_.SetFloat("Speed", NPCSpeed_);

            //rotate to look at player
            Vector3 target = new Vector3(player_.transform.position.x, controller_.transform.position.y, player_.transform.position.z);
            transform.LookAt(target, Vector3.up);
            moveDir_ = transform.TransformDirection(Vector3.forward);

            //copycat player movement bools
            moving_ = player_.GetComponent<PlayerMovement>().getMoving();
            running_ = player_.GetComponent<PlayerMovement>().getRunning();


            //apply movement
            if (inParty_ && !Is_Close_To_Player(followDistance))
            {
                Update_Movement();
            }
            else
            {
                NPCSpeed_ = 0f;
                //audio.Stop();
                controller_.Move(new Vector3(0f, -9.8f * Time.deltaTime, 0f));//this is just for gravity when stopped.
            }
        }
        else // we still want gravity though!!!
        {
            controller_.Move(new Vector3(0f, -9.8f * Time.deltaTime, 0f));//this is just for gravity when stopped.
        }
        
    }

    private void Update_Movement()
    {
        //apply gravity
        if (controller_.isGrounded)
        {
            fallSpeed_ = 0f;
            falling_ = false;
        }
        else
        {
            fallSpeed_ -= 9.8f * Time.deltaTime;
            falling_ = true;
        }
        moveDir_.y = fallSpeed_;

        if (running_) //Handle running movement
        {
            controller_.Move(moveDir_.normalized * runSpeed_ * Time.deltaTime);
            NPCSpeed_ = runSpeed_;

            //UpdateAudio("Run");

        }
        else //handle walking movement
        {
            NPCSpeed_ = walkSpeed_;
            controller_.Move(moveDir_.normalized * walkSpeed_ * Time.deltaTime);
            //UpdateAudio("Walk");
        }
    }

    public bool Is_Close_To_Player(float dist)
    {
        //find distance
        float xd = Math.Abs(player_.position.x - GetComponent<Transform>().position.x);
        float zd = Math.Abs(player_.position.z - GetComponent<Transform>().position.z);
        if (xd <= dist && zd <= dist)
        {
            return true;
        }
        else return false;
    }
}
