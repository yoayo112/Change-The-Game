using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Door : MonoBehaviour
{

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

    // Start is called before the first frame update
    void Start()
    {
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
        if (swing == true)
        {
            //find distance
            float xd = Math.Abs(player.position.x - frame.position.x);
            float zd = Math.Abs(player.position.z - frame.position.z);

            if (xd <= openDistance && zd <= openDistance)
            {
                Vector3 diff = frame.position - player.position;
                if (Vector3.Angle(player.forward, diff) >= 0.5)
                {
                    Quaternion rotation = Quaternion.LookRotation(diff, Vector3.up);
                    player.rotation = Quaternion.Slerp(player.rotation, rotation, 0.7f);
                }
                else if(Math.Abs(Vector3.Angle(player.forward, diff)) < 0.5)
                {
                    openWait.Update();
                    Animator cowboyAnim = GameObject.Find("Cowboy_body").GetComponent<Animator>();
                    if (isOpen == false && opened == false)
                    {
                        cowboyAnim.SetTrigger("open");
                        opened = true;
                    }
                    else if(isOpen == true && opened == true && swing == true)
                    {
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
