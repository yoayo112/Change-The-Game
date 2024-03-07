using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class PriestessMovement : MonoBehaviour
{
    public CharacterController controller;
    private float fallSpeed;
    private bool falling;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveDir = 0 * Vector3.forward; // Makes sure direction is correct with respect to camera

        //apply gravity
        if (controller.isGrounded)
        {
            fallSpeed = 0f;
            falling = false;
        }
        else
        {
            fallSpeed -= 9.8f * Time.deltaTime;
            falling = true;
        }
        moveDir.y = fallSpeed;
        controller.Move(moveDir.normalized);
    }
}
