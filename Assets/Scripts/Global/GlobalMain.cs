using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalMain : MonoBehaviour
{
    //we can put global information here that can be both dynamic and static between scenes.
    private GameObject player_;
    public GameObject getPlayer()
    {
        return player_;
    }
    public void setPlayer(GameObject p)
    {
        player_ = p;
        //remove NPC movement script and anything else.
        //add playermovement.cs, free look camera, etc
        //initialize prefab as player object
    }
    private GameObject[] allCharacters;

    // Start is called before the first frame update
    void Start()
    {
        //set all characters as NPC's
        foreach(GameObject character in allCharacters)
        {
            //assign NPC movement script.
            //assign anything else specific to NPC's
        }

        //Make sure that the _Global_ Object is never destroyed.
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
