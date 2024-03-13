/*
Project: Change the Game
File: Door.cs
Date Created: February 2024
Author(s): Sky Vercauteren
Info:

tracks the animation of a player interacting with a given door.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Door : MonoBehaviour
{
    public AudioClip opensound;
    public AudioClip closesound;

    private bool audioplaying_;
    private Animator doorAnim_;
    private Animator playerAnim_;
    private Transform player_;
    private Transform frame_;
    private float openDistance_;
    private bool isOpen_;
    private bool wasOpened_;
    private bool swinging_;
    public void Operate(bool b)
    {
        swinging_ = b;
    }
    private Timer doorDelay_;
    private AudioSource audio_;

    // Start is called before the first frame update
    void Start()
    {
        player_ = GlobalService.Get_Player_Instance().GetComponent<Transform>();
        playerAnim_ = GlobalService.Get_Player_Instance().GetComponentInChildren<Animator>();
        audio_ = GetComponent<AudioSource>();
        audio_.clip = opensound;
        audioplaying_ = false;
        doorAnim_ = GetComponent<Animator>();
        frame_ = GetComponent<Transform>().parent;
        openDistance_ = 1.5f;
        isOpen_ = false;
        wasOpened_ = false;
        swinging_ = false;
        doorDelay_ = new Timer(Animate_Door_Open, 1.3f);
    }

    // Update is called once per frame
    void Update()
    {
        if(audio_.isPlaying == false)
        {
            audioplaying_ = false;
        }
        
        if(swinging_ == true)
        {
            swing();
        }
    }
    
    //used to delay the door animating so it happens after the player steps forawrd and grabs the handle, not simultaneously.
    private void Animate_Door_Open()
    {
        if(isOpen_ == false)
        {
            doorAnim_.SetTrigger("open");
            isOpen_ = true;
            swinging_ = false;
        }
        doorDelay_ = new Timer(Animate_Door_Open, 1.3f);
    }

    //the meat and potatos of the door swinging open or swinging closed.
    public void swing()
    {
        //find distance
        float xd = Math.Abs(player_.position.x - frame_.position.x);
        float zd = Math.Abs(player_.position.z - frame_.position.z);

        if (!(xd <= openDistance_ && zd <= openDistance_)) { swinging_ = false; return; }
        else
        {
            Vector3 diff = frame_.position - player_.position;
            //if in range - run delay timer.
            if (isOpen_ == false) { doorDelay_.Update(); }
            
            //I know this is confusing but the distinction between isOpen and wasOpened is when they resolve to true. 
            //wasOpened resolves to true after the player starts their animation of reaching forward and turning the handle. The player opened it.
            //isOpen resolves to true only AFTER the player animation and the door animation have finished and the door IS open. 
            //So in the state where wasOpened == true and isOpen == false : we are waiting for one or more of the animations to conlcude.
            if (isOpen_ == false && wasOpened_ == false)
            {
                playerAnim_.SetTrigger("open");
                if (audio_.clip != opensound)
                {
                    audio_.Stop();
                    audio_.clip = opensound;
                    audio_.Play();
                    audioplaying_ = true;
                }
                if (audioplaying_ == false)
                {
                    audio_.Play();
                    audioplaying_ = true;
                }
                wasOpened_ = true;
            }
            //same for closing
            else if (isOpen_ == true && wasOpened_ == true && swinging_ == true)
            {
                if (audio_.clip != closesound)
                {
                    audio_.Stop();
                    audio_.clip = closesound;
                    audio_.Play();
                    audioplaying_ = true;
                }
                if (audioplaying_ == false)
                {
                    audio_.Play();
                    audioplaying_ = true;
                }
                doorAnim_.SetTrigger("close");
                playerAnim_.SetTrigger("close");
                wasOpened_ = false;
                isOpen_ = false;
                swinging_ = false;
            }
        }
    }
}
