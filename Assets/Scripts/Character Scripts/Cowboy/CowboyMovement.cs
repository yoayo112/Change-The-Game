//Movement and Rotation of both a prefab body and a floating (pivot point) camera in 3D space.
//Plus animator/controller triggers.
//Sky Vercauteren 2024

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowboyMovement : MonoBehaviour
{
    //exposed movement
    public float walkSpeed = 2f;
    public float runSpeed = 4f;

    //exposed camera
    public float mouseSensitivity = 2f;
    public float cameraHeight = 2.05f;
    public float cameraFollowDistance = 1.5f;
    public float minFollow = 0;
    public float maxFollow = 20;

    //exposed audio
    public AudioClip runsound;
    public AudioClip walksound;

    //unexposed vars
    private float fixedCameraHeight = 0f;
    private float fixedCameraFollow = 0f;
    private GameObject cameraPivot;
    private float playerSpeed;
    public float getPlayerSpeed()
    {
        return playerSpeed;
    }
    private float cameraRotateX = 0f;
    private float cameraRotateY = 0f;
    private Animator animator;
    private Transform body;
    private Transform main;
    private AudioSource audio;
    private bool audioplaying = false;


    void Start()
    {
        //set all components
        animator = GameObject.Find("Cowboy_body").GetComponent<Animator>();
        body = GameObject.Find("Cowboy_body").GetComponent<Transform>();
        main = GameObject.Find("COWBOY_PREFAB").GetComponent<Transform>();
        cameraPivot = GameObject.Find("Main Camera");
        fixedCameraHeight = cameraHeight;
        fixedCameraFollow = cameraFollowDistance;
        audio = GameObject.Find("Cowboy_body").GetComponent<AudioSource>();
    }


    void Update()
    {
        //get Inputs!
        Vector3 horizontalInput = Input.GetAxis("Horizontal") * gameObject.transform.right;
        Vector3 verticalInput = Input.GetAxis("Vertical") * gameObject.transform.forward;
        float HI = Input.GetAxisRaw("Horizontal");
        float VI = Input.GetAxisRaw("Vertical");
        float mouse_X = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouse_Y = Input.GetAxis("Mouse Y") * mouseSensitivity;
        float facing = cameraPivot.transform.localRotation.eulerAngles.y;

        //set Camera Follow Distance based on scroll wheel!! :D
        if(cameraFollowDistance + 1 > maxFollow) { cameraFollowDistance = maxFollow; }
        if(cameraFollowDistance - 1 < minFollow) { cameraFollowDistance = minFollow; }

        if (cameraFollowDistance <= (fixedCameraFollow/7f)) { cameraHeight = fixedCameraHeight * 1.25f; }
        else if(cameraFollowDistance > 0.25f) { cameraHeight = fixedCameraHeight; }
        cameraFollowDistance += Input.mouseScrollDelta.y * -1f;
        //cameraLerpSpeed += Input.mouseScrollDelta.y * 0.0007f;

        //Body rotation, create a new value for y that we can lerp/rotate to.
        float currentRotation = body.localRotation.eulerAngles.y;
        float newRotation = 0f;
        //modify the new orientation according to the vertical input so that its either moving towards or away from the camera.
        switch (VI)
        {
            case 0f:
                switch(HI)
                {
                    case 0f:
                        newRotation = currentRotation;
                        break;
                    case 1f:
                        newRotation = facing + 90f;
                        break;
                    case -1f:
                        newRotation = facing - 90f;
                        break;
                }
                break;
            case 1f:
                switch (HI)
                {
                    case 0f:
                        newRotation = facing;
                        break;
                    case 1f:
                        newRotation = facing + 45f;
                        break;
                    case -1f:
                        newRotation = facing - 45f;
                        break;
                }
                break;
            case -1f:
                switch (HI)
                {
                    case 0f:
                        newRotation = facing + 180f;
                        break;
                    case 1f:
                        newRotation = facing + 135f;
                        break;
                    case -1f:
                        newRotation = facing - 135f;
                        break;
                }
                break;
        }

        //apply the new orientation!
        body.localRotation = Quaternion.Lerp(body.localRotation, Quaternion.Euler(body.localRotation.x, newRotation, body.localRotation.z), 0.075f);

        //camera rotation
        cameraRotateY += mouse_X;
        cameraRotateX -= mouse_Y;
        cameraRotateX = Mathf.Clamp(cameraRotateX, -60, 90); //limites the up/down rotation of the camera - it gets crazy out there!
        Quaternion newCameraRotation = Quaternion.Euler(cameraRotateX, cameraRotateY, 0);
        cameraPivot.transform.localRotation = newCameraRotation;
        //Rotation and orientation should be done.

        //Body Movement: no lerping :(
        //multiply the z position (not x and y) by the eurler degrees of the new camera angle.
        //but then we want it relative to the body so ++ body position. 
        cameraPivot.transform.position = newCameraRotation * (new Vector3(0,0,-cameraFollowDistance)) + body.position;
        //but of course this means the height will be 0. Or well, the "height" of the bottom of the cowboy. so in a seperate operation:
        cameraPivot.transform.position = (new Vector3(cameraPivot.transform.position.x, body.transform.position.y + cameraHeight, cameraPivot.transform.position.z));
        //which seems like overkill, but oh well.

        
        //Now we just move the body and trigger animations.
        animator.SetFloat("Speed", playerSpeed);

        //calculate movement
        Vector3 move = horizontalInput + verticalInput;
        float normalizedSpeed = Vector3.Dot(move.normalized, move.normalized);

        //change playerSpeed and Animator Parameters when the "run" button is pressed
        bool moving = (Input.GetButton("Horizontal") || Input.GetButton("Vertical"));
        if (Input.GetButton("Run") && moving)
        {
            //movement
            main.transform.Translate(body.forward * runSpeed * Time.deltaTime);
            playerSpeed = Mathf.Lerp(playerSpeed, normalizedSpeed * runSpeed, 0.05f);
            //audio
            if(audio.clip != runsound)
            {
                audio.Stop();
                audio.clip = runsound;
                audio.Play();
            }
            if (audioplaying == false)
            {
                audio.Play();
                audioplaying = true;
            }
        }
        else if(moving) //this is the standard walk behaviour 
        {
            //movement
            main.transform.Translate(body.forward * walkSpeed * Time.deltaTime);
            playerSpeed = Mathf.Lerp(playerSpeed, normalizedSpeed * walkSpeed, 0.05f);
            //audio
            if(audio.clip != walksound)
            {
                audio.Stop();
                audio.clip = walksound;
                audio.Play();
            }
            if (audioplaying == false)
            {
                audio.Play();
                audioplaying = true;
            }
        }
        else //stopped
        {
            playerSpeed = Mathf.Lerp(playerSpeed, normalizedSpeed * 0, 0.05f);
            audio.Stop();
            audioplaying = false;
        }
    }
}
