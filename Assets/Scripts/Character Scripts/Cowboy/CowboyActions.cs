/*
Project: Change the Game
File: CowboyAction.cs
Date Created: February 2, 2024
Author(s): Sky Vercauteren
Info:

Used to control the handful of actions that are unique to the cowboy according to player input.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class CowboyAction : MonoBehaviour
{
    //internal vars
    private Animator animator_;
    
    private ParentConstraint constraint_;
    private List<ConstraintSource> sources_;
    Timer holster;
    private bool inCombat_;
    public void Set_Combat(bool t)
    {
        inCombat_ = t;
    }
    private Transform body_;

    //exposed vars
    public  RuntimeAnimatorController combatController;
    public RuntimeAnimatorController movementController;


    // Start is called before the first frame update
    void Start()
    {
        body_ = gameObject.GetComponent<Transform>();
        animator_ = body_.GetChild(0).GetComponent<Animator>();
        constraint_ = body_.GetChild(1).GetComponent<ParentConstraint>();
        sources_ = new List<ConstraintSource>(constraint_.sourceCount);
        constraint_.GetSources(sources_);
        holster = new Timer(PutAway, 0f);
        inCombat_ = false;
    }

    // Update is called once per frame
    void Update()
    {
        //COMBAT = true;
        if (inCombat_ && holster != null)
        {
            holster.Update(); 
        }

        //"Mlady"
        if (Input.GetButtonDown("Hat"))
        {
            float playerSpeed_ = body_.gameObject.GetComponent<PlayerMovement>().GetPlayerSpeed();
            if (playerSpeed_ < 1.5)
            {
                animator_.SetTrigger("Hat");
            }
            else
            {
                animator_.SetTrigger("movingHat");
            }
        }  
    }

    public void PutAway()
    {
        ConstraintSource holst_ = sources_[0];
        ConstraintSource hand_ = sources_[1];
        holst_.weight = 1;
        hand_.weight = 0;
        sources_[0] = holst_;
        sources_[1] = hand_;
        constraint_.SetSources(sources_);
    }

    public void GetOut()
    {
        ConstraintSource holst_ = sources_[0];
        ConstraintSource hand_ = sources_[1];
        holst_.weight = 0;
        hand_.weight = 1;
        sources_[0] = holst_;
        sources_[1] = hand_;
        constraint_.SetSources(sources_);
    }
}
