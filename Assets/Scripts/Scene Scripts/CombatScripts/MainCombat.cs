using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCombat : MonoBehaviour
{
    //damage event
    public delegate void damageAction();
    public static event damageAction takeDamage;

    private List<GameObject> allies = new List<GameObject>();
    private List<GameObject> enemies = new List<GameObject>();
    public void setAllies(List<GameObject> a)
    {
        allies = a;
    }
    public void setEnemies(List<GameObject> e)
    {
        enemies = e;
    }

    private List<GameObject> turnQueue = new List<GameObject>();
    private int currentTurn = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        //initialize allied forces
        foreach(GameObject ally in allies)
        {
            //do stuff

            //push into queue
            turnQueue.Add(ally);
            //spawn
        }
        //initialize enemy forces
        foreach(GameObject enemy in enemies)
        {
            //push into queue
            turnQueue.Add(enemy);
        }

        //sort the turnqueue
        turnQueue.Sort(
            );//delegate (GameObject a, GameObject b) { if (a.speed >= b.speed) { return a; } else if (b.speed > a.speed) { return b; } });
    }

    // Update is called once per frame
    void Update()
    {
        //Whoevers turn it is
        GameObject currentPlayer = turnQueue[currentTurn];
        //show this players UI
        // OR
        //take enemy actions.
    }

   // dealDamage()
}
