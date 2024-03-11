/*
Project: Change the Game
File: GlobalService.cs
Date Created: March 05, 2024
Author(s): Sean Thornton
Info:

Service with functions returning the global transform and its components. 
All methods are static. No constructor, not to be instantiated.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalService
{
    static public Transform Get_Global()
    {
        return GameObject.Find("_GLOBAL_").GetComponent<Transform>();
    }

    static public GlobalMain Get_Main() 
    {
        return Get_Global().GetChild(0).GetComponent<GlobalMain>();
    }

    static public Camera Get_Camera() 
    {
        return Get_Global().GetChild(1).GetComponent<Camera>();
    }

    static public GameObject Get_Player()
    {
        return Get_Main().GetPlayer();
    }

    static public List<GameObject> Get_Party()
    {
        return Get_Main().GetParty();
    }



}
