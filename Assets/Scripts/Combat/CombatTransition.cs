using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatTransition : SceneTransition
{
    [Header("Use this to get from the overworld -> combat. (Not vica-versa)")]

    //reference each enemy. We can do this randomly or something later, for now lets just get each one.
    [Header("List the name of the prefabs you want to fight")]
    public string mainEnemy = "";
    public string enemy2 = "";
    public string enemy3 = "";
    public string enemy4 = "";
    public string enemy5 = "";
    public string enemy6 = "";

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
            StartCoroutine(fadeOut(goTo));
            //pass in the enemies
            List<string> baddies = new List<string> { mainEnemy, enemy2, enemy3, enemy4, enemy5, enemy6 };
            GlobalService.Get_Main().Set_Enemies(baddies);
        }

    }
}
