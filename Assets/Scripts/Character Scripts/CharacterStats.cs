/*
Project: Change the Game
File: CharacterStats.cs
Date Created: March 01, 2024
Author(s): Elijah Theander
Info:
Character Stats script that stores stats about characters,
handles whether or not a character is living, and processes
outgoing and incoming damage with events.

*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public enum CharacterType //Globally Public Enum for if a character is a player or enemy
{
    player,
    enemy
}

[System.Serializable]
public class CharacterStats : MonoBehaviour, IComparable
{

    public string characterName = "Place Holder";
    public float armor = 0.1f;
    private float _armor = 0.1f;

    public int attackPower = 20;
    private int _attackPower = 20;

    public int healPower = 20;
    private int _healPower = 20;

    public int maxHealth = 100;
    private int _maxHealth = 100;

    private int _currentHealth = 100;
    
    public int maxEnergy = 100;
    private int _maxEnergy = 100;
    private int _currentEnergy = 100;

    public int speed = 100;
    private int _speed = 100;

    private bool _isAlive = true;

    private int _queuePosition = 0; // What index do I have in the Control Script's turn queue?

    

    public CharacterType myType = CharacterType.player;// I am a player or an enemy

    private void OnEnable() //Subscrizzle.
    {
        CombatEventManager.onStartTurn += Start_Turn;
        CombatEventManager.onDamage += Take_Damage;
        CombatEventManager.onHeal += Take_Healing;
    }
    private void OnDisable() //De-subscrizzle
    {
        CombatEventManager.onStartTurn -= Start_Turn;
        CombatEventManager.onDamage -= Take_Damage;
        CombatEventManager.onHeal -= Take_Healing;
    }

    void Awake()
    {
        //We want this to happen before Start().
        //IF it happens at the same time as start, the Character controller doesn't see the updated values.
        Set_Starting_Stats();
    }

    //-------------------------------------------------------------------
    // Accessors and Mutators
    //-------------------------------------------------------------------
    // Accessors
    public string Get_Name() => characterName;
    public float Get_Armor() => _armor;
    public int Get_AttackPower() => _attackPower;
    public int Get_HealPower() => _healPower;
    public int Get_MaxHealth() => _maxHealth;
    public int Get_CurrentHealth() => _currentHealth;
    public int Get_MaxEnergy() => _maxEnergy;
    public int Get_CurrentEnergy() => _currentEnergy;
    public int Get_Speed() => _speed;
    public int Get_QueuePosition() => _queuePosition;
    public bool Is_Alive() => _isAlive;
    
    // Mutators
    public void Set_Name(string name_) => characterName = name_; 
    public void Set_Armor(float armor_) => _armor = armor_;
    public void Set_AttackPower(int attack_) => _attackPower = attack_;
    public void Set_HealPower(int heal_) => _healPower = heal_;
    public void Set_MaxHealth(int health_) => _maxHealth = health_;
    public void Set_CurrentHealth(int health_) => _currentHealth = health_;
    public void Set_MaxEnergy(int energy_) => _maxEnergy = energy_;
    public void Set_CurrentEnergy(int energy_) => _currentEnergy = energy_;
    public void Set_QueuePosition(int pos_) => _queuePosition = pos_;
    public void Set_Alive(bool alive_) => _isAlive = alive_;
    public void Set_Speed(int speed_) => _speed = speed_;

    // Initial Stats based on selected public values from unity inspector
    private void Set_Starting_Stats()
    {
        Set_Armor(armor);
        Set_AttackPower(attackPower);
        Set_HealPower(healPower);
        Set_MaxHealth(maxHealth);
        Set_CurrentHealth(maxHealth);
        Set_MaxEnergy(maxEnergy);
        Set_CurrentEnergy(maxEnergy);
        Set_Speed(speed);
    }

    //------------------------------------------------------------------------------
    // Allow sorting based on speed. (Basically overloading List.Sort() here.)
    //------------------------------------------------------------------------------
    public int CompareTo(object obj)
    {
        var a = this;
        var b = obj as CharacterStats;

        int aSpeed = a.Get_Speed();
        int bSpeed = b.Get_Speed();

        if(aSpeed < bSpeed)
        {
            return 1;
        }
        if(aSpeed > bSpeed)
        {
            return -1;
        }

        return 0;
    }

    //----------------------------------------------------------------------------
    //  Handling Game Events.
    //  Incoming and Outgoing attacks.
    //  Incoming and Outgoing Healing.
    //----------------------------------------------------------------------------

    //Random generated effectiveness in place of enemy logic for now.
    public void Attack_Enemies(int[] targets_)
    {
        float effectiveness_ = UnityEngine.Random.Range(0f,1.0f);

        float damage_ = _attackPower * (effectiveness_ + 1);

        Debug.Log("Character " + characterName + " is attacking with " + damage_ + "damage.");

        CombatEventManager.Deal_Damage(targets_, damage_); //Tell the world what's up
    }

    //Overloaded attack for player minigame effectiveness input.
    public void Attack_Enemies(int[] targets_, float effectiveness_)
    {
        float damage_ = _attackPower * (effectiveness_ + 1);

        Debug.Log("Character " + characterName + " is attacking with " + damage_ + "damage.");

        CombatEventManager.Deal_Damage(targets_, damage_);
    }

    public void Take_Damage(int[] targets_, float damage_)
    {
        if(targets_.Contains(_queuePosition))
        {
            if(_isAlive)
            {
                int actualDamage_ = (int)(damage_ * (1 - _armor));
                _currentHealth -= actualDamage_;
                //animate here
                Debug.Log("Character " + characterName + " Just took " + actualDamage_ + " damage!");
                if(_currentHealth <= 0)
                {
                    _isAlive = false;
                    Debug.Log("Character " + characterName + "has perished...");
                }
            }
            else
            {
                Debug.Log("Character " + characterName + " is still dead.");
            }
        }
    }

    public void Heal_Characters(int [] targets_)
    {
        float effectiveness_ = UnityEngine.Random.Range(0f, 1.0f);
        float health_ = _healPower * (effectiveness_ + 1);

        Debug.Log("Character " + characterName + "is healing someone for " + health_ + "health.");
        
        CombatEventManager.Heal_Damage(targets_, health_);
    }

    //Overloaded Heal for minigame effectiveness input.
    public void Heal_Characters(int [] targets_, float effectiveness_)
    {
        float health_ = _healPower * (effectiveness_ + 1);

        Debug.Log("Character " + characterName + "is healing someone for " + health_ + "health.");
        
        CombatEventManager.Heal_Damage(targets_, health_);
    }

    public void Take_Healing(int[] targets_, float healing_)
    {
        if(targets_.Contains(_queuePosition))
        {
            if(_isAlive)
            {
                int actualHealing_ = (int)(healing_ * (1 + _armor));

                int temp = _currentHealth + actualHealing_;

                if(temp > _maxHealth)
                {
                    temp = _maxHealth;
                }

                _currentHealth = temp;
                Debug.Log("Character " + characterName + " Just got healed for " + actualHealing_ + " health!");
            }
        }
    }
    

    //----------------------------------------------------------------------------
    //  Handling Events
    //----------------------------------------------------------------------------
    public void Start_Turn(int turnPos_)
    {
        if(_queuePosition == turnPos_)
        {
            Debug.Log("Character " + characterName + " is starting their turn!");
            //Start turn here

            //In place of just attacking a random target, we either do game logic for enemies,
            // Or enable UI elements/camera view for players.

            int target_ = UnityEngine.Random.Range(0,6);

            int[] targets_ = {target_};

            Attack_Enemies(targets_);
            End_Turn();
        }
    }

    public void End_Turn()
    {
        Debug.Log("Character " + characterName + " is ending their turn.");
        CombatEventManager.End_Turn();
    }
}
