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
using System.Linq;

public class GlobalService
{
    private static Dictionary<string, int> children = new Dictionary<string, int>() {
        { Get_Global().GetChild(0).gameObject.name, 0 },
        { Get_Global().GetChild(1).gameObject.name, 1 },
        { Get_Global().GetChild(2).gameObject.name, 2 } };
    private static int numberOfChildren_ = 2;
    private static string playerObjectName_;
    private static string followCam_;

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

    //returns names of all game objects in _GLOBAL_, each can be accessed with GlobalService.Get_Child(name);
    static public string[] Get_All_Children()
    {
        Sort_Children();
        string[] names = children.Keys.ToArray();
        return names;
    }

    //returns the smaller but arguably more important enchilada.
    static public GlobalMain Get_Main() 
    {
        return Get_Child("Global Main Script").GetComponent<GlobalMain>();
    }

    //sets the main follow cam to the instance of the virtual camera in the plaers prefab object (i.e. Cowboy free look)
    static public void Set_Follow_Cam(string camName)
    {
        followCam_ = camName;
    }

    //returns the main camera
    //this needs to return only the gameObject so that it includes the audioListener and Cinemachine components.
    //to access the camera itself you can add the suffix call Get_Camera_Brain().GetComponent<Camera>();
    static public GameObject Get_Camera_Brain() 
    {
        return Get_Child("Camera Brain").gameObject;
    }

    //returns a reference to the instance of the player object saved/stored in _Global_
    static public GameObject Get_Player_Instance()
    {
        return Get_Child(playerObjectName_);
    }

    //saves/stores an instance of a prefab in _GLOBAL_ that we use as the player!
    static public void Set_Player_Instance(GameObject o)
    {
        Get_Main().Set_Player_Prefab(o);
        playerObjectName_ = o.name;
        Add_Global_Object(o);
    }

    //returns a list of prefabs as original reference templates to the party
    static public List<GameObject> Get_Party()
    {
        return Get_Main().Get_Party();
    }

    //returns a list of all the members of the party that have prefab instances in _GLOBAL_
    static public List<GameObject> Get_Party_Instances()
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

    //names and orders all the Globally Saved GameObjects.
    private static void Sort_Children()
    {
        int count_ = Get_Global().childCount;
        for(int i = 0; i < count_; i ++)
        {
            children[Get_Global().GetChild(i).name] = i;
        }
    }

    //---------------------------------------------------------
    //Static Globally Available Helpers!
    //---------------------------------------------------------

    //given a GO and a name, returns canvas matching the name
    public static Canvas Find_Canvas_In_Children(GameObject o, string name)
    {
        Canvas[] all = o.GetComponentsInChildren<Canvas>(true);
        foreach (Canvas canvas in all)
        {
            if (canvas.gameObject.name == name) { return canvas; }
        }
        return null;
    }

    //Triggers an animation, then WAITS for the animation to finish before returning!
    public static IEnumerator AnimWait(Animator animator, string trigger, string stateName)
    {
        //give an empty string to skip the animation triggering and do it manually.
        if(trigger != "")
        {
            animator.SetTrigger(trigger);
        }

        //wait for it to start
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
        {
            yield return null;
        }
        //wait until it's done animating
        while ((animator.GetCurrentAnimatorStateInfo(0).normalizedTime) < 1f)
        {
            yield return null;
        }

        yield return null;
    }
}
