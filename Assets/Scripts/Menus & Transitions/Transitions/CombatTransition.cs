using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatTransition : SceneTransition
{
    [Header("Use this to get from the overworld -> combat. (Not vica-versa)")]

    //reference each enemy. We can do this randomly or something later, for now lets just get each one.
    [Header("List the name of the prefabs you want to fight")]
    public string[] enemies;

    [Header("Movable spawn locations (gameObjects)")]
    //Spawn Positions
    public GameObject[] partySpawns;
    public GameObject[] baddySpawns;

    [Header("Check this from the overworld")]
    public bool inOverworld;
    [Header("Check this from the combat scene")]
    public bool inCombat;

    [Header("Reference to combat controller and scene camera")]
    public GameObject controller;
    public CinemachineVirtualCamera mainShot;

    protected override void Spawn()
    {
        //We dont want to spawn combat when enetering the overworld!
        if (inCombat)
        {
            //disable the overworld camera and set live camera to static main
            //GlobalService.Get_Camera_Brain().SetActive(false);
            //canvas_.worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            canvas_.worldCamera = GlobalService.Get_Camera_Brain().GetComponent<Camera>();

            //initialize player and enemy lists.
            CombatController.players = new List<Character>();
            CombatController.enemies = new List<Character>();
            //Find all Player and enemy character controls, put them in a list, and sort.

            //Main character first 
            // spawn, initialize and reposition the prefabs
            player_.transform.position = partySpawns[0].transform.position;
            Vector3 initialTarget = new Vector3(baddySpawns[0].transform.position.x, player_.transform.position.y, baddySpawns[0].transform.position.z);
            player_.transform.LookAt(initialTarget, Vector3.up);
            player_.GetComponent<PlayerAction>().Set_Combat(true);

            //Add to the Combat Controllers list of characters
            CombatController.players.Add(player_.GetComponent<Character>());

            //Now the rest of the party
            
            int j = 1;
            foreach (GameObject member in party_)
            {
                CombatController.players.Add(member.GetComponent<Character>());
                member.transform.position = partySpawns[j].transform.position;
                member.transform.LookAt(initialTarget, Vector3.up);
                member.GetComponent<PartyMovement>().Set_Combat(true); 
            }

            //ok now the neerdowells
            List<string> enemyNames = GlobalService.Get_Main().Get_Enemies();
            initialTarget = new Vector3(partySpawns[0].transform.position.x, player_.transform.position.y, partySpawns[0].transform.position.z);
            for (int i = 0; i < enemyNames.Count; i++)
            {
                GameObject baddy = Instantiate(Resources.Load<GameObject>(enemyNames[i]));
                CombatController.enemies.Add(baddy.GetComponent<Character>());
                baddy.transform.position = baddySpawns[i].transform.position;
                baddy.transform.LookAt(initialTarget, Vector3.up);
                baddy.GetComponent<BasicNPCMovement>().Set_Combat(true);
            }
            //Once we are ready to start combat - we can power up the combatController
            controller.GetComponent<CombatController>().Start_Combat();
        }
        else //we are returning to the overword from combat
        {
            //ensure camera is live
            //GlobalService.Get_Camera_Brain().SetActive(true);

            //ensure everyone in the party has overworld behavior
            player_.GetComponent<PlayerAction>().Set_Combat(false);
            foreach (GameObject member in party_)
            {
                member.GetComponent<PartyMovement>().Set_Combat(false);
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        //conditionlogic for scene transitions.
        if (portal.GetComponent<Collider>().bounds.Contains(body_.position))
        {
            //we just hit the collider in the overworld to go into combat
            if (inOverworld)
            {
                //pass in the enemies
                GlobalService.Get_Main().Set_Enemies(enemies.ToList());
            }
            //otherwise we must be int the collider *exiting* combat
            else
            {
                //do cleanup stuff?
            }

            //either way do the thing/
            StartCoroutine(fadeOut(goTo));
        }

    }
}
