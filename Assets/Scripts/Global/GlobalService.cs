using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalService
{
    static public GlobalMain Get_Main() 
    {
        return GameObject.Find("_GLOBAL_").GetComponent<Transform>().GetChild(0).GetComponent<GlobalMain>();
    }

}
