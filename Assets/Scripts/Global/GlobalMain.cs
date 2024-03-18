/*
Project: Change the Game
File: GlobalMain.cs
Date Created: March 06, 2024
Author(s): Sky Vercauteren
Info:

A script responsible for managing global data associated with a given session
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalMain : MonoBehaviour
{
    //PLAYER INFORMATION:
    //this is just a reference to the prefab in memory!
    //to get the actual instance in a scene, use GlobalService.Get_Player_Instance();
    private GameObject player_;
    public GameObject Get_Player_Prefab()
    {
        return player_;
    }
    public void Set_Player_Prefab(GameObject p)
    {
        player_ = p;
    }

    //PARTY INFORMATION
    //This is also just a list of prefab references in memory. 
    //use GlobalService.Get_Party_Instances() for actua in-scene objects.
    private List<GameObject> party_;
    public void Set_Party(List<GameObject> others)
    {
        party_ = others;
    }
    public List<GameObject> Get_Party()
    {
        return party_;
    }
    public void Party_Push(GameObject npc)
    {
        //some logic here to make sure the npc is a valid party member and has the correct components.
        //if components ? do nothing : add npc movement script etc.
        party_.Add(npc);
    }
    public bool Is_In_Party(GameObject npc)
    {
        return party_.Contains(npc);
    }

    //TEMP ENEMY INFORMATION
    //this just holds a list of strings, since the combat scene is responsible for resource.load and spawn.
    private List<string> enemies;
    public void Set_Enemies(List<string> baddies)
    {
        enemies = baddies;
    }
    public List<string> Get_Enemies()
    {
        return enemies;
    }
    public void Reset_Enemies()
    {
        enemies.Clear();
    }


    // Start is called before the first frame update
    void Start()
    {
        //Make sure that the _Global_ Object is never destroyed.
        DontDestroyOnLoad(gameObject.transform.root);
    }
}
