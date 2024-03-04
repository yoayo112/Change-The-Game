/*
Project: Change the Game
File: Character.cs
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
public class Character : MonoBehaviour, IComparable
{
    //public CombatController combatController;
    
    public string characterName = "Place Holder";
    public CharacterType myType = CharacterType.player;

    public float armor;
    private float _armor;

    public int attackPower;
    private int _attackPower;

    public int healPower;
    private int _healPower;

    public int maxHealth;
    private int _maxHealth;
    private int _currentHealth;

    public int maxEnergy;
    private int _maxEnergy;
    private int _currentEnergy;

    public int speed;
    private int _speed;

    private bool _isAlive = true;

    private int _position = 0; // the index of position slot I am occupying in the Control Script
    private int _queuePosition = 0; // What index do I have in the Control Script's turn queue?
    private List<int> _targets;


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

    //------------------------------------------------------------------------------
    // Allow sorting based on speed. (Basically overloading List.Sort() here.)
    //------------------------------------------------------------------------------
    public int CompareTo(object obj)
    {
        var a = this;
        var b = obj as Character;

        int aSpeed = a.Get_Speed();
        int bSpeed = b.Get_Speed();

        if (aSpeed < bSpeed)
        {
            return 1;
        }
        if (aSpeed > bSpeed)
        {
            return -1;
        }

        return 0;
    }

    //-------------------------------------------------------------------
    // Accessors and Mutators
    //-------------------------------------------------------------------
    // Accessors
    //public CombatController Get_CombatController() => combatController;
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
    public int Get_Position() => _position;
    public bool Is_Alive() => _isAlive;
    public CharacterType Get_CharacterType() => myType;
    public List<int> Get_Targets() => _targets;


    // Mutators
    //public void Set_CombatController(CombatController combatController_) => combatController = combatController_;
    public void Set_Name(string name_) => characterName = name_;
    public void Set_Armor(float armor_) => _armor = armor_;
    public void Set_AttackPower(int attack_) => _attackPower = attack_;
    public void Set_HealPower(int heal_) => _healPower = heal_;
    public void Set_MaxHealth(int health_) => _maxHealth = health_;
    public void Set_CurrentHealth(int health_) => _currentHealth = health_;
    public void Set_MaxEnergy(int energy_) => _maxEnergy = energy_;
    public void Set_CurrentEnergy(int energy_) => _currentEnergy = energy_;
    public void Set_QueuePosition(int pos_) => _queuePosition = pos_;
    public void Set_Position(int pos_) => _position = pos_;
    public void Set_Alive(bool alive_) => _isAlive = alive_;
    public void Set_Speed(int speed_) => _speed = speed_;
    public void Set_CharacterType(CharacterType type_) => myType = type_;
    public void Set_Targets(List<int> targets_) => _targets = targets_;
    

    

    //----------------------------------------------------------------------------
    //  Handling Game Events.
    //  Incoming and Outgoing attacks.
    //  Incoming and Outgoing Healing.
    //----------------------------------------------------------------------------

    //Random generated effectiveness in place of enemy logic for now.
    public void Attack_Enemies(int[] targets_)
    {
        float effectiveness_ = UnityEngine.Random.Range(0f, 1.0f);

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

    public void Attack_Enemy(int target_, float effectiveness_)
    {
        int[] targets_ = { target_ };
        Attack_Enemies(targets_, effectiveness_);
    }

    public void Take_Damage(int[] targets_, float damage_)
    {
        if (targets_.Contains(_position))
        {
            if (_isAlive)
            {
                int actualDamage_ = (int)(damage_ * (1 - _armor));
                _currentHealth -= actualDamage_;
                //animate here
                Debug.Log("Character " + characterName + " Just took " + actualDamage_ + " damage!");
                if (_currentHealth <= 0)
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

    //Overloaded Take_Damage method that always targets self
    public void Take_Damage(float damage_)
    {
        int[] targets_ = { _position };
        Take_Damage( targets_, damage_);
    }

    public void Heal_Characters(int[] targets_)
    {
        float effectiveness_ = UnityEngine.Random.Range(0f, 1.0f);
        float health_ = _healPower * (effectiveness_ + 1);

        Debug.Log("Character " + characterName + "is healing someone for " + health_ + "health.");

        CombatEventManager.Heal_Damage(targets_, health_);
    }

    //Overloaded Heal for minigame effectiveness input.
    public void Heal_Characters(int[] targets_, float effectiveness_)
    {
        float health_ = _healPower * (effectiveness_ + 1);

        Debug.Log("Character " + characterName + "is healing someone for " + health_ + "health.");

        CombatEventManager.Heal_Damage(targets_, health_);
    }

    public void Heal_Character(int target_, float effectiveness_)
    {
        int[] targets_ = { target_ };
        Heal_Characters(targets_, effectiveness_);
    }

    public void Take_Healing(int[] targets_, float healing_)
    {
        if (targets_.Contains(_position))
        {
            if (_isAlive)
            {
                int actualHealing_ = (int)(healing_ * (1 + _armor));

                int temp = _currentHealth + actualHealing_;

                if (temp > _maxHealth)
                {
                    temp = _maxHealth;
                }

                _currentHealth = temp;
                Debug.Log("Character " + characterName + " Just got healed for " + actualHealing_ + " health!");
            }
        }
    }

    //Overloaded Take_Healing method that always targets self
    public void Take_Healing(float healing_)
    {
        int[] targets_ = { _queuePosition };
        Take_Healing(targets_, healing_);
    }


    //----------------------------------------------------------------------------
    //  Handling Events
    //----------------------------------------------------------------------------
    public void Start_Turn(int turnPos_)
    {
        if (_queuePosition == turnPos_)
        {
            Debug.Log("Character " + characterName + " is starting their turn!");

            if (Is_Alive())
                Execute_Turn();

            End_Turn();
        }
    }

    public void End_Turn()
    {
        Debug.Log("Character " + characterName + " is ending their turn.");
        CombatEventManager.End_Turn();
    }

    //----------------------------------------------------------------------------
    // Stubs for subclass methods. Override these when you extend the class
    //----------------------------------------------------------------------------
    public virtual void Set_Starting_Stats()
    // Initial Stats based on selected public values from unity inspector
    {
        Set_Armor(armor);
        Set_AttackPower(attackPower);
        Set_HealPower(healPower);
        Set_MaxHealth(maxHealth);
        Set_MaxEnergy(maxEnergy);
        Set_Speed(speed);

        Set_CurrentHealth(maxHealth);
        //Set_CurrentHealth(_currentHealth);

        Set_CurrentEnergy(maxEnergy);
        //Set_CurrentEnergy(_currentEnergy);
    }

    public virtual void Execute_Turn()
    {
        int target_ = UnityEngine.Random.Range(CombatController.MAX_PLAYERS, CombatController.MAX_PLAYERS + CombatController.enemies.Count);

        int[] targets_ = { target_ };

        Attack_Enemies(targets_);
    }

}