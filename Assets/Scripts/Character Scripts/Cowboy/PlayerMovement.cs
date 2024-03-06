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
    [Header("Movement Members")]
    public CharacterController controller;
    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float turnSmoothTime = 0.1f;
    //Unexposed
    private float fallSpeed;
    private float turnSmoothVelocity;
    private float normalizedSpeed;
    private float playerSpeed;
    public float GetPlayerSpeed(){return playerSpeed;}
    private Vector3 direction;
    private Vector3 hInput;
    private Vector3 vInput;
    private bool moving;
    private bool runHasToggled;
    private bool running;
    private bool falling;

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

        //move the character once dynamically, otherwise gravity wont be applied until the user moves.
        //controller.Move(new Vector3(1,1,1).normalized * 1 * Time.deltaTime);
    }

    void Update()
    {
        // Get Inputs!
        hInput = Input.GetAxis("Horizontal") * gameObject.transform.right;
        vInput = Input.GetAxis("Vertical") * gameObject.transform.forward;
        
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        float mouseScroll = Input.GetAxis("Mouse ScrollWheel");

        //add scroll zoom
        if(mouseScroll != 0f)
        {
            if(mouseScroll > 0)
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
        normalizedSpeed = Vector3.Dot(move.normalized,move.normalized);

        direction = new Vector3(horizontal,0f,vertical).normalized;

        //trigger animations
        moving = direction.magnitude >= 0.1f;
        if(Input.GetButton("Run"))
        {
            if(!runHasToggled)
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
        if(moving)
        {
            UpdateMovement();
        }
        else
        {
            playerSpeed = 0f;
            audio.Stop();
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

        if(moving && running) //Handle running movement
        {   
            controller.Move(moveDir.normalized * runSpeed * Time.deltaTime);
            playerSpeed = runSpeed;
    
            UpdateAudio("Run");

        }
        else if (moving) //handle walking movement
        {
            playerSpeed = walkSpeed;
            controller.Move(moveDir.normalized * walkSpeed * Time.deltaTime);
            UpdateAudio("Walk");
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
