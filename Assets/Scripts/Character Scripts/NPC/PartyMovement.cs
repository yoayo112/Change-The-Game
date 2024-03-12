/*
Project: Change the Game
File: PartyMovement.cs
Date Created: March 06, 2024
Author(s): Sky Vercauteren
Info:

Defines the movement behavior of a given NPC in a players party.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class PartyMovement : MonoBehaviour
{
    //Exposed:
    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float turnSmoothTime = 0.1f;
    [Header("Audio Members")]
    public AudioSource audio;
    public AudioClip runSound;
    public AudioClip walkSound;

    //Hidden:
    //player
    private Transform player_;
    //this npc
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
        if (moving && inParty_)
        {
            UpdateMovement();
        }
        else
        {
            NPCSpeed = 0f;
            //audio.Stop();
            controller_.Move(new Vector3(0f, -9.8f * Time.deltaTime, 0f));//this is just for gravity when stopped.
        }
    }

    private void UpdateMovement()
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

        if (moving && running) //Handle running movement
        {
            controller_.Move(moveDir.normalized * runSpeed * Time.deltaTime);
            NPCSpeed = runSpeed;

            //UpdateAudio("Run");

        }
        else if (moving) //handle walking movement
        {
            NPCSpeed = walkSpeed;
            controller_.Move(moveDir.normalized * walkSpeed * Time.deltaTime);
            //UpdateAudio("Walk");
        }
        else // just for gravity
        {
            controller_.Move(new Vector3(0f, moveDir.normalized.y, 0f));
        }
    }
}
