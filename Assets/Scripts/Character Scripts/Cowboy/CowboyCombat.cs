using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CowboyCombat : MonoBehaviour
{
    public float armor = 1f;
    public float attackPower = 2f;
    public float health = 10f;
    public float energy = 10f;
    public float speed = 1f;
    private int myIndex = 0;
    public DealDamage d_event;

    public void doAttack()
    {
        //get targeted enemy
        int[] enemyIndexes = { 0, 1 };
        attack(enemyIndexes);
    }

    public void attack(int[] targets)
    {
        //trigger minigame
        float effectiveness = 0.5f;

        float damage = attackPower * (effectiveness +1);
        //generate damage
        
        d_event.Invoke(targets, damage);
        
        //d_event = null;
    }

    public void onDamage(int[] targets, float damage)
    {
        if (0 == myIndex)
        {
            //decrement health
            health -= (int) (damage / armor) * 100;
            //animate
        }
        //else do nothing.
        //check if event contains cowboy index.
    }

    public void heal()
    {
        
    }

    void Start()
    {
        d_event = new DealDamage();
        d_event.AddListener(onDamage);
    }
}