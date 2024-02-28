using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class CowboyCombat : MonoBehaviour
{
    private float armor = 0.1f;
    private float attackPower = 20f;
    private float health = 100f;
    private float energy = 100f;
    private float speed = 10f;
    private int myIndex = 0;
    
    private void OnEnable()
    {
        CombatEventManager.onDamage += Take_Damage;
    }
    private void OnDisable()
    {
        CombatEventManager.onDamage -= Take_Damage;
    }

    public void Do_Attack()
    {
        //get targeted enemy
        Debug.Log("Button Clicked");
        int[] enemyIndexes = { 0, 1 };

        //trigger minigame
        float effectiveness = 0.5f;

        float damage = attackPower * (effectiveness + 1);
        //generate damage

        Debug.Log("Attacking with " + damage + " damage.");

        CombatEventManager.Take_Damage(enemyIndexes, damage);
    }


    public void Take_Damage(int[] targets, float damage)
    {
        if (targets.Contains(myIndex))
        {
            
            //decrement health
            health -= (int) (damage * (1 - armor));
            Debug.Log("Taking " + damage + " damage. New health is " + health + ".");
            //animate
        }
        //else do nothing.
        //check if event contains cowboy index.
    }

    public void heal()
    {
        
    }
}