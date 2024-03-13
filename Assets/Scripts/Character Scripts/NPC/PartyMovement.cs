/*
Project: Change the Game
File: PartyMovement.cs
Date Created: March 06, 2024
Author(s): Sky Vercauteren
Info:

Defines the movement behavior of a given NPC in a players party.
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEditor.Animations;


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
    private float walkSpeed;
    private float runSpeed;
    private static bool inParty_;
    public static void set_inParty(bool b)
    {
        inParty_ = b;
    }
    private Animator animator_;
    private CharacterController controller_;
    private float fallSpeed_;
    private bool falling_;
    private bool moving;
    private bool running;
    private Vector3 direction;
    private float normalizedSpeed;
    private float NPCSpeed;
    public float GetNPCSpeed() { return NPCSpeed; }
    Vector3 moveDir;

    // Start is called before the first frame update
    void Start()
    {
        controller_ = gameObject.GetComponent<CharacterController>();
        player_ = GlobalService.Get_Player_Instance().GetComponent<Transform>();
        inParty_ = GlobalService.Get_Main().Is_In_Party(transform.gameObject);
        animator_ = transform.GetChild(0).GetComponent<Animator>();
        running = false;
        moving = false;
        //copycat run and walk speeds
        runSpeed = player_.gameObject.GetComponent<PlayerMovement>().runSpeed -1f;
        walkSpeed = player_.gameObject.GetComponent<PlayerMovement>().walkSpeed -0.5f;
        updateAnimationConditions(animator_);
    }

    // Update is called once per frame
    void Update()
    {
        //trigger animations
        moving = direction.magnitude >= 0.1f;
        animator_.SetFloat("Speed", NPCSpeed);

        //rotate to look at player
        Vector3 target = new Vector3(player_.transform.position.x, controller_.transform.position.y, player_.transform.position.z);
        transform.LookAt(target, Vector3.up);
        moveDir = transform.TransformDirection(Vector3.forward);

        //copycat player movement bools
        moving = player_.GetComponent<PlayerMovement>().getMoving();
        running = player_.GetComponent<PlayerMovement>().getRunning();


        //apply movement
        if (inParty_ && !Is_Close_To_Player(followDistance))
        {
            Update_Movement();
        }
        else
        {
            NPCSpeed = 0f;
            //audio.Stop();
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
        moveDir.y = fallSpeed_;

        if (running) //Handle running movement
        {
            Debug.Log("is running at "+runSpeed);
            controller_.Move(moveDir.normalized * runSpeed * Time.deltaTime);
            NPCSpeed = runSpeed;

            //UpdateAudio("Run");

        }
        else //handle walking movement
        {
            NPCSpeed = walkSpeed;
            controller_.Move(moveDir.normalized * walkSpeed * Time.deltaTime);
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

    private void updateAnimationConditions(Animator anim)
    {
        //Set run/walk transition conditionals to public player movement speeds
        ChildAnimatorState[] childStates_ = (anim.runtimeAnimatorController as AnimatorController).layers[0].stateMachine.states;
        foreach (ChildAnimatorState s_ in childStates_)
        {
            AnimatorState state_ = s_.state;
            string transition_ = state_.name;
            if (transition_ == "BAKED Walk" || transition_ == "BAKED Run")
            {
                AnimatorStateTransition[] ts_ = state_.transitions;
                foreach (AnimatorStateTransition t_ in ts_)
                {
                    if (transition_ == "BAKED Walk" && t_.conditions[0].parameter == "Speed")
                    {
                        t_.conditions[0].threshold = walkSpeed;
                        state_.speed = state_.speed >= 5 ? 1 + (walkSpeed / 50) : 1 - (walkSpeed / 50); // walking animation scales at +/- ~2% movement speed from base anim speed.
                    }
                    else if (transition_ == "BAKED Run" && t_.conditions[0].parameter == "Speed")
                    {
                        t_.conditions[0].threshold = walkSpeed;
                        state_.speed = state_.speed >= 10 ? 1 + (runSpeed / 80) : 1 - (runSpeed / 80); //running animation scales at +/- ~1.3% movement speed from base anim speed.
                    }
                }
            }
        }
    }
}
