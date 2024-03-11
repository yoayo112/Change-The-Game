using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    //-------------
    //Start Menu
    //-------------
    private GlobalMain main_;

    void Awake()
    {
        //this is literally just so the cowboy is interactable in the start menu.      
        main_ = GlobalService.Get_Main();            // = GameObject.Find("_GLOBAL_").GetComponent<Transform>().GetChild(0).GetComponent<GlobalMain>();
        main_.Set_Player(Resources.Load<GameObject>("COWBOY_PREFAB"));
    }

    public void Play()
    {
        //this is purely temporary, there should be a selection by the player.
        // Also, this should Resources.Load so we dont have to have all the prefabs in the scene.
        main_.Set_Player(Resources.Load<GameObject>("COWBOY_PREFAB"));
        main_.Set_Party(new List<GameObject>());

        //This should actually probably be a cutscene.
        SceneManager.LoadScene("HomeTown_v2"); //should probably start at family farm at some point but Oh well
    }

    public void Quit()
    {
        Application.Quit();
    }
}
