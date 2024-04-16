/*
Project: Change the Game
File: Character.cs
Date Created: March 01, 2024
Author(s): Elijah Theander, Sean Thornton
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
using DG.Tweening;

[System.Serializable]
public class Character : MonoBehaviour, IComparable
{
    //public CombatController combatController;

    //a reference to the canvas image that needs to be modified with health change
    public GameObject healthBar;

    public string characterName = "Place Holder";
    private CharacterType _myType = CharacterType.player;
    private StatsStruct _currentStats = new StatsStruct(); //StatsStruct is defined in Scripts/Global/Stats_Struct
    private int _position = 0; // the index of position slot I am occupying in the Control Script
    private int _queuePosition = 0; // What index do I have in the Control Script's turn queue?

    protected bool _executingTurn = false;


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

    public int Get_Armor() => _currentStats.armor;
    public int Get_AttackPower() => _currentStats.attackPower;
    public int Get_HealPower() => _currentStats.healPower;
    public int Get_MaxHealth() => _currentStats.maxHealth;
    public int Get_CurrentHealth() => _currentStats.currentHealth;
    public int Get_MaxEnergy() => _currentStats.maxEnergy;
    public int Get_CurrentEnergy() => _currentStats.currentEnergy;
    public int Get_Speed() => _currentStats.speed;

    public int Get_QueuePosition() => _queuePosition;
    public int Get_Position() => _position;
    public bool Is_Alive() => _currentStats.currentHealth > 0;
    public CharacterType Get_CharacterType() => _myType;


    // Mutators
    //public void Set_CombatController(CombatController combatController_) => combatController = combatController_;
    public void Set_Name(string name_) => characterName = name_;
    public void Set_Armor(int armor_) => _currentStats.armor = armor_;
    public void Set_AttackPower(int attack_) => _currentStats.attackPower = attack_;
    public void Set_HealPower(int heal_) => _currentStats.healPower = heal_;
    public void Set_MaxHealth(int health_) => _currentStats.maxHealth = health_;
    public void Set_CurrentHealth(int health_) => _currentStats.currentHealth = health_;
    public void Set_MaxEnergy(int energy_) => _currentStats.maxEnergy = energy_;
    public void Set_CurrentEnergy(int energy_) => _currentStats.currentEnergy = energy_;
    public void Set_QueuePosition(int pos_) => _queuePosition = pos_;
    public void Set_Position(int pos_) => _position = pos_;
    public void Set_Speed(int speed_) => _currentStats.speed = speed_;

    public void Set_CharacterType(CharacterType type_) => _myType = type_;

    public void Set_StatsStruct(StatsStruct value_) => _currentStats = value_;

    //----------------------------------------------------------------------------
    //  Handling Game Events.
    //  Incoming and Outgoing attacks.
    //  Incoming and Outgoing Healing.
    //----------------------------------------------------------------------------

    //Random generated effectiveness in place of enemy logic for now.
    public void Attack_Characters(CharacterType type_, int[] targets_)
    {
        float effectiveness_ = UnityEngine.Random.Range(0f, 1.0f);

        float damage_ = _currentStats.attackPower * (effectiveness_ + 1);

        Debug.Log("Character " + characterName + " is attacking with " + damage_ + " damage.");

        CombatEventManager.Deal_Damage(type_, targets_, damage_); //Tell the world what's up
    }

    //Overloaded attack for player minigame effectiveness input.
    public void Attack_Characters(CharacterType type_, int[] targets_, float effectiveness_)
    {
        float damage_ = _currentStats.attackPower * (effectiveness_ + 1);

        Debug.Log("Character " + characterName + " is attacking with " + damage_ + " damage.");

        CombatEventManager.Deal_Damage(type_, targets_, damage_);
    }

    public void Attack_Character(CharacterType type_, int target_, float effectiveness_)
    {
        int[] targets_ = { target_ };
        Attack_Characters(type_, targets_, effectiveness_);
    }

    public void Take_Damage(CharacterType type_, int[] targets_, float damage_)
    {

        if (_myType == type_ && targets_.Contains(_position))
        {
            if (Is_Alive())
            {
                //modify stats
                int actualDamage_ = (int) (damage_ * 100f / (_currentStats.armor + 100f));
                _currentStats.currentHealth -= actualDamage_;
                
                //we only need to show health bar on players
                if (_myType == CharacterType.player)
                {
                    float newHealthPercentage_ = (float)_currentStats.currentHealth / (float)_currentStats.maxHealth;
                    Debug.Log(newHealthPercentage_);
                    //TODO: animate health bar changing instead of just setting its value.
                    healthBar.transform.GetChild(1).GetComponent<Image>().DOFillAmount(newHealthPercentage_, 2f);
                }

                Debug.Log("Character " + characterName + " just took " + actualDamage_ + " damage!");
                Animator _animator = gameObject.GetComponentInChildren<Animator>();

                if (!Is_Alive())
                {
                    //animate death
                    _animator.SetTrigger("Death");
                    Debug.Log("Character " + characterName + " has perished...");
                }
                else
                {
                    //animate getting hurt (but still alive)
                    Debug.Log(characterName + " Hurt");
                    _animator.SetTrigger("Hurt");
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
        Take_Damage(_myType, targets_, damage_);
    }

    public void Heal_Characters(CharacterType type_, int[] targets_)
    {
        float effectiveness_ = UnityEngine.Random.Range(0f, 1.0f);
        float health_ = _currentStats.healPower * (effectiveness_ + 1);

        Debug.Log("Character " + characterName + "is healing someone for " + health_ + "health.");

        CombatEventManager.Heal_Damage(type_, targets_, health_);
    }

    //Overloaded Heal for minigame effectiveness input.
    public void Heal_Characters(CharacterType type_, int[] targets_, float effectiveness_)
    {
        float health_ = _currentStats.healPower * (effectiveness_ + 1);

        Debug.Log("Character " + characterName + "is healing someone for " + health_ + "health.");

        CombatEventManager.Heal_Damage(type_, targets_, health_);
    }

    public void Heal_Character(CharacterType type_, int target_, float effectiveness_)
    {
        int[] targets_ = { target_ };
        Heal_Characters(type_, targets_, effectiveness_);
    }

    public void Take_Healing(CharacterType type_, int[] targets_, float healing_)
    {
        if (_myType == type_ && targets_.Contains(_position))
        {
            if (Is_Alive())
            {
                int temp = (int) (_currentStats.currentHealth + healing_);

                if (temp > _currentStats.maxHealth)
                    temp = _currentStats.maxHealth;

                int actualHealing_ = temp - _currentStats.currentHealth;
                _currentStats.currentHealth = temp;

                Debug.Log("Character " + characterName + " Just got healed for " + actualHealing_ + " health!");
            }
        }
    }

    //Overloaded Take_Healing method that always targets self
    public void Take_Healing(float healing_)
    {
        int[] targets_ = { _position };
        Take_Healing(_myType, targets_, healing_);
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
            {
                _executingTurn = true;
                Execute_Turn();
                StartCoroutine(Wait_For_Gameplay());
            }
        }
    }

    public void End_Turn()
    {
        Debug.Log("Character " + characterName + " is ending their turn.");
        CombatEventManager.End_Turn();
    }

    public IEnumerator Wait_For_Gameplay()
    {
        yield return new WaitWhile(()=> _executingTurn);
        _executingTurn = false;
        End_Turn();
    }

    //----------------------------------------------------------------------------
    // Stubs for subclass methods. Override these when you extend the class
    //----------------------------------------------------------------------------
    public virtual void Set_Starting_Stats()
    // Initial Stats based on selected public values from unity inspector
    {
        Set_Armor(20);
        Set_AttackPower(20);
        Set_HealPower(20);
        Set_MaxHealth(100);
        Set_MaxEnergy(100);
        Set_Speed(100);

        Set_CurrentHealth(100);
        //Set_CurrentHealth(_currentHealth);

        Set_CurrentEnergy(100);
        //Set_CurrentEnergy(_currentEnergy);
    }

    public virtual void Execute_Turn()
    {
        //Hitting a random enemy target by default
        int target_ = UnityEngine.Random.Range(0, CombatController.enemies.Count);

        int[] targets_ = { target_ };

        Attack_Characters(CharacterType.enemy, targets_);
    }
}