using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CowboyMovement_v3 : MonoBehaviour
{

    //--------------------------------------------------------
    //  Movement
    //--------------------------------------------------------
    //Exposed
    [Header("Movement Members")]
    [Tooltip("Insert Main Camera here.")]
    public Transform followCam;
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
    private Animator animator;

    void Start()
    {
        //set all components
        animator = GameObject.Find("Cowboy_body").GetComponent<Animator>();
        running = false;
        runHasToggled = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Get Inputs!
        hInput = Input.GetAxis("Horizontal") * gameObject.transform.right;
        vInput = Input.GetAxis("Vertical") * gameObject.transform.forward;
        
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 move = hInput + vInput;
        normalizedSpeed = Vector3.Dot(move.normalized,move.normalized);

        direction = new Vector3(horizontal,0f,vertical).normalized;

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
        else if (moving)
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
}
