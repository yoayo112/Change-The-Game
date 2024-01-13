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
    private float playerSpeed;
    private float cameraRotateX = 0f;
    private float cameraRotateY = 0f;
    private Rigidbody rigidBody;
    private Animator animator;
    private Transform trans;
    private Transform main;


    void Start()
    {
        //--get components--
        animator = GameObject.Find("Cowboy_body").GetComponent<Animator>();
        trans = GameObject.Find("Cowboy_body").GetComponent<Transform>();
        main = GameObject.Find("COWBOY_PREFAB").GetComponent<Transform>();
        cameraPivot = GameObject.Find("Main Camera");
        compass = 'N';
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

        //trans.Rotate(0, Input.GetAxis("Vertical"), 0);
        trans.Rotate(0, mouse_X, 0);

        cameraRotateY += mouse_X;
        cameraRotateX -= mouse_Y;
        cameraRotateX = Mathf.Clamp(cameraRotateX, -60, 90); //limites the up/down rotation of the camera 
        cameraPivot.transform.localRotation = Quaternion.Euler(cameraRotateX, cameraRotateY, 0);

        //Determine which direction character is facing and lerp the camera so that every 90 degrees it ends up softly behind the prefab.
        float facing = cameraPivot.transform.localRotation.eulerAngles.y;
        if (facing <= 300 && facing >= 240) { compass = 'W'; }
        else if (facing >= 150 && facing <= 210) { compass = 'S'; }
        else if (facing >= 60 && facing <= 120) { compass = 'E'; }
        else if (facing <= 30 || facing >= 330) { compass = 'N'; }

        Vector3 newFace = new Vector3(0, trans.position.y + cameraHeight, 0);
        switch(compass)
        {
            case 'W': newFace = new Vector3(trans.position.x + cameraFollowDistance, newFace.y, trans.position.z); break;
            case 'S': newFace = new Vector3(trans.position.x, newFace.y, trans.position.z + cameraFollowDistance); break;
            case 'E': newFace = new Vector3(trans.position.x - cameraFollowDistance, newFace.y, trans.position.z); break;
            case 'N': newFace = new Vector3(trans.position.x, newFace.y, trans.position.z - cameraFollowDistance); break;
        }
        cameraPivot.transform.position = Vector3.Lerp(cameraPivot.transform.position, newFace, 0.013f);

        animator.SetFloat("Speed", playerSpeed);

        //TODO: Ok now prefab needs to rotate 90 with input keys.

        //change playerSpeed and Animator Parameters when the "run" button is pressed
        if (Input.GetButton("Run"))
        {
            main.transform.Translate(move * runSpeed * Time.deltaTime);
            playerSpeed = Mathf.Lerp(playerSpeed, normalizedSpeed * runSpeed, 0.05f);
        }
        else //this is the standard walk behaviour 
        {
            main.transform.Translate(move * walkSpeed * Time.deltaTime);
            playerSpeed = Mathf.Lerp(playerSpeed, normalizedSpeed * walkSpeed, 0.5f);
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
