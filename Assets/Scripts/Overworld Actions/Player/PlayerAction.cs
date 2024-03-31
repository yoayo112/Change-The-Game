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
using UnityEngine.UI;

public class PlayerAction : MonoBehaviour
{
    //exposed vars
    public RuntimeAnimatorController movementController;
    public RuntimeAnimatorController combatController;
    public float interactionDistance;

    //internal vars
    private Animator animator_;
    private Transform player_;
    public Transform body_ { get; private set; }
    

    private Canvas speechBubble_;
    public bool isDialogActive()
    {
        return speechBubble_.gameObject.activeInHierarchy;
    }
    private TMP_Text dialogText_;
    private bool talking_;

    private Canvas combatGUI_;

    private bool inCombat_ = false;
    public void Set_Combat(bool t)
    {
       inCombat_ = t;
        if (t == true)
        {
            gameObject.GetComponent<PlayerMovement>().Set_Combat(true);
            animator_.runtimeAnimatorController = combatController;
            //initialize combat GUI
            combatGUI_.renderMode = RenderMode.ScreenSpaceCamera;
            Camera camera = GameObject.Find("Main Camera").GetComponent<Camera>();
            combatGUI_.worldCamera = camera;
            combatGUI_.planeDistance = 2;
        }
        else
        {
            gameObject.GetComponent<PlayerMovement>().Set_Combat(false);
            animator_.runtimeAnimatorController = movementController;
        }
    }

    void Start()
    {
        //player prefab
        player_ = GlobalService.Get_Player_Instance().GetComponent<Transform>();
        animator_ = player_.GetComponentInChildren<Animator>();
        animator_.runtimeAnimatorController = movementController;
        body_ = player_.GetChild(0).GetComponent<Transform>();

        //dialog 
        speechBubble_ = GlobalService.Find_Canvas_In_Children(player_.gameObject, "DialogBox");
        speechBubble_.renderMode = RenderMode.ScreenSpaceCamera;
        Camera camera = GlobalService.Get_Camera().GetComponentInChildren<Camera>(true);
        speechBubble_.worldCamera = camera;
        speechBubble_.planeDistance = Vector3.Distance(camera.gameObject.GetComponent<Transform>().position, player_.position) / 2;
        dialogText_ = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        speechBubble_.GetComponent<GraphicRaycaster>().enabled = false;
        speechBubble_.gameObject.SetActive(false);

        //combat
        combatGUI_ = GlobalService.Find_Canvas_In_Children(player_.gameObject, "CombatGUI");
        combatGUI_.GetComponent<GraphicRaycaster>().enabled = false;
        combatGUI_.gameObject.SetActive(false);
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
        speechBubble_.GetComponent<GraphicRaycaster>().enabled = talking_;
        speechBubble_.gameObject.SetActive(talking_);
        //speechBubble_.enabled = talking_;
    }
}
