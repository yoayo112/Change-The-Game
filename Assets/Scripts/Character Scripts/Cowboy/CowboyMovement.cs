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

        //--hide the mosue cursor. Press Esc during play to show the cursor. --
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }


    void Update()
    {
        //--get values used for character and camera movement--
        Vector3 horizontalInput = Input.GetAxis("Horizontal") * gameObject.transform.right;
        Vector3 verticalInput = Input.GetAxis("Vertical") * gameObject.transform.forward;
        float mouse_X = Input.GetAxis("Mouse X")*mouseSensitivity;
        float mouse_Y = Input.GetAxis("Mouse Y")*mouseSensitivity;
        //normalize horizontal and vertical input (I am not sure about this one but it seems to work :P)
        Vector3 move = horizontalInput + verticalInput;
        float normalizedSpeed = Vector3.Dot(move.normalized, move.normalized);

        //--camera movement and character sideways rotation--
        // Allows for player to face camera

        //trans.Rotate(0, Input.GetAxis("Vertical"), 0);
        trans.Rotate(0, mouse_X, 0);

        cameraRotateY += mouse_X;
        cameraRotateX -= mouse_Y;
        cameraRotateX = Mathf.Clamp(cameraRotateX, -60, 90); //limites the up/down rotation of the camera 
        cameraPivot.transform.localRotation = Quaternion.Euler(cameraRotateX, cameraRotateY, 0);
        cameraPivot.transform.position = new Vector3(trans.position.x, trans.position.y+cameraHeight, trans.position.z-cameraFollowDistance);

        //--sets Speed parameters in the Animator--
        animator.SetFloat("Speed", playerSpeed);

        //--change playerSpeed and Animator Parameters when the "run" or "crouch" buttons are pressed--
        if (Input.GetButton("Run"))
        {
            main.transform.Translate(move * runSpeed * Time.deltaTime);
            playerSpeed = Mathf.Lerp(playerSpeed, normalizedSpeed * runSpeed, 0.05f);
        }
        else //this is the standard walk behaviour 
        {
            main.transform.Translate(move * walkSpeed * Time.deltaTime);
            playerSpeed = Mathf.Lerp(playerSpeed, normalizedSpeed * walkSpeed, 1f);
        }

        //--Play the "Special" animation --
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
