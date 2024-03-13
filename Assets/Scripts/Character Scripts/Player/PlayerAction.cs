/*
Project: Change the Game
File: PlayerAction.cs
Date Created: February 2, 2024
Author(s): Sky Vercauteren
Info:

Used to control the actions that are unique to the cowboy according to player input.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using TMPro;

public class PlayerAction : MonoBehaviour
{
    //internal vars
    private Animator animator_;
    private Transform player_;
    private Transform body_;
    

    private Canvas speechBubble_;
    private TMP_Text dialogText_;
    private bool talking_;
    private bool inCombat_;
    public void SetCombat(bool t)
    {
       inCombat_ = t;
    }


    //exposed vars
    public  RuntimeAnimatorController movementController;
    


    // Start is called before the first frame update
    void Start()
    {
        //player prefab
        player_ = GlobalService.Get_Player_Instance().GetComponent<Transform>();
        animator_ = player_.GetChild(0).GetComponent<Animator>();
        animator_.runtimeAnimatorController = movementController;
        body_ = player_.GetChild(0).GetComponent<Transform>();

        //misc
        inCombat_ = false;
        

        //dialog 
        speechBubble_ = player_.gameObject.GetComponentInChildren<Canvas>(true);
        speechBubble_.renderMode = RenderMode.ScreenSpaceCamera;
        speechBubble_.planeDistance = 3f;
        speechBubble_.worldCamera = GlobalService.Get_Camera().GetComponentInChildren<Camera>(true);
        dialogText_ = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        speechBubble_.gameObject.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        //Main interact switch
        if (Input.GetButtonDown("Interact"))
        {
            //determine which action to do! (How? Lol)
            //TODO: General talking with NPC's

            //TODO: Fun result of interacting with:
            //-Barell?
            //-table/chair/pew?

            //TODO:
            //Open Chest


            //Door open/close
            //Temp solution! Needs to work with every scene!
            HometownListener htl_ = FindObjectOfType<HometownListener>();
            Door d_ = GetClosestDoor(htl_.Get_All_Doors());
            if (d_ != null)
            {
                d_.Operate(true);
            }
        }

        
    }

    //finds which door is closest to the cowboy.
    private Door GetClosestDoor(Door[] all_)
    {
        //according to some folks online, the most resource friendly way to find the real distance between points is by using squares.
        Door closestDoor_ = null;
        float closestDistance_ = Mathf.Infinity;
        Vector3 location_ = body_.position;
        foreach (Door d_ in all_)
        {
            Transform door_ = d_.gameObject.GetComponent<Transform>();
            Vector3 direction_ = door_.position - location_;
            float dSqrToTarget_ = direction_.sqrMagnitude;
            if (dSqrToTarget_ < closestDistance_)
            {
                closestDistance_ = dSqrToTarget_;
                closestDoor_ = d_;
            }
        }
        return closestDoor_;
    }

    //toggles dialog box
    public void Dialog(string text)
    {
        if (!talking_)
        {
            talking_ = true;
            dialogText_.text = text;
        }
        else { talking_ = false; }
        speechBubble_.gameObject.SetActive(talking_);
        //speechBubble_.enabled = talking_;
    }
}
