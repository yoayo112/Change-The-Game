//Movement and Rotation of both a prefab body and a floating (pivot point) camera in 3D space.
//Plus animator/controller triggers.
//Sky Vercauteren 2024

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float runSpeed = 8f;
    public float mouseSensitivity = 2f;
    public float mouseBias = 2f;

    public float cameraHeight = 8f;
    public float cameraFollowDistance = 15f;

    private GameObject cameraPivot;
    private char compass;
    private bool forward = true;
    private float playerSpeed;
    private float cameraRotateX = 0f;
    private float cameraRotateY = 0f;
    private Rigidbody rigidBody;
    private Animator animator;
    private Transform body;
    private Transform main;


    void Start()
    {
        //set all components
        animator = GameObject.Find("Cowboy_body").GetComponent<Animator>();
        body = GameObject.Find("Cowboy_body").GetComponent<Transform>();
        main = GameObject.Find("COWBOY_PREFAB").GetComponent<Transform>();
        cameraPivot = GameObject.Find("Main Camera");
        compass = 'N';
    }


    void Update()
    {
        //get values 
        Vector3 horizontalInput = Input.GetAxis("Horizontal") * gameObject.transform.right;
        Vector3 verticalInput = Input.GetAxis("Vertical") * gameObject.transform.forward;
        float HI = Input.GetAxisRaw("Horizontal");
        float VI = Input.GetAxisRaw("Vertical");
        float mouse_X = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouse_Y = Input.GetAxis("Mouse Y") * mouseSensitivity;
        float facing = cameraPivot.transform.localRotation.eulerAngles.y;

        //Body rotation:
        float currentRotation = body.localRotation.eulerAngles.y;
        float newRotation = 0f;
        float mouseFocus = mouse_X * mouseBias;
        //modify the new orientation according to the vertical input and flip the body around if it's negative. 
        switch (VI)
        {
            case 0f:
                newRotation = currentRotation;
                break;
            case 1f:
                if (forward == false) { newRotation = 180f - currentRotation; forward = true; }
                else { newRotation = currentRotation; }
                break;
            case -1f:
                if (forward == true) { newRotation = 180f - currentRotation; forward = false; HI = HI * -1f; }
                else { newRotation = currentRotation; }
                break;
        }
        //Now modify the new orientation according to horizontal input, increase the angle theta to rotate clockwise, decrease for counter-clockwise.
        Debug.Log(Mathf.Abs(currentRotation - facing));
        if(Mathf.Abs(currentRotation - facing) >= 90 && Mathf.Abs(currentRotation - facing) <= 270)
        {
            mouseFocus = mouseFocus * -1f;
        }
        newRotation = newRotation + HI;
        //newRotation = newRotation + mouseFocus;
        //apply the new orientation!
        body.localRotation = Quaternion.Euler(0f, newRotation, 0f);

        //camera movement
        cameraRotateY += mouse_X;
        cameraRotateX -= mouse_Y;
        cameraRotateX = Mathf.Clamp(cameraRotateX, -60, 90); //limites the up/down rotation of the camera 
        cameraPivot.transform.localRotation = Quaternion.Euler(cameraRotateX, cameraRotateY, 0);

        //Determine which direction camera is facing 
        if (facing <= 305 && facing >= 235) { compass = 'W'; }
        else if (facing >= 145 && facing <= 215) { compass = 'S'; }
        else if (facing >= 55 && facing <= 125) { compass = 'E'; }
        else if (facing <= 35 || facing >= 325) { compass = 'N'; }

        //create a new orientation for the camera based on which direction its facing.
        Vector3 newFace = new Vector3(0, body.position.y + cameraHeight, 0);

        //lerp the camera so that every 90 degrees it ends up softly behind the prefab.
        switch (compass)
        {
            case 'W':
                newFace = new Vector3(body.position.x + cameraFollowDistance, newFace.y, body.position.z);
                break;
            case 'S':
                newFace = new Vector3(body.position.x, newFace.y, body.position.z + cameraFollowDistance);
                break;
            case 'E':
                newFace = new Vector3(body.position.x - cameraFollowDistance, newFace.y, body.position.z);
                break;
            case 'N':
                newFace = new Vector3(body.position.x, newFace.y, body.position.z - cameraFollowDistance);
                break;
        }
        cameraPivot.transform.position = Vector3.Lerp(cameraPivot.transform.position, newFace, 0.013f);

        //Rotation and orientation should be done.
        //Now we just move the body and trigger animations.
        animator.SetFloat("Speed", playerSpeed);

        //calculate movement
        Vector3 move = horizontalInput + verticalInput;
        float normalizedSpeed = Vector3.Dot(move.normalized, move.normalized);

        //change playerSpeed and Animator Parameters when the "run" button is pressed
        bool moving = (Input.GetButton("Horizontal") || Input.GetButton("Vertical"));
        if (Input.GetButton("Run") && moving)
        {
            main.transform.Translate(body.forward * runSpeed * Time.deltaTime);
            playerSpeed = Mathf.Lerp(playerSpeed, normalizedSpeed * runSpeed, 0.05f);
        }
        else if(moving) //this is the standard walk behaviour 
        {
            main.transform.Translate(body.forward * walkSpeed * Time.deltaTime);
            playerSpeed = Mathf.Lerp(playerSpeed, normalizedSpeed * walkSpeed, 0.05f);
        }
        else //stopped
        {
            playerSpeed = Mathf.Lerp(playerSpeed, normalizedSpeed * 0, 0.05f);
        }

        //"Mlady"
        if (Input.GetButtonDown("Hat"))
        {
            if(playerSpeed < 1.5)
            {
                animator.SetTrigger("Hat");
            }
            else
            {
                animator.SetTrigger("movingHat");
            }
        }
    }
}
