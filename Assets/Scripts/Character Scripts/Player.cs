using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    //Starting Values for when combat starts. We will revert to these at combat end.
    //Basically, only using these dictionaries to allow specific stats to be mutable 
    //while in combat, but not outside of combat.
    private StatsStruct _permanentStats = new StatsStruct(
        0.1f, //armor
        20,   //attack power
        20,   //heal power
        100,  //max health
        100,  //max energy
        100  //speed
    );

    //------------------------------------------------------------------------------------
    // Methods for updating starting stats(permanent stats, update on level up or such.)
    //------------------------------------------------------------------------------------
    public void Set_PermanentArmor(float val_) => _permanentStats.armor = val_;
    public void Set_PermanentAttackPower(int val_) => _permanentStats.attackPower = val_;
    public void Set_PermanentHealPower(int val_) => _permanentStats.healPower = val_;
    public void Set_PermanentMaxHealth(int val_) => _permanentStats.maxHealth = val_;
    public void Set_PermanentMaxEnergy(int val_) => _permanentStats.maxEnergy = val_;
    public void Set_PermanentSpeed(int val_) => _permanentStats.speed = val_;
    
    //------------------------------------------------------------------------------------
    // Stat Modification methods (temporary modifications to stats during combat.)
    //------------------------------------------------------------------------------------
    public void Modify_Armor(float modVal_)
    {
        //do whatever calulation makes sense here.
        float modified_ = Get_Armor();

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
