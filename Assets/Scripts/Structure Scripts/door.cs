using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Door : MonoBehaviour
{
    public AudioClip opensound;
    public AudioClip closesound;

    private bool audioplaying;
    private Animator animator;
    private Transform player;
    private Transform frame;
    private float openDistance;
    private bool isOpen;
    private bool opened;
    private bool swing;
    public void setSwing(bool s)
    {
        swing = s;
    }
    private Timer openWait;
    private AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
        audio.clip = opensound;
        audioplaying = false;
        animator = GetComponent<Animator>();
        player = GameObject.Find("Cowboy_body").GetComponent<Transform>();
        frame = GetComponent<Transform>().parent;
        openDistance = 1.5f;
        isOpen = false;
        opened = false;
        swing = false;
        openWait = new Timer(openDoor, 1.3f);
    }

    // Update is called once per frame
    void Update()
    {
        if(audio.isPlaying == false)
        {
            audioplaying = false;
        }
        if (swing == true)
        {
            //find distance
            float xd = Math.Abs(player.position.x - frame.position.x);
            float zd = Math.Abs(player.position.z - frame.position.z);

            if (xd <= openDistance && zd <= openDistance)
            {
                Vector3 diff = frame.position - player.position;
                
                {
                    openWait.Update();
                    Animator cowboyAnim = GameObject.Find("Cowboy_body").GetComponent<Animator>();
                    if (isOpen == false && opened == false)
                    {
                        cowboyAnim.SetTrigger("open");
                        if (audio.clip != opensound)
                        {
                            audio.Stop();
                            audio.clip = opensound;
                            audio.Play();
                            audioplaying = true;
                        }
                        if (audioplaying == false)
                        {
                            audio.Play();
                            audioplaying = true;
                        }
                        opened = true;
                    }
                    else if(isOpen == true && opened == true && swing == true)
                    {
                        if (audio.clip != closesound)
                        {
                            audio.Stop();
                            audio.clip = closesound;
                            audio.Play();
                            audioplaying = true;
                        }
                        if (audioplaying == false)
                        {
                            audio.Play();
                            audioplaying = true;
                        }
                        animator.SetTrigger("close");
                        cowboyAnim.SetTrigger("close");
                        opened = false;
                        isOpen = false;
                        swing = false;
                    }
                }
            }
        }
    }

    private void openDoor()
    {
        if(isOpen == false)
        {
            animator.SetTrigger("open");
            isOpen = true;
            swing = false;
            
        }
        openWait = new Timer(openDoor, 1.3f);
    }
}
