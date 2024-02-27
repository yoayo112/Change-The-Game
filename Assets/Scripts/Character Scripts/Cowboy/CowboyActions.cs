using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class CowboyActions : MonoBehaviour
{
    //internal vars
    private Animator animator;
    private Door[] doors;
    private Transform body;
    private bool COMBAT;
    public void setCombat(bool t)
    {
        COMBAT = t;
    }
    private ParentConstraint constraint;
    private List<ConstraintSource> sources;
    Timer holster;

    //exposed vars
    public  RuntimeAnimatorController combatController;
    public  RuntimeAnimatorController movementController;
    


    // Start is called before the first frame update
    void Start()
    {
        animator = GameObject.Find("Cowboy_body").GetComponent<Animator>();
        body = GameObject.Find("Cowboy_body").GetComponent<Transform>();
        constraint = GameObject.Find("Gun").GetComponent<ParentConstraint>();
        sources = new List<ConstraintSource>(constraint.sourceCount);
        constraint.GetSources(sources);
        doors = FindObjectsOfType<Door>();
        holster = new Timer(putAway, 0f);
        COMBAT = false;
    }

    // Update is called once per frame
    void Update()
    {
        //COMBAT = true;
        if (COMBAT)
        {
            animator.runtimeAnimatorController = combatController;
            if (holster != null) { holster.Update(); }
        }
        else
        {
            animator.runtimeAnimatorController = movementController;
        }
        //"Mlady"
        if (Input.GetButtonDown("Hat"))
        {
            float playerSpeed = GameObject.Find("COWBOY_PREFAB").GetComponent<CowboyMovement_v3>().GetPlayerSpeed();
            if (playerSpeed < 1.5)
            {
                Debug.Log("Setting trigger hat");
                animator.SetTrigger("Hat");
            }
            else
            {
                animator.SetTrigger("movingHat");
            }
        }

        //Main interact switch
        if (Input.GetButtonDown("Interact"))
        {
            //Door open/close
            if(doors.Length > 0)
            {
                Door d = getClosestDoor(doors);
                d.setSwing(true);
            }
            
            if(COMBAT)
            {
                holster = new Timer(putAway, 3f);
                getOut();
                animator.SetTrigger("attack");
            }
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

    private void putAway()
    {
        ConstraintSource holst = sources[0];
        ConstraintSource hand = sources[1];
        holst.weight = 1;
        hand.weight = 0;
        sources[0] = holst;
        sources[1] = hand;
        constraint.SetSources(sources);
    }

    private void getOut()
    {
        ConstraintSource holst = sources[0];
        ConstraintSource hand = sources[1];
        holst.weight = 0;
        hand.weight = 1;
        sources[0] = holst;
        sources[1] = hand;
        constraint.SetSources(sources);
    }
}
