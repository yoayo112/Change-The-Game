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
    public override IEnumerator Execute_Turn()
    {
        Find_Canvas("CombatGUI").gameObject.SetActive(true);
        yield return new WaitWhile(() => Find_Canvas("CombatGUI").gameObject.activeInHierarchy);
        //yield return null;
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

    //-----------------------------------------------------------------
    //Helper functions
    //-----------------------------------------------------------------
    public Canvas Find_Canvas(string name)
    {
        Canvas[] all = gameObject.GetComponentsInChildren<Canvas>(true);
        foreach (Canvas canvas in all)
        {
            if (canvas.gameObject.name == name) { return canvas; }
        }
        return null;
    }

    private IEnumerator In_GUI()
    {
        while (true) { if (!Find_Canvas("CombatGUI").gameObject.activeInHierarchy) { yield return null; } }
    }
}
