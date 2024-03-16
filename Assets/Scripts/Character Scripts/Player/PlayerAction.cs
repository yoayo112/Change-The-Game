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
    public bool isDialogActive()
    {
        return speechBubble_.gameObject.active;
    }
    private TMP_Text dialogText_;
    private bool talking_;
    private bool inCombat_ = false;
    public void Set_Combat(bool t)
    {
       inCombat_ = t;
        if (t == true)
        {
            gameObject.GetComponent<PlayerMovement>().Set_Combat(true);
            animator_.runtimeAnimatorController = combatController;
        }
        else
        {
            gameObject.GetComponent<PlayerMovement>().Set_Combat(false);
            animator_.runtimeAnimatorController = movementController;
        }
    }


    //exposed vars
    public RuntimeAnimatorController movementController;
    public RuntimeAnimatorController combatController;
    public float interactionDistance;
    


    // Start is called before the first frame update
    void Start()
    {
        //player prefab
        player_ = GlobalService.Get_Player_Instance().GetComponent<Transform>();
        animator_ = player_.GetChild(0).GetComponent<Animator>();
        animator_.runtimeAnimatorController = movementController;
        body_ = player_.GetChild(0).GetComponent<Transform>();

        

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
        if (Input.GetButtonDown("Interact") && inCombat_ == false)
        {
            Collider[] hitColliders = Physics.OverlapSphere(body_.position, interactionDistance);
            for(int i = 0; i < hitColliders.Length; i++)
            {
                GameObject nearbyThing = hitColliders[i].gameObject;
                switch(nearbyThing.tag)
                {
                    case "NPC":
                        //dialog stuff
                        break;
                    case "Door":
                        nearbyThing.GetComponentInChildren<Door>().Operate(true);
                        break;
                       
                        //TODO: Fun result of interacting with:
                        //-Barell?
                        //-table/chair/pew?

                        //TODO:
                        //Open Chest
                }
            }
        }  
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
