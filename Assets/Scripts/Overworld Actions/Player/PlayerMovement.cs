/*
Project: Change the Game
File: PlayerMovement.cs
Date Created: February 2, 2024
Author(s): Elijah Theander, Sky Vercauteren
Info: Version 3.1
(V1: cowboy movement, V2: cowboy movement new camera, V3: cowboy movement cinemachine. V4: Player Movement using GlobalService)

Handles player input on the selected character in the overworld.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerMovement : MonoBehaviour
{
    //--------------------------------------------------------
    //  Camera
    //--------------------------------------------------------
    //Exposed
    [Header("Camera Members")]
    public Transform followCam;
    public CinemachineFreeLook virtualCam;

    [Header("Camera Settings")]
    public float maxZoomOut = 1.5f; //150% of start value.
    public float maxZoomIn = 0.5f; //50% of start value.
    public float zoomIncrement = 0.1f; //What percentage value each zoom input will increment by.
    public float zoomLerpTime = 0.5f;

    //Camera orbit radii.
    private float topStartRad, midStartRad, botStartRad;
    private float topMaxRad, midMaxRad, botMaxRad;
    private float topMinRad, midMinRad, botMinRad;
    private float topCurRad, midCurRad, botCurRad; //Current Values

    //--------------------------------------------------------
    //  Movement
    //--------------------------------------------------------
    //Exposed
    [Header("Movement Settings")]
    public float walkSpeed;
    public float runSpeed;
    public float turnSmoothTime = 0.1f;
    public GameObject combatTarget;
    //Unexposed

    public CharacterController controller_;
    private float fallSpeed;
    private float turnSmoothVelocity;
    private float normalizedSpeed;
    private float playerSpeed;
    public float GetPlayerSpeed(){return playerSpeed;}
    private Vector3 direction;
    private Vector3 hInput;
    private Vector3 vInput;
    private bool moving;
    public bool getMoving() { return moving; }
    private bool runHasToggled;
    private bool running;
    public bool getRunning() { return running; }
    private bool falling;
    private bool inCombat = false;
    public void Set_Combat(bool b)
    {
        inCombat = b;
    }

    //--------------------------------------------------------
    //  Audio
    //--------------------------------------------------------
    //Exposed
    [Header("Audio Members")]
    public AudioSource audio;
    public AudioClip runSound;
    public AudioClip walkSound;
    // -------------------------------------------------------
    //  Animation
    //--------------------------------------------------------
    public Animator animator;

    void Start()
    {
        //set all components
        controller_ = GlobalService.Get_Player_Instance().GetComponent<CharacterController>();
        running = false;
        runHasToggled = false;
        //Cursor.lockState = CursorLockMode.Locked;

        topStartRad = virtualCam.m_Orbits[0].m_Radius;
        midStartRad = virtualCam.m_Orbits[1].m_Radius;
        botStartRad = virtualCam.m_Orbits[2].m_Radius;

        topCurRad = topStartRad;
        midCurRad = midStartRad;
        botCurRad = botStartRad;

        topMaxRad = topStartRad * maxZoomOut;
        midMaxRad = midStartRad * maxZoomOut;
        botMaxRad = botStartRad * maxZoomOut;

        topMinRad = topStartRad * maxZoomIn;
        midMinRad = midStartRad * maxZoomIn;
        botMinRad = botStartRad * maxZoomIn;

    }

    void Update()
    {
        if(inCombat == false)
        {
            // Get Inputs!
            hInput = Input.GetAxis("Horizontal") * gameObject.transform.right;
            vInput = Input.GetAxis("Vertical") * gameObject.transform.forward;

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            float mouseScroll = Input.GetAxis("Mouse ScrollWheel");

            //add scroll zoom
            if (mouseScroll != 0f)
            {
                if (mouseScroll > 0)
                {
                    UpdateCamRadius("Zoom_in");
                }
                else
                {
                    UpdateCamRadius("Zoom_out");
                }
            }

            //get applied movement
            Vector3 move = hInput + vInput;
            normalizedSpeed = Vector3.Dot(move.normalized, move.normalized);

            direction = new Vector3(horizontal, 0f, vertical).normalized;

            //trigger animations
            moving = direction.magnitude >= 0.1f;
            if (Input.GetButton("Run"))
            {
                if (!runHasToggled)
                {
                    running = !running;
                    runHasToggled = true;
                }
            }
            else
            {
                runHasToggled = false;
            }

            animator.SetFloat("Speed", playerSpeed);

            //apply movement
            if (moving)
            {
                UpdateMovement();
            }
            else
            {
                playerSpeed = 0f;
                audio.Stop();
                controller_.Move(new Vector3(0f, -9.8f * Time.deltaTime, 0f));//this is just for gravity when stopped.
            }
        }
        else // we still want gravity though!!!! also, Look at the target.
        {
            controller_.Move(new Vector3(0f, -9.8f * Time.deltaTime, 0f));//this is just for gravity when stopped.
        }
        
        
    }

    private void UpdateMovement()
    {
        //Have character model face direction of input.
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + followCam.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y,targetAngle, ref turnSmoothVelocity, turnSmoothTime);

        transform.rotation = Quaternion.Euler(0f, angle, 0f);
        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward; // Makes sure direction is correct with respect to camera
        
        //apply gravity
        if (controller_.isGrounded)
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

        if(moving && running) //Handle running movement
        {   
            controller_.Move(moveDir.normalized * runSpeed * Time.deltaTime);
            playerSpeed = runSpeed;
    
            UpdateAudio("Run");

        }
        else if (moving) //handle walking movement
        {
            playerSpeed = walkSpeed;
            controller_.Move(moveDir.normalized * walkSpeed * Time.deltaTime);
            UpdateAudio("Walk");
        }
        else
        {
            controller_.Move(new Vector3(0f, moveDir.normalized.y, 0f));
        }
    }

    private void UpdateAudio(string clip)
    {
        if(clip == "Run")
        {
            audio.clip = runSound;
        }
        if(clip == "Walk")
        {
            audio.clip = walkSound;
        }

        if(!audio.isPlaying && !falling)
        {
            audio.Play();
        }
    }

    private void UpdateCamRadius(string dir)
    {
        float temp = 0f;

        if(dir == "Zoom_in")
        {
            temp = topCurRad - (topStartRad * zoomIncrement);
            if(temp >= topMinRad)
            {
                topCurRad = Mathf.Lerp(topCurRad, temp, zoomLerpTime);
            }

            temp = midCurRad - (midStartRad * zoomIncrement);
            if(temp >= midMinRad)
            {
                midCurRad = Mathf.Lerp(midCurRad, temp, zoomLerpTime);
            }
            temp = botCurRad - (botStartRad * zoomIncrement);
            if(temp >= botMinRad)
            {
                botCurRad = Mathf.Lerp(botCurRad, temp, zoomLerpTime);
            }
        }
        
        if(dir == "Zoom_out")
        {
            temp = topCurRad + (topStartRad * zoomIncrement);
            if(temp <= topMaxRad)
            {
                topCurRad = Mathf.Lerp(topCurRad, temp, zoomLerpTime);
            }
            temp = midCurRad + (midStartRad * zoomIncrement);
            if(temp <= midMaxRad)
            {
                midCurRad = Mathf.Lerp(midCurRad, temp, zoomLerpTime);
            }
            temp = botCurRad + (botStartRad * zoomIncrement);
            if(temp <= botMaxRad)
            {
                botCurRad = Mathf.Lerp(botCurRad, temp, zoomLerpTime);
            }
        }

        virtualCam.m_Orbits[0].m_Radius = topCurRad;
        virtualCam.m_Orbits[1].m_Radius = midCurRad;
        virtualCam.m_Orbits[2].m_Radius = botCurRad;
    }

}
