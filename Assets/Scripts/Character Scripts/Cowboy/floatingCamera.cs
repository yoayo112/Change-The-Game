using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floatingCamera : MonoBehaviour
{

    private Transform player;

    void Start()
    {
        player = GameObject.Find("Cowboy_body").GetComponent<Transform>();
    }


    // Update is called once per frame
    void Update()
    {
        //set "forward" = to the direction the camera is looking. 

        //find the line between the camera and the player 
        //cancel out the y cordinate. Otherwise looking up will cause the player to fly.
        Vector3 direction = player.position - (new Vector3(transform.position.x, player.position.y, transform.position.z));

        //set the forward orientation to be in this direction.
        //normalized just means with a magnitude of 1. like how (1,0,0) means forward with a fixed camera.
        player.forward = direction.normalized;

        
    }
}
