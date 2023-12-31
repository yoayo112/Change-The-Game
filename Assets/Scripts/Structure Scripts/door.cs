using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class door : MonoBehaviour
{

    private Animator animator;
    private Transform player;
    private Transform frame;
    public int openDistance;
    private Boolean isOpen;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.Find("cowboy").GetComponent<Transform>();
        frame = GetComponent<Transform>();
        openDistance = 8;
        isOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        //find distance
        float xd = Math.Abs(player.position.x - frame.position.x);
        float zd = Math.Abs(player.position.z - frame.position.z);
        //Debug.Log(xd + "," + zd);
        Debug.Log(isOpen);

        if ((xd <= openDistance && zd <= openDistance) && isOpen == false)
        {
            animator.SetTrigger("open");
            isOpen = true;
            Debug.Log("Hi");
        }
        if ((xd > openDistance + 1 || zd > openDistance +1) && isOpen == true)
        {
            animator.SetTrigger("close");
            isOpen = false;
        }
    }
}
