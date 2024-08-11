/*
Project: Change the Game
File: TFE_Projectile.cs
Date Created: Aug 10, 2024
Author(s): Sky Vercauteren
Info:

The behavior script attached to a spawned projectile prefab object in the 2048 minigame
*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TFE_Projectile : MonoBehaviour
{
    public GameObject gravityWell;
    private Collider gravitySpace;
    private Rigidbody body;

    public int value;


    // using awake instead of start because these objects are instantiated on click
    void Awake()
    {
        gravityWell = GameObject.Find("GravityWell");
        gravitySpace = gravityWell.GetComponent<Collider>();
        body = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        //if its inside the gravity well, turn off gravity
        if(gravitySpace.bounds.Contains(transform.position))
        {
            body.useGravity = false;
        }

    }

    //when it collides with stuff.
    void OnCollisionEnter(Collision collision)
    {
        string[] names = new string[11] 
        { 
            "2048Bubble(Clone)", 
            "TFE_bubble_4(Clone)", 
            "TFE_bubble_8(Clone)", 
            "TFE_bubble_16(Clone)", 
            "TFE_bubble_32(Clone)", 
            "TFE_bubble_64(Clone)", 
            "TFE_bubble_128(Clone)",
            "TFE_bubble_256(Clone)",
            "TFE_bubble_512(Clone)",
            "TFE_bubble_1024(Clone)",
            "TFE_bubble_2048(Clone)"
       };
        if (names.Contains(collision.gameObject.name))
        {

            GameObject other = collision.gameObject;

            //check the value, they have to be the same
            if (other.GetComponent<TFE_Projectile>().value == value)
            {
                //begin merge

                Vector3 position = collision.transform.position;
                gravityWell.GetComponent<TFE_Merger>().addMergeRequest(gameObject, other, position);
            }
        }
    }
}
