/*
Project: Change the Game
File: GlobalMain.cs
Date Created: March 06, 2024
Author(s): Sky Vercauteren
Info:

Defines the movement behavior of the NPC's in a players party.
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
    private bool inParty_;
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

    // Start is called before the first frame update
    void Start()
    {
        Transform global = GameObject.Find("_GLOBAL_").GetComponent<Transform>();
        player_ = global.GetChild(0).GetComponent<GlobalMain>().Get_Player().GetComponent<Transform>();
        inParty_ = global.GetChild(0).GetComponent<GlobalMain>().Is_In_Party(transform.gameObject);
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

        //copycat player movement
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
        }
    }

    private void UpdateMovement()
    {
        //rotate to look at player
        Quaternion rotation = Quaternion.LookRotation(player_.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.1f);
        Vector3 moveDir = Quaternion.Euler(0f, rotation.eulerAngles.y, 0f) * Vector3.forward;

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
        else
        {
            controller_.Move(new Vector3(0f, moveDir.normalized.y, 0f));
        }
    }
}
