/*
Project: Change the Game
File: Character.cs
Date Created: March 01, 2024
Author(s): Elijah Theander, Sean Thornton, Sky Vercauteren
Info:
player Stats script that stores stats about players,
handles whether or not a character is living, and processes
outgoing and incoming damage with events. extends Character.cs

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class Player : Character
{
    //Starting Values for when combat starts. We will revert to these at combat end.
    //Basically, only using these dictionaries to allow specific stats to be mutable 
    //while in combat, but not outside of combat.
    protected StatsStruct _permanentStats = new StatsStruct(
        50, //armor
        20,   //attack power
        20,   //heal power
        100,  //max health
        100,  //max energy
        100  //speed
    );

    //temp storage for each turn.
    protected int[] _targets;
    protected float _effectiveness;

    //minigame vars
    private Camera _main;
    private Camera _overlay;
    protected string _title;
    protected string _trigger;
    protected string _actionState;
    private GameObject _minigame;

    //------------------------------------------------------------------------------------
    // Methods for updating starting stats(permanent stats, update on level up or such.)
    //------------------------------------------------------------------------------------
    public void Set_PermanentArmor(int val_) => _permanentStats.armor = val_;
    public void Set_PermanentAttackPower(int val_) => _permanentStats.attackPower = val_;
    public void Set_PermanentHealPower(int val_) => _permanentStats.healPower = val_;
    public void Set_PermanentMaxHealth(int val_) => _permanentStats.maxHealth = val_;
    public void Set_PermanentMaxEnergy(int val_) => _permanentStats.maxEnergy = val_;
    public void Set_PermanentSpeed(int val_) => _permanentStats.speed = val_;
    public void Set_PermanentStats(int armor_, int ap_, int hp_, int maxHealth_, int maxEnergy_, int speed_)
    {
        Set_PermanentArmor(armor_);
        Set_PermanentAttackPower(ap_);
        Set_PermanentHealPower(hp_);
        Set_PermanentMaxEnergy(maxEnergy_);
        Set_PermanentMaxHealth(maxHealth_);
        Set_PermanentSpeed(speed_);
    }
    
    //------------------------------------------------------------------------------------
    // Stat Modification methods (temporary modifications to stats during combat.)
    //------------------------------------------------------------------------------------
    public void Modify_Armor(int modVal_)
    {
        //do whatever calulation makes sense here.
        int modified_ = Get_Armor();

        modified_ -= modVal_;

        Set_Armor(modified_);
    }

    public void Modify_AttackPower(int modVal_)
    {
        int modified_ = Get_AttackPower();

        modified_ -= modVal_;

        Set_AttackPower(modified_);
    }

    public void Modify_HealPower(int modVal_)
    {
        int modified_ = Get_HealPower();

        modified_ -= modVal_;

        Set_HealPower(modified_);
    }

    public void Modify_MaxHealth(int modVal_)
    {
        int modified_ = Get_MaxHealth();

        modified_ -= modVal_;

        Set_MaxHealth(modified_);
    }

    public void Modify_MaxEnergy(int modVal_)
    {
        int modified_ = Get_MaxEnergy();

        modified_ -= modVal_;

        Set_MaxEnergy(modified_);
    }

    public void Modify_Speed(int modVal_)
    {
        int modified_ = Get_Speed();

        modified_ -= modVal_;

        Set_Speed(modified_);
    }

    //----------------------------------------------------------------
    //Overridden Starting Method
    //----------------------------------------------------------------
    public override void Set_Starting_Stats()
    {
        Set_StatsStruct(_permanentStats);
        Set_CharacterType(CharacterType.player);
    }

    //---------------------------------------------------------------
    //Overriden Execute_Turn Method to show player GUI
    //---------------------------------------------------------------
    public override void Execute_Turn()
    {
        GlobalService.Find_Canvas_In_Children(gameObject, "CombatGUI").gameObject.SetActive(true);
    }

    //----------------------------------------------------------------
    //Minigame and Targetting System
    //----------------------------------------------------------------

    //Called on button click. Wrapper for minigame coroutine.
    //PLZ OVERRIDE ME: for game title, animation state, and trigger for each character!!
    //This won't work with these empty strings!
    public virtual void Minigame_Button()
    {
        _title = "";
        _trigger = "";
        _actionState = "";
        StartCoroutine(Select_Target());
    }

    //use me to find who I am targetting.
    public virtual IEnumerator Select_Target()
    {
        _targets = new int[] { Random.Range(0, 2) };

        yield return new WaitWhile(() => _targets.Length < 1);
        StartCoroutine(Run_Minigame());
    }


    //Use me to trigger and run gameplay, 
    //or override me for a specific/unique minigame.
    public virtual IEnumerator Run_Minigame()
    {
        //display minigame
        _minigame = GameObject.Find(_title);
        _main = GameObject.Find("Main Camera").GetComponent<Camera>();
        _overlay = _minigame.GetComponentInChildren<Camera>();
        _main.GetUniversalAdditionalCameraData().cameraStack.Add(_overlay);
        GlobalService.Find_Canvas_In_Children(gameObject, "CombatGUI").gameObject.SetActive(false);

        //broadcast start
        _minigame.GetComponentInChildren<MiniGameTimer>().Start_Countdown();

        //wait for gameplay to start
        yield return new WaitWhile(() => !_minigame.GetComponentInChildren<TypingGame>().Get_isRunning());

        //wait for minigame to finish
        yield return new WaitWhile(() => _minigame.GetComponentInChildren<TypingGame>().Get_isRunning());

        //then get the effictiveness
        _effectiveness = _minigame.GetComponentInChildren<TypingGame>().Get_effectiveness();
        _main.GetUniversalAdditionalCameraData().cameraStack.Remove(_overlay);

        //animate and wait for animation to finish
        Animator animator = gameObject.GetComponentInChildren<Animator>();
        yield return GlobalService.AnimWait(animator, _trigger, _actionState); //TODO: how to better sync attack anim with hurt anim??

        //Broadcast the event
        //TODO: can we make the character action methods into delegates?
        // -> This way, a function could be passed as an arg on button click and called here,
        //    I.e. minigames could trigger and broadcast other actions, not just attack.
        Attack_Characters(CharacterType.enemy, _targets, _effectiveness); 

        //End the turn
        _executingTurn = false;
    }

    //-----------------------------------------------------------------
    // Writing currentHealth and currentEnergy values back into
    // "permanent" stats
    //-----------------------------------------------------------------

    public void End_Combat()
    {
        _permanentStats.currentHealth = Get_CurrentHealth();
        _permanentStats.currentEnergy = Get_CurrentEnergy();
    }
}
