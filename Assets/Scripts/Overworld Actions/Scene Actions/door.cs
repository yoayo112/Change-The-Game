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
    private Animator _doorAnim;
    private Animator playerAnim_;
    private Transform player_;
    private Transform frame_;
    private float openDistance_;
    private bool _isOpen;
    private bool _startOpening;
    private bool swinging_;
    public void Operate(bool b)
    {
        swinging_ = b;
    }
    private AudioSource audio_;

    // Start is called before the first frame update
    void Start()
    {
        player_ = GlobalService.Get_Player_Instance().GetComponent<Transform>();
        playerAnim_ = GlobalService.Get_Player_Instance().GetComponentInChildren<Animator>();
        audio_ = GetComponent<AudioSource>();
        audio_.clip = opensound;
        audioplaying_ = false;
        _doorAnim = GetComponent<Animator>();
        frame_ = GetComponent<Transform>().parent;
        openDistance_ = 1.5f;
        _isOpen = false;
        _startOpening = false;
        swinging_ = false;
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
    private IEnumerator Animate_Door_Open()
    {
        yield return new WaitForSeconds(1.3f);

        if(_isOpen == false)
        {
            _doorAnim.SetTrigger("open");
            //enter door anim
            while (!_doorAnim.GetCurrentAnimatorStateInfo(0).IsName("open")) {yield return null;}
            //wait until it's done animating
            while ((_doorAnim.GetCurrentAnimatorStateInfo(0).normalizedTime) < 1f) {yield return null;}
            _isOpen = true;
            swinging_ = false;
        }
    }

    //the meat and potatos of the door swinging open or swinging closed.
    public void swing()
    {
        //find distance
        float xd = Math.Abs(player_.position.x - frame_.position.x);
        float zd = Math.Abs(player_.position.z - frame_.position.z);

        if (!(xd <= openDistance_ && zd <= openDistance_)) { swinging_ = false; return; }
        
        //we're close enough to open a door
        else
        {

            //The distinction between isOpen and _startOpening is the time that they resolve to true. 
            //_startOpening resolves to true after the player starts their animation of reaching forward and turning the handle. The player opened it.
            //isOpen resolves to true only AFTER the player animation and the door animation have finished and the door IS open. 
            //So in the state where _startOpening == true and isOpen == false : we are waiting for one or more of the animations to conlcude.
            if (_isOpen == false && _startOpening == false)
            {
                StartCoroutine(Animate_Door_Open());
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
                _startOpening = true;
            }
            //same for closing
            else if (_isOpen == true && _startOpening == true && swinging_ == true)
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
                _doorAnim.SetTrigger("close");
                playerAnim_.SetTrigger("close");
                _startOpening = false;
                _isOpen = false;
                swinging_ = false;
            }
        }
    }
}
