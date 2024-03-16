using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicNPCMovement : MonoBehaviour
{

    private CharacterController controller_;
    private bool inCombat = false;
    public void Set_Combat(bool b)
    {
        inCombat = b;
    }

    // Start is called before the first frame update
    void Start()
    {
        controller_ = gameObject.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(inCombat == false)
        {
            //overworld movement
        }
        else
        {
            //for now, there's no movement, but still gravity!
            controller_.Move(new Vector3(0f, -9.8f * Time.deltaTime, 0f));//this is just for gravity when stopped.
        }
    }
}
