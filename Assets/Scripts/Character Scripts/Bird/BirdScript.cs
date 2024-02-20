using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdScript : MonoBehaviour
{
    public string perchTag;
    public float flyspeed;
    public int minWaitTime;
    public int maxWaitTime;
    private Transform bird;
    private Transform target;
    private bool moving;
    private GameObject[] perches;
    private Animator animator;
    private Timer flyTimer;


    // Start is called before the first frame update
    void Start()
    {
        bird = GetComponent<Transform>();
        animator = GetComponentInChildren<Animator>();
        perches = GameObject.FindGameObjectsWithTag(perchTag);
        flyTimer = new Timer(fly, 5f);
        target = bird;
        moving = false;
    }

    // Update is called once per frame
    void Update()
    {
        //count existing timer - the timer will automatically call fly when it hits 0.
        flyTimer.Update();

        
        //do nothing unless a target is found.
        if (target.position != bird.position)
        {
            //rotate towards new perch - or do nothing if target == current.
            //birds angle on the perch will always == the angle it was facing when it took off.
            Vector3 diff = target.position - bird.position;
            if (Vector3.Angle(bird.forward, diff) != 0)
            {
                Quaternion rotation = Quaternion.LookRotation(diff, Vector3.up);
                bird.rotation = Quaternion.Slerp(bird.rotation, rotation, 0.05f);
            }
            //move bird to new perch - or do nothing if target == current position. 
            else
            {
                bird.position = Vector3.MoveTowards(bird.position, target.position, flyspeed * Time.deltaTime);
                moving = true;
                
            }
        }
        else { moving = false; }
        animator.SetBool("Flying", moving);
    }

    //what to do when the timer hits 0 ansd calls fly. 
    private void fly()
    {
        //generate new perch randomly.
        int index = Random.Range(0, perches.Length);
        target = perches[index].GetComponent<Transform>();

        //generate new time (seconds) to wait before flying again
        float newTime = Random.Range(minWaitTime, maxWaitTime);

        //ok we need to make sure that the wait time should be IN ADDITION to the time it takes to fly to the new perch.
        float distance = Vector3.Distance(bird.position, target.position);
        newTime += (distance / flyspeed);

        //setnewTimer.
        flyTimer = new Timer(fly, newTime);
    }
}
