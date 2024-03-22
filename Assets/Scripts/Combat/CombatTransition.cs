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

    public bool enteringCombat;

    // Start is called before the first frame update
    void Start()
    {
        //initialization
        player_ = GlobalService.Get_Player_Instance();
        body_ = player_.GetComponent<Transform>().GetChild(0).GetComponent<Transform>();
        transform.GetChild(0).GetComponent<Canvas>().worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        animator_ = transform.GetChild(0).GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //conditionlogic for scene transitions.
        if (portal.GetComponent<Collider>().bounds.Contains(body_.position))
        {
            if(enteringCombat)
            {
                //pass in the enemies
                GlobalService.Get_Main().Set_Enemies(enemies.ToList());
            }
            StartCoroutine(fadeOut(goTo));
        }

    }

    public void GTFO()
    {
        transform.GetComponentInChildren<BoxCollider>().enabled = true;
    }
}
