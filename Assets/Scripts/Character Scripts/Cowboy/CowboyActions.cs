using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowboyActions : MonoBehaviour
{
    private Animator animator;
    private Door[] doors;
    private Transform body;


    // Start is called before the first frame update
    void Start()
    {
        animator = GameObject.Find("Cowboy_body").GetComponent<Animator>();
        body = GameObject.Find("Cowboy_body").GetComponent<Transform>();
        doors = FindObjectsOfType<Door>();
    }

    // Update is called once per frame
    void Update()
    {
        //"Mlady"
        if (Input.GetButtonDown("Hat"))
        {
            float playerSpeed = GameObject.Find("Cowboy_body").GetComponent<CowboyMovement>().getPlayerSpeed();
            if (playerSpeed < 1.5)
            {
                animator.SetTrigger("Hat");
            }
            else
            {
                animator.SetTrigger("movingHat");
            }
        }

        //Door open/close
        if (Input.GetButtonDown("Interact"))
        {
            Door d = getClosestDoor(doors);
            d.setSwing(true);
        }
    }

    //finds which door is closest to the cowboy.
    private Door getClosestDoor(Door[] all)
    {
        //according to some folks online, the most resource friendly way to find the real distance between points is by using squares.
        Door closestDoor = null;
        float closestDistance = Mathf.Infinity;
        Vector3 location = body.position;
        foreach (Door d in all)
        {
            Transform door = d.GetComponent<Transform>();
            Vector3 direction = door.position - location;
            float dSqrToTarget = direction.sqrMagnitude;
            if (dSqrToTarget < closestDistance)
            {
                closestDistance = dSqrToTarget;
                closestDoor = d;
            }
        }
        return closestDoor;
    }
}
