/*
Project: Change the Game
File: GlobalService.cs
Date Created: March 05, 2024
Author(s): Sean Thornton, Sky Vercauteren
Info:

Service with functions returning the global transform and its components. 
All methods are static. No constructor, not to be instantiated.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalService
{
    private static Dictionary<string, int> children = new Dictionary<string, int>() { { Get_Global().GetChild(0).gameObject.name, 0 }, { Get_Global().GetChild(1).gameObject.name, 1 } };
    private static int numberOfChildren_ = 2;
    private static string playerObjectName_;

    //Returns the BIG enchilada.
    static public Transform Get_Global()
    {
        return GameObject.Find("_GLOBAL_").GetComponent<Transform>();
    }

    //returns a child gameObject given the name of that object, or null
    static public GameObject Get_Child(string name)
    {
        if (children.ContainsKey(name))
        {
            return Get_Global().GetChild(children[name]).gameObject;
        }
        else return null;
        
    }

    //returns the smaller but arguably more important enchilada.
    static public GlobalMain Get_Main() 
    {
        return Get_Child("Global Main Script").GetComponent<GlobalMain>();
    }

    //returns the main camera
    //this needs to return only the gameObject so that it includes the audioListener and Cinemachine components.
    //to access the camera itself you can add the suffix call Get_Camera().GetComponent<Camera>();
    static public GameObject Get_Camera() 
    {
        return Get_Child("Main Camera").gameObject;
    }

    //returns a reference to the instance of the player object saved/stored in _Global_
    static public GameObject Get_Player_Instance()
    {
        return Get_Child(playerObjectName_);
    }

    //saves/stores an instance of a prefab in _GLOBAL_ that we use as the player!
    static public void Set_Player_Instance(GameObject o)
    {
        playerObjectName_ = o.name;
        Add_Global_Object(o);
    }

    //returns a list of prefabs as original reference templates to the party
    static public List<GameObject> Get_Party()
    {
        return Get_Main().Get_Party();
    }

    //returns true if the prefab is listen in the party AND the prefab is real in _Global_
    static public List<GameObject> Get_Real_Party()
    {
        List<GameObject> party = Get_Party();
        List<GameObject> realMembers = new List<GameObject>();
        for (int i = 0; i < party.Count; i++)
        {
            GameObject member = Get_Child(party[i].name);
            if (member != null)
            {
                realMembers.Add(member);
            }
        }
        return realMembers;
    }

    //saves/stores an object to _GLOBAL_
    static public void Add_Global_Object(GameObject o)
    {
        o.transform.parent = Get_Global();
        children.Add(o.name, numberOfChildren_ - 1);
        numberOfChildren_++;
        Sort_Children();
    }

    //unsaves an object from _GLOBAL_ but doesn't destroy the object. It simply won't be saved into the next scene.
    static public void Remove_Global_Object(GameObject o)
    {
        if(children.ContainsKey(o.name))
        {
            o.transform.parent = null;
        }
    }

    private static void Sort_Children()
    {
        int count_ = Get_Global().childCount;
        for(int i = 0; i < count_; i ++)
        {
            children[Get_Global().GetChild(i).name] = i;
        }
    }

}
