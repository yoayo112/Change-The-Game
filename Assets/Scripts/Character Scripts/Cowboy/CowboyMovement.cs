using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float runSpeed = 8f;
    public float mouseSensitivity = 2f;

    public float cameraHeight = 8f;
    public float cameraFollowDistance = 15f;

    private GameObject cameraPivot;
    private char compass;
    private Transform focus;
    private float playerSpeed;
    private float cameraRotateX = 0f;
    private float cameraRotateY = 0f;
    private Rigidbody rigidBody;
    private Animator animator;
    private Transform body;
    private Transform main;


    void Start()
    {
        //--get components--
        animator = GameObject.Find("Cowboy_body").GetComponent<Animator>();
        body = GameObject.Find("Cowboy_body").GetComponent<Transform>();
        main = GameObject.Find("COWBOY_PREFAB").GetComponent<Transform>();
        cameraPivot = GameObject.Find("Main Camera");
        compass = 'N';
        focus = GameObject.Find("Cowboy_Focus").GetComponent<Transform>();
        focus.position = new Vector3(body.position.x, body.position.y, body.position.z + 1);

        //--hide the mosue cursor. Press Esc during play to show the cursor. --
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }


    void Update()
    {
        //get values 
        Vector3 horizontalInput = Input.GetAxis("Horizontal") * gameObject.transform.right;
        Vector3 verticalInput = Input.GetAxis("Vertical") * gameObject.transform.forward;
        float mouse_X = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouse_Y = Input.GetAxis("Mouse Y") * mouseSensitivity;
        Vector3 move = horizontalInput + verticalInput;
        float normalizedSpeed = Vector3.Dot(move.normalized, move.normalized);

        //camera movement and character sideways rotation

        cameraRotateY += mouse_X;
        cameraRotateX -= mouse_Y;
        cameraRotateX = Mathf.Clamp(cameraRotateX, -60, 90); //limites the up/down rotation of the camera 
        cameraPivot.transform.localRotation = Quaternion.Euler(cameraRotateX, cameraRotateY, 0);

        //Determine which direction camera is facing 
        float facing = cameraPivot.transform.localRotation.eulerAngles.y;
        if (facing <= 305 && facing >= 235) { compass = 'W'; }
        else if (facing >= 145 && facing <= 215) { compass = 'S'; }
        else if (facing >= 55 && facing <= 125) { compass = 'E'; }
        else if (facing <= 35 || facing >= 325) { compass = 'N'; }

        //create a new focus point based on axis inputs to set "LookAt"
        //limit this to a square around the body

        int range = 4;
        float newX = focus.position.x;
        if(newX >= body.position.x + range) { newX = body.position.x + range; }
        if(newX <= body.position.x - range) { newX = body.position.x - range; }
        float newZ = focus.position.z;
        if (newZ >= body.position.z + range) { newZ = body.position.z + range; }
        if (newZ <= body.position.z - range) { newZ = body.position.z - range; }
        focus.position = new Vector3(newX, body.position.y, newZ);

        //float xFocus = 2 * Input.GetAxis("Horizontal");                //RAW - movement and camera serperated
        float xFocus = (2 * Input.GetAxis("Horizontal") + 4 * mouse_X);//BIASED - mouse influences movement. 
        float yFocus = 2 * Input.GetAxis("Vertical");

        //create a new orientation for the camera based on which direction its facing.
        Vector3 newFace = new Vector3(0, body.position.y + cameraHeight, 0);
        Vector3 newFocus = new Vector3(0, 0, 0);

        //lerp the location of the focus point the prefab should look at.
        //and lerp the camera so that every 90 degrees it ends up softly behind the prefab.
        switch (compass)
        {
            case 'W':
                newFace = new Vector3(body.position.x + cameraFollowDistance, newFace.y, body.position.z);
                newFocus = new Vector3(focus.position.x - yFocus, body.position.y, focus.position.z + xFocus);
                break;
            case 'S':
                newFace = new Vector3(body.position.x, newFace.y, body.position.z + cameraFollowDistance);
                newFocus = new Vector3(focus.position.x - xFocus, body.position.y, focus.position.z - yFocus);
                break;
            case 'E':
                newFace = new Vector3(body.position.x - cameraFollowDistance, newFace.y, body.position.z);
                newFocus = new Vector3(focus.position.x + yFocus, body.position.y, focus.position.z - xFocus);
                break;
            case 'N':
                newFace = new Vector3(body.position.x, newFace.y, body.position.z - cameraFollowDistance);
                newFocus = new Vector3(focus.position.x + xFocus, body.position.y, focus.position.z + yFocus);
                break;
        }
        cameraPivot.transform.position = Vector3.Lerp(cameraPivot.transform.position, newFace, 0.013f);
        focus.position = Vector3.Lerp(focus.position, newFocus, 0.02f);

        body.LookAt(focus);

        animator.SetFloat("Speed", playerSpeed);

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
