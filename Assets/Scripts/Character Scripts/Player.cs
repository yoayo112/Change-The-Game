using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    //Starting Values for when combat starts. We will revert to these at combat end.
    //Basically, only using these dictionaries to allow specific stats to be mutable 
    //while in combat, but not outside of combat.
    private StatsStruct _startingStats = new StatsStruct(
        0.1f, //armor
        20,   //attack power
        20,   //heal power
        100,  //max health
        100,  //max energy
        100  //speed
    );

    //These values can be changed during combat.
    private StatsStruct _combatStats = new StatsStruct();

    //------------------------------------------------------------------------------------
    // Methods for updating starting stats(permanent stats, update on level up or such.)
    //------------------------------------------------------------------------------------
    public void Set_PermanentArmor(float val_) => _startingStats.armor = val_;
    public void Set_PermanentAttackPower(int val_) => _startingStats.attackPower = val_;
    public void Set_PermanentHealPower(int val_) => _startingStats.healPower = val_;
    public void Set_PermanentMaxHealth(int val_) => _startingStats.maxHealth = val_;
    public void Set_PermanentMaxEnergy(int val_) => _startingStats.maxEnergy = val_;
    public void Set_PermanentSpeed(int val_) => _startingStats.speed = val_;
    
    //------------------------------------------------------------------------------------
    // Stat Modification methods (temporary modifications to stats during combat.)
    //------------------------------------------------------------------------------------
    public void Modify_Armor(float modVal_)
    {
        //do whatever calulation makes sense here.
        float modified_ = _combatStats.armor;

        modified_ -= modVal_;

        _combatStats.armor = modified_;
        Set_Armor(_combatStats.armor);
    }

    public void Modify_AttackPower(int modVal_)
    {
        int modified_ = _combatStats.attackPower;

        modified_ -= modVal_;

        _combatStats.attackPower = modified_;
        Set_AttackPower(modified_);
    }

    public void Modify_HealPower(int modVal_)
    {
        int modified_ = _combatStats.healPower;

        modified_ -= modVal_;

        _combatStats.healPower = modified_;
        Set_HealPower(modified_);
    }

    public void Modify_MaxHealth(int modVal_)
    {
        int modified_ = _combatStats.maxHealth;

        modified_ -= modVal_;

        _combatStats.maxHealth = modified_;
        Set_MaxHealth(modified_);
    }

    public void Modify_MaxEnergy(int modVal_)
    {
        int modified_ = _combatStats.maxEnergy;

        modified_ -= modVal_;

        _combatStats.maxEnergy = modified_;
        Set_MaxEnergy(modified_);
    }

    public void Modify_Speed(int modVal_)
    {
        int modified_ = _combatStats.speed;

        modified_ -= modVal_;

        _combatStats.speed = modified_;
        Set_Speed(modified_);
    }

    //----------------------------------------------------------------
    //Overridden Starting Method
    //----------------------------------------------------------------
    public override void Set_Starting_Stats()
    {
        _combatStats = _startingStats;
    
        Set_CharacterType(CharacterType.player);

        Set_Armor(_combatStats.armor);
        Set_AttackPower(_combatStats.attackPower);
        Set_HealPower(_combatStats.healPower);
        Set_MaxHealth(_combatStats.maxHealth);
        Set_MaxEnergy(_combatStats.maxEnergy);
        Set_Speed(_combatStats.speed);
    }
}
