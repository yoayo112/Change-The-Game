using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float runSpeed = 8f;
    public float mouseSensitivity = 2f;
    public GameObject cameraPivot;

    private float playerSpeed;
    private float cameraRotateX = 0f;
    private Rigidbody rigidBody;
    private Animator animator;

    void Start()
    {
        //--get components--
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();

        //--hide the mosue cursor. Press Esc during play to show the cursor. --
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }


    void Update()
    {
        //--get values used for character and camera movement--
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        float mouse_X = Input.GetAxis("Mouse X")*mouseSensitivity;
        float mouse_Y = Input.GetAxis("Mouse Y")*mouseSensitivity;
        //normalize horizontal and vertical input (I am not sure about this one but it seems to work :P)
        float normalizedSpeed = Vector3.Dot(new Vector3(horizontalInput, 0f, verticalInput).normalized, new Vector3(horizontalInput, 0f, verticalInput).normalized);

        //--camera movement and character sideways rotation--
        transform.Rotate(0, mouse_X, 0);
        cameraRotateX -= mouse_Y;
        cameraRotateX = Mathf.Clamp(cameraRotateX, -15, 60); //limites the up/down rotation of the camera 
        cameraPivot.transform.localRotation = Quaternion.Euler(cameraRotateX, 0, 0);

        //--sets Speed parameters in the Animator--
        animator.SetFloat("Speed", playerSpeed);

        //--change playerSpeed and Animator Parameters when the "run" or "crouch" buttons are pressed--
        if (Input.GetButton("Run"))
        {
            transform.Translate(new Vector3(horizontalInput, 0, verticalInput) * runSpeed * Time.deltaTime);
            playerSpeed = Mathf.Lerp(playerSpeed, normalizedSpeed * runSpeed, 0.05f);
        }
        else //this is the standard walk behaviour 
        {
            transform.Translate(new Vector3(horizontalInput, 0, verticalInput) * walkSpeed * Time.deltaTime);
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
